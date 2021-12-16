using me.cg360.spookums.core.network;
using me.cg360.spookums.core.network.packet.type;
using me.cg360.spookums.utility;
using UnityEngine;

namespace net.cg360.spookums.server.network.packet.game.entity {

    public class PacketInOutEntityMove : PacketInOutEntity {

        public byte Type { get; set; }
        public Vector2 Movement { get; set; }

        public PacketInOutEntityMove() {
            this.Type = 0;
            this.Movement = Vector2.zero;
        }


        protected override byte GetPacketTypeID() {
            return VanillaProtocol.PACKET_ENTITY_MOVE;
        }


        protected override ushort EncodeBody() {
            ushort total = base.EncodeBody();
            total += (ushort) (Body.PutUnsignedByte(Type) ? 1 : 0);
            total += Body.PutVector2(Movement);

            return total;
        }

        protected override void DecodeBody(ushort inboundSize) {
            base.DecodeBody(inboundSize);

            if(Body.CanReadBytesAhead(1)) {
                Type = Body.Get();

                if(Body.CanReadBytesAhead(NetworkBuffer.VECTOR2_BYTE_COUNT)) {
                    Movement = Body.GetVector2();
                }
            }
        }

        public enum MovementType {
            DELTA,
            ABSOLUTE,
            UNKNOWN
        }


        public static MovementType getTypeFromID(byte id)
        {
            switch (id)
            {
                case 0:
                    return MovementType.DELTA;

                case 1:
                    return MovementType.ABSOLUTE;

                default:
                    return MovementType.UNKNOWN;
            }
        }
    }
}
