using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using me.cg360.spookums.core.scheduler.task;

namespace me.cg360.spookums.core.scheduler
{
    public class CommandingScheduler : SchedulingType
    {
        private static CommandingScheduler _primaryInstance;

        private ConcurrentDictionary<Guid, Scheduler> Children { get; }

        public CommandingScheduler() {
            Children = new ConcurrentDictionary<Guid, Scheduler>();
        }

        public void SetAsPrimaryInstance() {
            _primaryInstance = this;
        }

        

        public void SyncSchedulers(Scheduler[] children) {
            foreach (Scheduler s in children)
            {
                Children.TryAdd(s.SchedulerID, s);
            }
        }

        public void desyncSchedulers(Scheduler[] children) {
            foreach (Scheduler s in children)
            {
                Children.TryRemove(s.SchedulerID, out _);
            }
        }

        
        
        public override bool StartScheduler() {
            if(!IsRunning) {
                IsRunning = true;
                return true;
            }
            return false;
        }
        
        public override bool StopScheduler() {
            if(IsRunning)  {
                PauseScheduler();
                Children.Clear();
                return true;
            }
            return false;
        }
        
        public override void PauseScheduler() {
            IsRunning = false;
        }
        
        public override void RunSchedulerTick() {

            if(IsRunning) {
                // Must duplicate the set to allow schedulers to remove themselves.
                foreach (Scheduler scheduler in new ConcurrentDictionary<Guid, Scheduler>(Children).Values) {
                    scheduler.RunSchedulerTick();
                }
            }
        }


        public static CommandingScheduler Get()
        {
            return _primaryInstance;
        }
    }
}