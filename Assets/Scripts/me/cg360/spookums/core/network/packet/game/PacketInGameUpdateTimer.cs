using System;
using me.cg360.spookums.utility;

namespace me.cg360.spookums.core.network.packet.game
{
    public class PacketInGameUpdateTimer : NetworkPacket
    {
        public uint TimerTicks { get; set; }

        public PacketInGameUpdateTimer() {
            TimerTicks = 0;
        }
        
        protected override byte GetPacketTypeID() {
            return VanillaProtocol.PACKET_TIMER_UPDATE;
        }
        
        protected override ushort EncodeBody() {
            return Body.PutUnsignedInt(TimerTicks);
        }
        
        protected override void DecodeBody(ushort inboundSize) {
            if (Body.CanReadBytesAhead(NetworkBuffer.INT_BYTE_COUNT))
            {
                TimerTicks = Body.GetUnsignedInt();
            }
        }
        
    }
}