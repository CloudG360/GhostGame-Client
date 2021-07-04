namespace me.cg360.spookums.core.network.packet.type
{
    public abstract class PacketInOutEmpty : NetworkPacket
    {

        protected sealed override ushort EncodeBody() { return 0; }
        protected sealed override void DecodeBody(ushort inboundSize) { }
    }
}