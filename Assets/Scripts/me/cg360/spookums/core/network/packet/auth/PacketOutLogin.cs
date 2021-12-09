using me.cg360.spookums.utility;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace me.cg360.spookums.core.network.packet.auth
{
    public class PacketOutLogin : NetworkPacket
    {

        public byte Mode { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }

        public PacketOutLogin()
        {
            Mode = 0;
            Username = null;
            Password = null;
            Token = null;
        }

        protected override byte GetPacketTypeID()
        {
            return VanillaProtocol.PACKET_LOGIN;
        }

        protected override ushort EncodeBody() {
            ushort total = 1;
            Body.PutUnsignedByte(Mode);

            switch(Mode)
            {
                case 0:
                    total += Body.PutSmallUTF8String(Username);
                    total += Body.PutSmallUTF8String(Password);
                    break;

                case 1:
                    total += Body.PutSmallUTF8String(Token);
                    break;

                default:
                    break; // ?? How is it here if it gets here.
            }

            return total; 
        }

        protected override void DecodeBody(ushort inboundSize) { }


        public bool IsValid()
        {
            return ((Username != null) && (Password != null)) || (Token != null);
        }
    }
}