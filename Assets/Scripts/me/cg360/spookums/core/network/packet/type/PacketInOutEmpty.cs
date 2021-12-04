namespace me.cg360.spookums.core.network.packet.type
{
    public abstract class PacketInOutEmpty : NetworkPacket
    {

        protected sealed override ushort EncodeBody()
        {
            // Hacky fix to get empty packets to send
            // I don't have the time to fix it right now but this defo needs fixing!
            Body.PutUnsignedByte(0x00);
            return 1;
        }
        protected sealed override void DecodeBody(ushort inboundSize) { }
    }
}