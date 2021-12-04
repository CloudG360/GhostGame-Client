using me.cg360.spookums.core.eventsys.type;
using me.cg360.spookums.utility;
using System;
using me.cg360.spookums.core.network.netimpl;
using me.cg360.spookums.core.network.packet;

namespace me.cg360.spookums.core.eventsys.type.network
{
    public class ConnectionEstablishedEvent : BaseEvent
    {

        public readonly string Hostname;
        public readonly int Port;
        public readonly NetworkInterface Net;

        public ConnectionEstablishedEvent(string hostname, int port, NetworkInterface net)
        {
            this.Hostname = hostname;
            this.Port = port;
            this.Net = net;
        }

    }
}