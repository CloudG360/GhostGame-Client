using me.cg360.spookums.core.network.packet.type;

namespace me.cg360.spookums.core.network.packet.game.info
{
    public class PacketOutGameQueueRequest : PacketInOutEmpty
    {
        protected override byte GetPacketTypeID()
        {
            return VanillaProtocol.PACKET_GAME_SEARCH_REQUEST;
        }
    }
}