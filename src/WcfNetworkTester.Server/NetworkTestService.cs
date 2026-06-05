using System;
using WcfNetworkTester.Contracts;

namespace WcfNetworkTester.Server
{
    /// <summary>
    /// Single implementation serving all three endpoints (XML, GZip, ProtoBuf).
    /// Generates a response payload of the size requested by the client.
    /// </summary>
    public class NetworkTestService : INetworkTestService, INetworkTestServiceProtoBuf
    {
        private static readonly byte[] PatternSource = BuildPattern();

        public TestResponse Echo(TestRequest request)
        {
            int size = Math.Max(0, request?.ResponsePayloadSize ?? 0);
            return new TestResponse { Payload = GeneratePayload(size) };
        }

        private static byte[] GeneratePayload(int size)
        {
            if (size == 0)
                return Array.Empty<byte>();

            byte[] buffer = new byte[size];
            int patternLen = PatternSource.Length;

            for (int i = 0; i < size; i++)
                buffer[i] = PatternSource[i % patternLen];

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
