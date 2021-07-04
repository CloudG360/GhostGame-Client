using System;
using System.Text;
using UnityEngine;

namespace me.cg360.spookums.utility
{
    public class NetworkBuffer
    {
        
        public byte[] Buffer { get; private set; }
        public int PointerIndex { get; private set; }


        protected NetworkBuffer(byte[] buffer)
        {
            Buffer = buffer;
            PointerIndex = 0;
        }
        
        public static NetworkBuffer Wrap(byte[] buffer)
        {
            return new NetworkBuffer(buffer);
        }
        
        public static NetworkBuffer Allocate(int size)
        {
            return new NetworkBuffer(new byte[size]);
        }



        /** Counts the amount of bytes between the pointer (inclusive) and the end of the buffer. */
        public int CountBytesRemaining()
        {
            return Buffer.Length - PointerIndex;
        }

        /** Checks if the buffer has a provided number of bytes left before the end. */
        public bool CanReadBytesAhead(int bytesAhead)
        {
            return CountBytesRemaining() >= bytesAhead;
        }



        /** Sets pointer index to 0. */
        public void Reset()
        {
            PointerIndex = 0;
        }

        /** Moves the pointer forward a set number of positions. */
        public void Skip(int delta = 1)
        {
            PointerIndex = Math.Min(Buffer.Length - 1, PointerIndex + delta);
        }

        /** Rewinds the pointer a set number of positions. */
        public void Rewind(int delta = 1)
        {
            PointerIndex = Math.Max(0, PointerIndex - delta);
        }
        

        
        /** Unsafe way to fetch a byte. Make sure to check first :)*/
        protected byte FetchRawByte() {
            byte b = Buffer[PointerIndex];
            PointerIndex++;
            return b;
        }

        protected byte[] FetchRawBytes(int byteCount) {
            byte[] bytes = new byte[byteCount];

            for(int i = 0; i < byteCount; i++) {
                bytes[i] = Buffer[PointerIndex];
                PointerIndex++;
            }
            return bytes;
        }

        /** Unsafe way to write a byte. Make sure to check first :)*/
        protected void WriteByte(byte b) {
            Buffer[PointerIndex] = b;
            PointerIndex++;
        }

        /** Unsafe way to write a series of bytes. Make sure to check first :)*/
        protected void WriteBytes(byte[] bytes) {
            foreach(byte b in bytes) WriteByte(b);
        }

        // Could be signed, unsigned, or even a string character.
        /** Fetches a byte from the buffer without converting it.*/
        public byte Get() {
            if(CanReadBytesAhead(1)) {
                return FetchRawByte();
            }
            throw new Exception("NetworkBuffer Underflow");
        }

        /** Fetches a quantity of bytes from the buffer without converting them.*/
        public void Get(byte[] target) {
            if(CanReadBytesAhead(target.Length)) {
                for(int i = 0; i < target.Length; i++) target[i] = FetchRawByte();
                return;
            }
            throw new Exception("NetworkBuffer Underflow");
        }

        public ushort GetUnsignedShort() {
            if(CanReadBytesAhead(2)) {
                int total = 0;
                total += ((((int)FetchRawByte()) << 8) & 0xFF00);
                total += (FetchRawByte() & 0x00FF);

                return (ushort) (total & 0xFFFF);
            }
            throw new Exception("NetworkBuffer Underflow");
        }

        public String GetUnboundUTF8String(int byteCount) {
            if(CanReadBytesAhead(byteCount)) {
               byte[] strBytes = FetchRawBytes(byteCount);
               return Encoding.UTF8.GetString(strBytes);
            }
            throw new Exception("NetworkBuffer Underflow");
        }




        /** Sets bytes to the buffer without converting it.*/
        public bool Put(params byte[] b) {
            if(CanReadBytesAhead(b.Length)) {
                foreach (byte value in b) WriteByte(value);
                return true;
            }
            return false;
        }

        public bool PutUnsignedByte(byte value) {
            if(CanReadBytesAhead(1)) {
                WriteByte(value);
                return true;
            }
            return false;
        }

        public bool PutUnsignedShort(ushort value) {
            if(value >= (int) Math.Pow(2, 16)) throw new Exception("Illegal Argument: Provided an 'unsigned short' with a value greater than 2^16");

            if(CanReadBytesAhead(2)) {
                byte bUpper = (byte) ((value & 0xFF00) >> 8);
                byte bLower = (byte) (value & 0x00FF);

                WriteByte(bUpper);
                WriteByte(bLower);
                return true;
            }

            return false;
        }

        /** @return the amount of bytes written. */
        public int PutUTF8String(string str) {
            if(str.Length == 0) return 0;

            byte[] strBytes = Encoding.UTF8.GetBytes(str);
            if(strBytes.Length <= ushort.MaxValue) throw new Exception("Illegal Argument: String exceeds the limit of "+ushort.MaxValue+" bytes.");

            // Check if both the length of bytes + the length short can be included.
            if(CanReadBytesAhead(2 + strBytes.Length)) {
                if(PutUnsignedShort((ushort) strBytes.Length)) {
                    WriteBytes(strBytes);
                    return 2 + strBytes.Length;
                }
            }

            return 0;
        }

        /** @return the amount of bytes written. */
        public int PutSmallUTF8String(String str) {
            if(str.Length == 0) return 0;

            byte[] strBytes = Encoding.UTF8.GetBytes(str);
            if(strBytes.Length <= byte.MaxValue) throw new Exception("String exceeds the limit of "+byte.MaxValue+" bytes.");

            // Check if both the length of bytes + the length short can be included.
            if(CanReadBytesAhead(1 + strBytes.Length)) {
                if(PutUnsignedByte((byte) strBytes.Length)) {
                    WriteBytes(strBytes);
                    return 1 + strBytes.Length;
                }
            }
            return 0;
        }

        /** A string is added without length marking bytes at the start. */
        public int PutUnboundUTF8String(String str) {
            if(str.Length == 0) return 0;
            byte[] strBytes = Encoding.UTF8.GetBytes(str);

            // Check if both the length of bytes + the length short can be included.
            if(CanReadBytesAhead(strBytes.Length)) {
                WriteBytes(strBytes);
                return strBytes.Length;
            }
            return 0;
        }


        public override string ToString()
        {
            return "{" +
                "index: " + PointerIndex.ToString() + ", " +
                "content: " + BitConverter.ToString(Buffer) +
                "}";
        }

    }
}
