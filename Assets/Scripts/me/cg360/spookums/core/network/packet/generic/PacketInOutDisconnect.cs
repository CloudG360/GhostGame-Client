namespace me.cg360.spookums.core.network.packet.generic
{
// Proposed change:
// Have a format which only contains a disconnect "code"
// to make it optionally shorter. (But keep the full te

    /**
 * <h3>Format:</h3>
 * x byte(s) - UTF-8 String data (length = body size)
 */
    public class PacketInOutDisconnect : NetworkPacket
    {

        public const string DEFAULT_INBOUND_TEXT = "The client has disconnected. (No Reason Specified)";
        public const string DEFAULT_OUTBOUND_TEXT = "You have been disconnected from the host server.";

        public string Text { get; protected set; }
        
        public PacketInOutDisconnect() : this(null) { }
        public PacketInOutDisconnect(string text)
        {
            Text = text;
        }
        
        protected override byte GetPacketTypeID()
        {
            return VanillaProtocol.PACKET_DISCONNECT_REASON;
        }

        protected override ushort EncodeBody()
        {
            string selectedText = Text ?? DEFAULT_OUTBOUND_TEXT;

            Body.Reset();
            int strSize = Body.PutUnboundUTF8String(selectedText);

            return (ushort) strSize;
        }

        protected override void DecodeBody(ushort inboundSize)
        {

            if (inboundSize == 0)
            {
                Text = DEFAULT_INBOUND_TEXT;
            }
            else
            {
                Text = Body.GetUnboundUTF8String(inboundSize);
            }

            Body.Reset();
        }

        public override string ToString()
        {
            return "Content: {" +
                   "text='" + Text + "'" +
                   "}";
        }
    }
}
