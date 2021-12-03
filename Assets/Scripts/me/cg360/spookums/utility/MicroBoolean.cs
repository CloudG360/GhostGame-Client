
namespace me.cg360.spookums.utility
{
    public class MicroBoolean
    {
        private byte values;

        public MicroBoolean(params bool[] bools)
        {
            Check.InclusiveBounds(bools.Length, 0, 7, "bools.length");
            this.values = 0x00;

            for (int i = 0; (i < 8) && (i < bools.Length); i++)
            {
                if (bools[i])
                {
                    this.values |= (byte) ((0b00000001 << i) & 0xFF);
                }
            }
        }

        private MicroBoolean(byte source)
        {
            this.values = source;
        }


        /**
         * Sets a value within a boolean
         * @param index the bit in the storage byte to modify (0 to 7 inclusive)
         * @param value the value of the boolean
         */
        public MicroBoolean SetValue(int index, bool value)
        {
            Check.InclusiveBounds(index, 0, 7, "index");

            // Sets the bits differently based on if they're true or false.
            // If true, use an OR bitwise operator to set the position to 1.
            // If false, use an AND bitwise operator + the compliment of the mask to set
            // the position to 0.
            if (value) this.values |= (byte) ((0b00000001 << index) & 0xFF);
            else this.values &= (byte) ((~(0b00000001 << index)) & 0xFF);

            return this;
        }

        /**
     * @param index the bit in the storage byte to fetch (0 to 7 inclusive)
     * @return the value at the index
     */
        public bool GetValue(int index)
        {
            Check.InclusiveBounds(index, 0, 7, "index");
            byte mask = (byte)((0b00000001 << index) & 0xFF);

            return (values & mask) != 0x00; // Return if bit under mask is NOT 0, meaning the bool is false.
        }

        public byte GetStorageByte()
        {
            return values;
        }

        public bool IsEmpty()
        {
            return this.values == 0x00;
        }


        // Below are a few static methods that make creation look a bit nicer :D

        public static MicroBoolean Empty()
        {
            return new MicroBoolean(0x00);
        }

        public static MicroBoolean From(byte source)
        {
            return new MicroBoolean(source);
        }

        public static MicroBoolean Of(params bool[] bools)
        {
            return new MicroBoolean(bools);
        }
    }
}