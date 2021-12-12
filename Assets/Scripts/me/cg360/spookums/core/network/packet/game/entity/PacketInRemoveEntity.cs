using System;
using me.cg360.spookums.core.network.packet.type;

namespace me.cg360.spookums.core.network.packet.game.entity
{
    public class PacketInRemoveEntity : PacketInOutEntity
    {

        public PacketInRemoveEntity()
        {
            EntityRuntimeID = UInt32.MaxValue;
        }
        
        protected override byte GetPacketTypeID()
        {
            return VanillaProtocol.PACKET_ENTITY_REMOVE;
        }
        
    }
}