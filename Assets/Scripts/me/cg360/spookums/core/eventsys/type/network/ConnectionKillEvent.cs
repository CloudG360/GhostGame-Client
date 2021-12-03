using me.cg360.spookums.core.eventsys.type;
using me.cg360.spookums.utility;
using System;
using me.cg360.spookums.core.network.packet;

namespace me.cg360.spookums.core.eventsys.type.network
{
    public class ConnectionKillEvent : CancellableEvent 
    {

        public readonly int ExitCode;
        public readonly string Reason;

        public ConnectionKillEvent(int exitCode, string reason)
        {
            this.ExitCode = exitCode;
            this.Reason = reason;
        }

    }
}