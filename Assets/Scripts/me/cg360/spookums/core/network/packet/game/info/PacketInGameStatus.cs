namespace me.cg360.spookums.core.network.packet.game.info
{
    public class PacketInGameStatus : NetworkPacket
    {
        public byte Type;
        public string GameID; // for if the type == 2 or 3
        public string Reason; // if rejected, this is filled.

        public PacketInGameStatus(){
            Type = 0;
            GameID = "";
            Reason = "";
        }
        
        protected override byte GetPacketTypeID() {
            return VanillaProtocol.PACKET_GAME_STATUS;
        }
        
        protected override ushort EncodeBody() {
            Body.Reset();

            ushort total = 0;
            total += (ushort) (Body.PutUnsignedByte(Type) ? 1 : 0);
            total += Body.PutSmallUTF8String(GameID);
            total += Body.PutUTF8String(Reason);
            return total;
        }
        
        protected override void DecodeBody(ushort inboundSize) {

            if (Body.CanReadBytesAhead(1))
            {
                Type = Body.Get();

                if (Body.CanReadBytesAhead(1))
                {
                    GameID = Body.GetSmallUTF8String();

                    if (Body.CanReadBytesAhead(2))
                    {
                        Reason = Body.GetUTF8String();
                    }
                }
            }
        }

        public StatusType GetStatusType()
        {
            return GetTypeFromID(Type);
        }

        public enum StatusType {
            QUEUE_JOINED, QUEUE_REJECTED,
            GAME_JOIN, GAME_REJECTED, GAME_JOIN_AS_SPECTATOR, GAME_DISCONNECT,
            GAME_COUNTDOWN, GAME_STARTED, GAME_CONCLUDED,
            
            UNKNOWN
        }
    
    
        public static StatusType GetTypeFromID(int i) {
            switch (i) {
                case 1: return StatusType.QUEUE_JOINED;
                case 2: return StatusType.QUEUE_REJECTED;
                case 3: return StatusType.GAME_JOIN;
                case 4: return StatusType.GAME_REJECTED;
                case 5: return StatusType.GAME_JOIN_AS_SPECTATOR;
                case 6: return StatusType.GAME_DISCONNECT;
                case 7: return StatusType.GAME_COUNTDOWN;
                case 8: return StatusType.GAME_STARTED;
                case 9: return StatusType.GAME_CONCLUDED;

                case 127:
                default: return StatusType.UNKNOWN;
            }
        }
    }
}