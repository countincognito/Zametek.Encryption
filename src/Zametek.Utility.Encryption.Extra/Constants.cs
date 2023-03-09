namespace Zametek.Utility.Encryption
{
    public static class Constants
    {
        public const int c_MinIterations = 500_000;

        public const int c_128_Bit_ByteArrayLength = 128 / 8;
        public const int c_256_Bit_ByteArrayLength = 256 / 8;
        public const int c_512_Bit_ByteArrayLength = 512 / 8;
        public const int c_1024_Bit_ByteArrayLength = 1024 / 8;
        public const int c_2048_Bit_ByteArrayLength = 2048 / 8;
        public const int c_4096_Bit_ByteArrayLength = 4096 / 8;

        public const int c_32ByteLength = c_256_Bit_ByteArrayLength;
    }
}
