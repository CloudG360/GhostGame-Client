using System.Threading;
using me.cg360.spookums.utility;

namespace me.cg360.spookums.core.scheduler.task
{
    public class RunnableTypeTask : SchedulerTask
    {
        private readonly ThreadStart taskRunnable;

        public RunnableTypeTask(ThreadStart taskRunnable) {
            Check.NullParam(taskRunnable, "taskRunnable");
            this.taskRunnable = taskRunnable;
        }
        
        public override void run() {
            taskRunnable.Invoke();
        }
    }
}