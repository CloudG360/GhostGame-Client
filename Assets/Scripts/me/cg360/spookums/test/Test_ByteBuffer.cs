using me.cg360.spookums.utility;
using System;
using UnityEngine;

namespace me.cg360.spookums.test
{
    public class Test_ByteBuffer : MonoBehaviour
    {

        // Code:
        // 0 = Succesful
        // 1 = Array does not start/end right
        public int TestReadBufferX(int iterations)
        {
            NetworkBuffer buffer = NetworkBuffer.Wrap(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F });
            byte[] buf = new byte[16 * iterations];

            for (int i = 0; i < iterations; i++) {
                buffer.Reset();
                while (buffer.CanReadBytesAhead(1))
                {
                    buf[buffer.PointerIndex + (16 * i)] = buffer.Get();
                }
            }

            Debug.Log(buffer.ToString());
            Debug.Log(BitConverter.ToString(buf));
            
            return buf[0] == 0x00 && buf[(16 * iterations) - 1] == 0x0F ? 0 : 1;
        }


        private void Start()
        {
            int resultBufferReadX2 = TestReadBufferX(2);
            Debug.Log("Test: ReadBufferX2  \nResult: "+resultBufferReadX2);
        }

    }
}
