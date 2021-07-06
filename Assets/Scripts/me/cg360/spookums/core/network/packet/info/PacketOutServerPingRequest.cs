using me.cg360.spookums.core.network.packet.type;

namespace me.cg360.spookums.core.network.packet.info
{
    public class PacketOutServerPingRequest : PacketInOutEmpty
    {
        protected override byte GetPacketTypeID()
        {
            return VanillaProtocol.PACKET_SERVER_PING_REQUEST;
        }
    }
}