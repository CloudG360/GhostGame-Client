using System;

namespace me.cg360.spookums.core.network.packet.info
{
    public class PacketInServerDetail : NetworkPacket
    {

        public byte PingVersion { get; protected set; } // The version of the ServerDetail packet format | Independent of protocol version.

        public string ServerName { get; protected set; }
        public string ServerRegion { get; protected set; } // 5 characters max
        public string ServerDescription { get; protected set; }


        protected override byte GetPacketTypeID()
        {
            return VanillaProtocol.PACKET_SERVER_DETAIL;
        }

        protected override ushort EncodeBody()
        {
            throw new NotImplementedException();
        }

        protected override void DecodeBody(ushort inboundSize)
        {
            // Set this by default in case of an error at the version byte.
            // This allows the same PingVersion to be provided if the rest of the
            // packet is malformed.
            PingVersion = 255;

            try
            {
                PingVersion = Body.Get();
                // Backwards compatability. Support multiple server info types.
                switch (PingVersion)
                {
                    case 1:
                        ServerName = Body.GetSmallUTF8String();
                        ServerRegion = Body.GetSmallUTF8String();
                        ServerDescription = Body.GetUTF8String();
                        break;

                    default:
                        ServerName = "Unidentified Server";
                        ServerRegion = "?";
                        ServerDescription =
                            "The details provided by this server are either too new or unsupported by your current version. Try updated or contacting the server administator.";
                        break;
                }
            }
            catch
            {
                ServerName = "Invalid Server";
                ServerRegion = "?";
                ServerDescription = "Server sent an empty/malformed response. Looks broken to me chief.";
            }
        }
    }
}