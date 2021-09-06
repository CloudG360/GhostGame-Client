using System;
using System.Collections.Generic;
using me.cg360.spookums.core.network.packet;
using UnityEngine;

namespace net.cg360.spookums.core.network {

    // While it's not essential register a packet here, only packets
    // found here are processed when recieved by the server.
    public class PacketRegistry {

        private static PacketRegistry _primaryInstance = null;

        protected Dictionary<byte, Type> PacketTypes;

        public PacketRegistry() {
            PacketTypes = new Dictionary<byte, Type>();
        }

        public void SetAsPrimaryInstance() 
        {
            _primaryInstance = this;
        }



        public bool GetPacketType(byte id, out Type packetType)
        {
            if (PacketTypes.ContainsKey(id))
            {
                packetType = PacketTypes[id];
                return packetType != null;
            }

            packetType = null;
            return false;
        }

        // Chaining
        public PacketRegistry R(byte id, Type type) {
            RegisterPacketType(id, type);
            return this;
        }

        public bool RegisterPacketType(byte id, Type type) {
            if (type == null) return false;
            if (typeof(NetworkPacket).IsAssignableFrom(type))
            {

                if (!PacketTypes.ContainsKey(id))
                {
                    PacketTypes.Add(id, type);
                    return true;
                }
            }

            return false;
        }



        public static PacketRegistry Get() {
            return _primaryInstance;
        }
    }
}
