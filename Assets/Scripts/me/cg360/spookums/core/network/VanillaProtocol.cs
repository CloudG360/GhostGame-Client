using me.cg360.spookums.core.network.packet.generic;
using me.cg360.spookums.core.network.packet.info;
using net.cg360.spookums.core.network;
using UnityEngine;

namespace me.cg360.spookums.core.network
{
    public class VanillaProtocol
    {

        /* -- General Notes: --
         *
         * Format:
         * 1 byte - packet type
         * 2 bytes (short) - packet body size
         * ... bytes - body
         *
         * Types:
         *  - String:
         *      Stores a string type encoded in UTF-8
         *      - (Tracked) Includes a short type "length" prior to the text, indicating the amount of bytes taken to store it.
         *      - (Simple) Doesn't include a length prior to the string. Only used when the size can be inferred from the packet body length.
         *
         */

        public const ushort PROTOCOL_ID = 1;

        public const int MAX_BUFFER_SIZE = 32768;
        public const int MAX_PACKET_SIZE = 4096;
        public const int TIMEOUT = 15000;


        // -- Packet Identifiers --

        // Protocol packets - These should not change in format, even after a large update for consistency.
        public const byte PACKET_PROTOCOL_INVALID_PACKET = 0x00; // in/out - This should not be used at all! Packets with this ID are ignored silently.
        public const byte PACKET_PROTOCOL_CHECK = 0x01; // in - Can be appened with new data. Includes vital protocol info. Nothing should be removed/reordered to accommodate for older clients.
        public const byte PACKET_PROTOCOL_SUCCESS = 0x02; // out - confirms the client is compatible.
        public const byte PACKET_PROTOCOL_ERROR = 0x03; // out - rejects the client for using an incompatible protocol. Returns the protocol version and the supported client version
        public const byte PACKET_PROTOCOL_BATCH = 0x04; // Unused currently. Probably a good idea though.


        // Information Packets
        public const byte PACKET_SERVER_PING_REQUEST = 0x10; // in - responded to with PACKET_SERVER_DETAIL | Accepted by the server even if a protocol check hasn't occurred.
        public const byte PACKET_SERVER_DETAIL = 0x11; // out - JSON format - Like a ping, should only be extended. Includes stuff like name + logo if present.
        public const byte PACKET_CLIENT_DETAIL = 0x12; // in - JSON format - Stores client OS, version, and other non-essential details. Could be use to split platforms
        public const byte PACKET_SERVER_NOTICE = 0x13; // out - Used to display generic information to a user
        public const byte PACKET_DISCONNECT_REASON = 0x14; // in/out - Sent by the server/client that's closing the connection.


        // Response/Generic Packets
        public const byte PACKET_RESPONSE_WARNING = 0x15; // out - Used to respond to client packets with a warn status
        public const byte PACKET_RESPONSE_SUCCESS = 0x16; // out - Used to respond to client packets with a info status
        public const byte PACKET_RESPONSE_ERROR = 0x17; // out - Used to respond to client packets with a error status

        public const byte PACKET_CHAT_MESSAGE = 0x18; // in/out - messages in may get some further formatting.


        // Account Management Packets
        public const byte PACKET_LOGIN = 0x20; // in - User attempts to login to their account
        public const byte PACKET_CREATE_ACCOUNT = 0x22; // in - User attempts to create an account (Returns a login response packet)
        public const byte PACKET_LOGIN_RESPONSE = 0x24; // out - Could split into token (success) packet + error (failure) packet.


        // Session Stuff
        public const byte PACKET_GAME_JOIN_REQUEST = 0x30; // in - Client's intent to join a game. Will return a PACKET_SESSION_RESPONSE type packet
        public const byte PACKET_GAME_CREATE_REQUEST = 0x31; // in - Client's intent to create their own game with it's settings included.
        public const byte PACKET_GAME_RESPONSE = 0x32; // out - Returns a specific game token or an error message

        public const byte PACKET_FETCH_GAME_LIST = 0x33; // in - Requests a list of games (Responded to with a few PACKET_GAME_DETAIL's)
        public const byte PACKET_REQUEST_GAME_DETAIL = 0x34; // in - Requests the details of a specific game
        public const byte PACKET_GAME_DETAIL = 0x35; // out - Sends details of the game to the client


        public static PacketRegistry CreateRegistry()
        {
            return new PacketRegistry()
                    .R(PACKET_PROTOCOL_INVALID_PACKET, null)
                    .R(PACKET_PROTOCOL_CHECK, typeof(PacketOutProtocolCheck))
                    .R(PACKET_PROTOCOL_SUCCESS, typeof(PacketInProtocolSuccess))
                    .R(PACKET_PROTOCOL_ERROR, typeof(PacketInProtocolError))
                    .R(PACKET_PROTOCOL_BATCH, null)
                     
                    .R(PACKET_SERVER_PING_REQUEST, typeof(PacketOutServerPingRequest))
                    .R(PACKET_SERVER_DETAIL, typeof(PacketInServerDetail))
                    .R(PACKET_CLIENT_DETAIL, null)
                    .R(PACKET_SERVER_NOTICE, typeof(PacketInServerNotice))
                    .R(PACKET_DISCONNECT_REASON, typeof(PacketInOutDisconnect))
                     
                    .R(PACKET_RESPONSE_WARNING, null)
                    .R(PACKET_RESPONSE_SUCCESS, null)
                    .R(PACKET_RESPONSE_ERROR, null)
                    .R(PACKET_CHAT_MESSAGE, typeof(PacketInOutChatMessage))
                     
                    .R(PACKET_LOGIN, null)
                    .R(PACKET_CREATE_ACCOUNT, null)
                    .R(PACKET_LOGIN_RESPONSE, null)
                            
                    .R(PACKET_GAME_JOIN_REQUEST, null)
                    .R(PACKET_GAME_CREATE_REQUEST, null)
                    .R(PACKET_GAME_RESPONSE, null)
                    .R(PACKET_FETCH_GAME_LIST, null)
                    .R(PACKET_REQUEST_GAME_DETAIL, null)
                    .R(PACKET_GAME_DETAIL, null);
        }

    }
}
