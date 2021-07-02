using me.cg360.spookums.core.eventsys.type;
using me.cg360.spookums.utility;
using System;
using me.cg360.spookums.core.network.packet;

namespace me.cg360.spookums.core.eventsys.type.network
{
    public class PacketEvent : CancellableEvent 
    {
        public NetworkPacket Packet { get; protected set; }

        protected PacketEvent(NetworkPacket packet)
        {
            Check.NullParam(packet, "packet");
            this.Packet = packet;
        }



        public class Recieved : PacketEvent
        {
            public Recieved(NetworkPacket packet) : base(packet) { } 
        }



        public class Sent : PacketEvent
        {

            public Sent(NetworkPacket packet) : base(packet) { }

        }

    }
}