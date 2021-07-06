using System;

namespace me.cg360.spookums.core.network.packet.info
{
    public class PacketOutProtocolCheck : NetworkPacket
    {
        public ushort ProtocolVersion { get; protected set; }
        public bool IsValid { get; protected set; }

        
        public PacketOutProtocolCheck() : this(VanillaProtocol.PROTOCOL_ID) { }
        public PacketOutProtocolCheck(ushort protocolVersion)
        {
            ProtocolVersion = protocolVersion;
            IsValid = true;
        }

        
        protected override byte GetPacketTypeID()
        {
            return VanillaProtocol.PACKET_PROTOCOL_CHECK;
        }

        protected override ushort EncodeBody()
        {
            if (!IsValid) throw new InvalidOperationException("Protocol Check packet must include a valid version.");
            
            Body.Reset();
            Body.PutUnsignedShort(ProtocolVersion);
            return 2;
        }
        
        protected override void DecodeBody(ushort inboundSize)
        {
            throw new NotImplementedException();
        }
    }
}