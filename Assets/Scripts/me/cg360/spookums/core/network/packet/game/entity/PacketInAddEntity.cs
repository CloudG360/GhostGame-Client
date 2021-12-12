using System;
using me.cg360.spookums.core.network.packet.type;
using me.cg360.spookums.utility;
using UnityEngine;

namespace me.cg360.spookums.core.network.packet.game.entity
{
    public class PacketInAddEntity : PacketInOutEntity
    {

        // inherited: entity runtime id
        public String EntityTypeId { get; set; }
        public Vector2 Position { get; set; }
        public byte FloorNumber { get; set; }

        public String PropertiesJSON { get; set; }

        public PacketInAddEntity() {
            EntityRuntimeID = 1;
            EntityTypeId = null;
            Position = Vector2.zero;
            FloorNumber = Byte.MaxValue;

            PropertiesJSON = "{}"; // providing a default as this isn't essential
        }

        
        protected override byte GetPacketTypeID()
        {
            return VanillaProtocol.PACKET_ENTITY_ADD;
        }


        protected override ushort EncodeBody() {
            ushort total = base.EncodeBody();

            total += Body.PutSmallUTF8String(EntityTypeId);
            total += Body.PutVector2(Position);
            total += 1; Body.PutUnsignedByte(FloorNumber);
            total += Body.PutUnboundUTF8String(PropertiesJSON);

            return total;
        }

        
        protected override void DecodeBody(ushort inboundSize) {
            base.DecodeBody(inboundSize);

            // Check if type id length can be read
            if(Body.CanReadBytesAhead(1)) {
                EntityTypeId = Body.GetSmallUTF8String();

                if(Body.CanReadBytesAhead(NetworkBuffer.VECTOR2_BYTE_COUNT + 1)) {
                    Position = Body.GetVector2();
                    FloorNumber = Body.Get();

                    PropertiesJSON = Body.CountBytesRemaining() > 0
                        ? Body.GetUnboundUTF8String(Body.CountBytesRemaining())
                        : "{}";
                }
            }
        }
    }
}