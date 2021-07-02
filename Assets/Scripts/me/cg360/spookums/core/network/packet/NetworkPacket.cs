using System;
using me.cg360.spookums.utility;
using UnityEngine;

namespace me.cg360.spookums.core.network.packet {

    public abstract class NetworkPacket {

        public NetworkBuffer Body { get; }
        public byte PacketID { get; private set; }
        protected ushort BodySize { get; private set; }

        public NetworkPacket() {
            this.Body = NetworkBuffer.Wrap(new byte[VanillaProtocol.MAX_PACKET_SIZE - 3]); // 3 bytes are reserved for meta.
            this.PacketID = GetPacketTypeID();
            this.BodySize = 0;
        }

        protected abstract byte GetPacketTypeID();
        protected abstract ushort EncodeBody(); // Takes data and puts it into the body buffer. returns: body size
        protected abstract void DecodeBody(ushort inboundSize); // Takes data from the body buffer and converts it to fields.

        public NetworkBuffer Encode(out int fullPacketSize) {
            NetworkBuffer data = NetworkBuffer.Wrap(new byte[VanillaProtocol.MAX_PACKET_SIZE]);

            BodySize = EncodeBody();
            ushort size = (ushort) (BodySize + 1); // packet id + body
            
            data.PutUnsignedShort(size);
            data.Put(PacketID);
            
            fullPacketSize = size + 2; // length short + content size
            Body.Reset(); // Go to the start of the body.

            for (short i = 0; i < size - 1; i++) {
                data.Put(Body.Get()); // Copy bytes from body up to the size
            }

            return data;
        }

        public NetworkPacket Decode(NetworkBuffer fullPacket) {
            fullPacket.Reset(); // Ensure buffers are ready for reading.
            Body.Reset();

            BodySize = fullPacket.GetUnsignedShort();
            PacketID = fullPacket.Get();

            for (int i = 0; i < BodySize - 1; i++) {
                Body.Put(fullPacket.Get()); // Copy bytes
            }

            Body.Reset();
            DecodeBody(BodySize);
            return this;
        }

        public string ToHeaderString() {
            return "(" +
                    "ID=0x" + BitConverter.ToString(new []{Convert.ToByte(PacketID)} ) +
                    "| size=" + (BodySize + 1) +
                    ")";
        }
    }
}
