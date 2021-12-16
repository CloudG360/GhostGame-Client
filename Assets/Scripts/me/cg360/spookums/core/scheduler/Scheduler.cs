using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using me.cg360.spookums.utility;
using UnityEngine;

namespace me.cg360.spookums.core.scheduler.task
{
    public class Scheduler : SchedulingType
    {
        public readonly Guid SchedulerID;

        protected readonly List<Thread> ActiveThreads;
        protected readonly List<SchedulerTaskEntry> SchedulerTasks;

        public Scheduler(int tickDelay) {
            SchedulerID = Guid.NewGuid();

            TickDelay = Math.Max(1, tickDelay);

            SchedulerTasks = new List<SchedulerTaskEntry>();
            ActiveThreads = new List<Thread>();
        }



        // -- Control --

        /** Enables ticking on the scheduler */
        public override bool StartScheduler() { return StartScheduler(true); }
        public bool StartScheduler(bool tickWithServer) {
            if(!IsRunning) 
            {
                IsRunning = true;
                if(tickWithServer) Main.Client.SchedulerBrain.SyncSchedulers(new []{this});
                return true;
            }
            return false;
        }

        /**
         * Removes scheduler's hook to the server tick whilst clearing the queue.
         * @return true if the scheduler was initially running and then stopped.
         */
        public override bool StopScheduler() {
            if(IsRunning) 
            {
                PauseScheduler();
                ClearQueuedSchedulerTasks();
                return true;
            }
            return false;
        }

        /** Removes scheduler's hook to the server tick whilst clearing the queue */
        public override void PauseScheduler() {
            IsRunning = false;
            Main.Client.SchedulerBrain.DesyncSchedulers(new []{this});
        }

        /** Clears all the tasks queued in the scheduler. */
        public void ClearQueuedSchedulerTasks() {
            foreach(SchedulerTaskEntry entry in new List<SchedulerTaskEntry>(SchedulerTasks)) 
            {
                entry.Task.cancel(); // For the runnable to use? idk
                SchedulerTasks.Remove(entry);
            }
        }



        // -- Ticking --
        // Methods used to tick a scheduler should only be triggered by the
        // main scheduler thread.

        /** Executes a scheduler tick, running any tasks due to run on this tick. */
        public override void RunSchedulerTick() {
            if(IsRunning) {

                // To avoid stopping the scheduler from inside a task making it scream, use ArrayList wrapping
                foreach(SchedulerTaskEntry task in new List<SchedulerTaskEntry>(SchedulerTasks)) 
                {
                    long taskTick = task.NextTick;

                    if(taskTick == SchedulerTick) 
                    {

                        // Cancelled tasks shouldn't be in the scheduler queue anyway.
                        if(!task.Task.IsCancelled) 
                        {

                            if(task.IsAsynchronous)
                            {
                                GameThread thread;
                                thread = new GameThread(() =>
                                {
                                    lock (ActiveThreads)
                                    {
                                        ActiveThreads.Add(Thread.CurrentThread);
                                    }

                                    try
                                    {
                                        task.Task.run();
                                    }
                                    catch (Exception err)
                                    {
                                        Debug.LogError("Error thrown in a scheduler (asynchronous) task:");
                                        Debug.LogException(err);
                                    }

                                    lock (ActiveThreads)
                                    {
                                        ActiveThreads.Remove(Thread.CurrentThread);
                                    }
                                });
                                
                                thread.Start();

                            } else {
                                // Run as sync. This task must complete before the next one
                                // is ran.
                                try {
                                    task.Task.run();

                                } catch (Exception err) {
                                    Debug.LogError("Error thrown in a scheduler (synchronous) task:");
                                    Debug.LogException(err);
                                }
                            }


                            // Not cancelled by the call of #run() + it's a repeat task.
                            if(task.IsRepeating() && (!task.Task.IsCancelled)) {
                                long targetTick = taskTick + task.RepeatInterval;

                                SchedulerTaskEntry newTask = new SchedulerTaskEntry(task.Task, task.RepeatInterval, targetTick, task.IsAsynchronous);
                                QueueTaskEntry(newTask);
                            }
                        }

                    } 
                    else if(taskTick > SchedulerTick) {
                        // Upcoming task, do not remove from queue! :)
                        break;
                    }

                    SchedulerTasks.Remove(task); // Operate like a queue.
                    // Remove from the start as long as it isn't an upcoming task.
                    // If a task is somehow scheduled *before* the current tick, it should
                    // be removed anyway.
                }
                SchedulerTick++; // Tick after so tasks can be ran without a delay.
            }
        }



        // -- Task Control --

        protected void QueueTaskEntry(SchedulerTaskEntry entry){
            if(entry.NextTick <= SchedulerTick) throw new InvalidOperationException("Task cannot be scheduled before the current tick.");

            int size = SchedulerTasks.Count;
            for(int i = 0; i < size; i++) {
                // Entry belongs before task? Insert into it's position
                if(SchedulerTasks[i].NextTick > entry.NextTick) {
                    SchedulerTasks.Insert(i, entry);
                    return;
                }
            }

            // Not added in loop. Append to the end.
            SchedulerTasks.Add(entry);
        }



        // -- Task Registering --

        public PendingEntryBuilder PrepareTask(ThreadStart task) {
            SchedulerTask rTask = new RunnableTypeTask(task);
            return new PendingEntryBuilder(this, rTask);
        }

        public PendingEntryBuilder PrepareTask(SchedulerTask task) {
            return new PendingEntryBuilder(this, task);
        }
        
        public sealed class SchedulerTaskEntry
        {
            public readonly SchedulerTask Task;
            public readonly int RepeatInterval;
            public readonly long NextTick;
            public readonly bool IsAsynchronous;

            public SchedulerTaskEntry(SchedulerTask task, int repeatInterval, long nextTick, bool isAsynchronous) {
                Check.NullParam(task, "task");

                Task = task;
                RepeatInterval = repeatInterval;
                NextTick = nextTick;
                IsAsynchronous = isAsynchronous;
            }
        
            public bool IsRepeating() { return RepeatInterval > 0; }
        }

        public class PendingEntryBuilder {

            protected readonly Scheduler Scheduler;
            protected readonly SchedulerTask Task;

            protected int Interval;
            protected int Delay;
            protected bool IsAsynchronous;

            internal PendingEntryBuilder(Scheduler scheduler, SchedulerTask task) {
                Scheduler = Check.NullParam(scheduler, "scheduler");
                Task = Check.NullParam(task, "task");

                Interval = 0;
                Delay = 0;
                IsAsynchronous = false;
            }

            public SchedulerTask Schedule() {
                lock (Scheduler) {
                    long nextTick = Scheduler.SchedulerTick + Delay + 1;
                    SchedulerTaskEntry entry = new SchedulerTaskEntry(Task, Interval, nextTick, IsAsynchronous);
                    Scheduler.QueueTaskEntry(entry);
                }
                return Task;
            }
            
            

            public PendingEntryBuilder SetInterval(int interval) {
                Interval = Check.InclusiveLowerBound(interval, 0, "interval");
                return this;
            }

            public PendingEntryBuilder SetDelay(int delay) {
                Delay = Check.InclusiveLowerBound(delay, 0, "delay");
                return this;
            }

            public PendingEntryBuilder SetAsynchronous(bool asynchronous) {
                IsAsynchronous = asynchronous;
                return this;
            }
        }
    }
}