using System;
using System.Collections.Generic;
using me.cg360.spookums.core.network.packet;

namespace net.cg360.spookums.core.network {

    // While it's not essential register a packet here, only packets
    // found here are processed when recieved by the server.
    public class PacketRegistry {

        private static PacketRegistry primaryInstance = null;

        protected Dictionary<byte, Type> packetTypes;

        public PacketRegistry() {
            this.packetTypes = new Dictionary<byte, Type>();
        }

        public bool SetAsPrimaryInstance() {
            if (primaryInstance == null) {
                primaryInstance = this;
                return true;
            }
            return false;
        }



        public bool GetPacketType(byte id, out Type packetType)
        {
            return this.packetTypes.TryGetValue(id, out packetType);
        }

        // Chaining
        public PacketRegistry r(byte id, Type type) {
            registerPacketType(id, type);
            return this;
        }

        public bool registerPacketType(byte id, Type type) {
            if (type == null) return false;
            if (type.IsAssignableFrom(typeof(NetworkPacket)))
            {

                if (!this.packetTypes.ContainsKey(id))
                {
                    this.packetTypes.Add(id, type);
                    return true;
                }
            }

            return false;
        }



        public static PacketRegistry Get() {
            return primaryInstance;
        }
    }
}
