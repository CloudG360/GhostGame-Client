using me.cg360.spookums.utility;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace me.cg360.spookums.core.network.packet.auth
{
    // This packet contains a lot of optional peices of data. They are
    // ordered based on their apperence in the flags.
    public class PacketInLoginResponse : NetworkPacket
    {

        /**
         * == CODES:
         * 0 = Success/Invalid (Depending on if info follows)
         *
         * 1 = Provided Username does not exist.
         * 2 = Provided Password/Token is invalid/incorrect.
         * 3 = Too many attempts.
         */
        public byte StatusCode { get; set; }

        public string Username { get; set; }
        public string Token { get; set; }

        // Keep the IDs consistent with updates, skip index 0.
        protected MicroBoolean MissingFields { get; set; } // For creating an account



        protected override byte GetPacketTypeID()
        {
            return VanillaProtocol.PACKET_LOGIN_RESPONSE;
        }


        protected override ushort EncodeBody()
        {
            return 0;
        }

        protected override void DecodeBody(ushort inboundSize)
        {
            Body.Reset();

            if (Body.CanReadBytesAhead(1))
            {
                StatusCode = Body.Get();

                if (CodeToStatus(StatusCode) == Status.SUCCESS)
                {
                    Username = Body.CanReadBytesAhead(1) 
                        ? Body.GetSmallUTF8String() 
                        : "";
                    Token = Body.CanReadBytesAhead(1) 
                        ? Body.GetSmallUTF8String() 
                        : "";
                    MissingFields = MicroBoolean.Empty();
                }
                else
                {
                    Username = "";
                    Token = "";
                    MissingFields = Body.CanReadBytesAhead(1) 
                        ? MicroBoolean.From(Body.Get()) 
                        : MicroBoolean.Empty();
                }
            }
        }


        public static Status CodeToStatus(byte code)
        {
            switch (code) {
                case 0:
                    return Status.SUCCESS; // Success/Generic fail
                case 1:
                    return Status.FAILURE_GENERAL;

                case 2:
                    return Status.INVALID_CREDENTIALS;
                case 3:
                    return Status.INVALID_TOKEN;
                case 4:
                    return Status.TOO_MANY_ATTEMPTS;
                case 5:
                    return Status.ALREADY_LOGGED_IN;

                case 6:
                    return Status.MISSING_FIELDS;
                case 7:
                    return Status.TAKEN_USERNAME;
                case 8:
                    return Status.TECHNICAL_SERVER_ERROR;
                case 9:
                    return Status.GENERAL_REGISTER_ERROR;
                case 10:
                    return Status.GENERAL_LOGIN_ERROR;

                case 126:
                    return Status.INVALID_PACKET;
                default:
                    return Status.UNKNOWN;
            }
        }
        
        public enum Status {

            SUCCESS = 0,
            FAILURE_GENERAL = 1,

            // Login
            INVALID_CREDENTIALS = 2, // Applies to login, updating account, and creating account (if username is taken)
            INVALID_TOKEN = 3, // Password required for updating an account so n/a there
            TOO_MANY_ATTEMPTS = 4,
            ALREADY_LOGGED_IN = 5, // Updating an account refreshes the login so not sent.

            // Creating an account
            MISSING_FIELDS = 6,
            TAKEN_USERNAME = 7,
            TECHNICAL_SERVER_ERROR = 8,
            GENERAL_REGISTER_ERROR = 9,
            GENERAL_LOGIN_ERROR = 10,


            INVALID_PACKET = 126,
            UNKNOWN = 127
            
        }
    }
    
}