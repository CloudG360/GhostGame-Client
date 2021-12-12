namespace me.cg360.spookums.core.network.packet.info
{
    public class PacketInProtocolError : NetworkPacket
    {

        public ushort RequiredProtocolVersion { get; protected set; }
        public string RequiredClientVersionInfo { get; protected set; }

        protected override byte GetPacketTypeID()
        {
            return VanillaProtocol.PACKET_PROTOCOL_ERROR;
        }

        protected override ushort EncodeBody()
        {
            throw new System.NotImplementedException();
        }

        protected override void DecodeBody(ushort inboundSize)
        {
            Body.Reset();
            if (Body.CanReadBytesAhead(2))
            {
                RequiredProtocolVersion = Body.GetUnsignedShort();
                RequiredClientVersionInfo = Body.GetUTF8String();
            }
        }
    }
}