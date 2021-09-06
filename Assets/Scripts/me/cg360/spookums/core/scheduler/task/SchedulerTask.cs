using System;

namespace me.cg360.spookums.core.scheduler.task
{
    public abstract class SchedulerTask
    {
        
        public readonly Guid TaskID;
        public bool IsCancelled { get; private set; }

        public SchedulerTask() {
            TaskID = Guid.NewGuid();
            IsCancelled = false;
        }


        /** Executes the task. */
        public abstract void run();
        
        /** Sets the task as cancelled. */
        public void cancel() { this.IsCancelled = true; }
    }
}