using System.Collections.Generic;

namespace me.cg360.spookums.core.eventsys.handler
{
    public enum Priority
    {
        LOWEST, // Event is ran FIRST
        LOWER,
        LOW,
        NORMAL,
        HIGH,
        HIGHER,
        HIGHEST // Event is ran LAST, thus the ID should put it at the end of the line.
    }
}