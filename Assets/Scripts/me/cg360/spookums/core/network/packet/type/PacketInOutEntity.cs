using me.cg360.spookums.utility;

namespace me.cg360.spookums.core.network.packet.type
{
    public abstract class PacketInOutEntity : NetworkPacket
    {

        public uint EntityRuntimeID { get; set; }
        
        
        protected override ushort EncodeBody() {
            Body.Reset();
            Body.PutUnsignedInt(EntityRuntimeID);
            return 4;
        }
        
        protected override void DecodeBody(ushort inboundSize) {
            Body.Reset();
            if(Body.CanReadBytesAhead(NetworkBuffer.INT_BYTE_COUNT))
                EntityRuntimeID = Body.GetUnsignedInt();
        }
        
    }
}