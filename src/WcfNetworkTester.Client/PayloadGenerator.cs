using System;

namespace WcfNetworkTester.Client
{
    /// <summary>Generates deterministic, compressible payloads for testing.</summary>
    internal static class PayloadGenerator
    {
        private static readonly byte[] Pattern = BuildPattern();

        public static byte[] Generate(PayloadSize size) => Generate((int)size);

        public static byte[] Generate(int bytes)
        {
            if (bytes <= 0)
                return Array.Empty<byte>();

            byte[] buffer = new byte[bytes];
            for (int i = 0; i < bytes; i++)
                buffer[i] = Pattern[i % Pattern.Length];

            return buffer;
        }

        private static byte[] BuildPattern()
        {
            const string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz ";
            byte[] bytes = new byte[text.Length];
            for (int i = 0; i < text.Length; i++)
                bytes[i] = (byte)text[i];
            return bytes;
        }
    }
}
