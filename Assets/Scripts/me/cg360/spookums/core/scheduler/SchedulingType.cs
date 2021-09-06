namespace me.cg360.spookums.core.scheduler
{
    public abstract class SchedulingType
    {
        public long SchedulerTick { get; protected set; } // The scheduler's actual tick. This depends on the tickrate
        public long SyncedTick { get; protected set; }    // The last received server tick.
        public bool IsRunning { get; protected set; }

        public int TickDelay { get; protected set; } // The amount of server ticks between each scheduler tick.

        protected SchedulingType() {
            SyncedTick = 0;
            SchedulerTick = 0;
            IsRunning = false;

            TickDelay = 1;
        }


        public abstract bool StartScheduler();
        public abstract bool StopScheduler();
        public abstract void PauseScheduler();

        public abstract void RunSchedulerTick();

        /**
         * Ran to indicate a server tick has occurred, potentially triggering a server tick.
         * @return true is a scheduler tick is triggered as a result.
         */
        public bool serverTick() { // Should only be done on the main thread
            if (IsRunning) {
                SyncedTick++;

                // Check if synced is a multiple of the delay
                if ((SyncedTick % TickDelay) == 0) {
                    RunSchedulerTick();
                    return true;
                }
            }
            return false;
        }
    }
}