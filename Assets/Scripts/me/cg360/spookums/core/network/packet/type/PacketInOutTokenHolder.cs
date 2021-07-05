using System;

namespace me.cg360.spookums.core.network.packet.type
{
    public abstract class PacketInOutTokenHolder : NetworkPacket
    {
        public string Token { get; set; }

        public PacketInOutTokenHolder() : this(null) { }

        public PacketInOutTokenHolder(String token)
        {
            Token = token;
        }
        

        protected override ushort EncodeBody()
        {
            String finalToken = string.IsNullOrEmpty(Token) ? "empty" : Token;
            Body.Reset();
            
            return Body.PutSmallUTF8String(finalToken);
        }

        protected override void DecodeBody(ushort inboundSize)
        {
            Body.Reset();
            Token = Body.GetSmallUTF8String();
        }
    }
}