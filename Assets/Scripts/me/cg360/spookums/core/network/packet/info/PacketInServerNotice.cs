namespace me.cg360.spookums.core.network.packet.info
{
    public class PacketInServerNotice : NetworkPacket
    {
        
        /*
         * key:
         * 0 = info box
         * 1 = menu banner
         * 2 = notification
         * 3 = hidden
         */
        public byte MessageType { get; protected set; }
        public string Message { get; protected set; }
        
        protected override byte GetPacketTypeID()
        {
            return VanillaProtocol.PACKET_SERVER_NOTICE;
        }

        protected override ushort EncodeBody()
        {
            throw new System.NotImplementedException();
        }

        protected override void DecodeBody(ushort inboundSize)
        {
            if (Body.CanReadBytesAhead(2)) // At least 2 bytes (1 for type, 1+ message bytes)
            {
                MessageType = Body.Get();
                Message = Body.GetUTF8String();
            }
            else
            {
                MessageType = 0;
                Message = "...";
            }
        }
    }
}