using System;

namespace me.cg360.spookums.core.eventsys.handler
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GEventHandler : Attribute
    {

        public Priority Priority = Priority.NORMAL;
        public bool IgnoreIfCancelled = false;

    }
}