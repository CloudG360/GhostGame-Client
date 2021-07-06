using me.cg360.spookums.core.network.packet.type;

namespace me.cg360.spookums.core.network.packet.info
{
    public class PacketInProtocolSuccess : PacketInOutEmpty
    {
        protected override byte GetPacketTypeID()
        {
            return VanillaProtocol.PACKET_PROTOCOL_SUCCESS;
        }
    }
}