using System;
using System.ServiceModel;
using WcfNetworkTester.Common;
using WcfNetworkTester.Contracts;

namespace WcfNetworkTester.Client
{
    /// <summary>
    /// Performs an initial connectivity check against all three server endpoints.
    /// Sends a tiny 1-byte request and expects a successful response.
    /// </summary>
    internal static class ConnectivityChecker
    {
        public static void Check(string host, int port)
        {
            Console.WriteLine("  Checking connectivity to server...");
            Console.WriteLine();

            CheckXml(host, port);
            CheckGZip(host, port);
            CheckProtoBuf(host, port);

            Console.WriteLine();
        }

        private static void CheckXml(string host, int port)
        {
            string url = $"http://{host}:{port}/xml";
            Console.Write($"    [Standard XML]  {url}  ... ");
            try
            {
                var factory = new ChannelFactory<INetworkTestService>(
                    BindingFactory.CreateXmlBinding(), new EndpointAddress(url));
                var channel = factory.CreateChannel();
                Ping(channel);
                ((IClientChannel)channel).Close();
                PrintOk();
            }
            catch (Exception ex) { PrintFailed(ex); }
        }

        private static void CheckGZip(string host, int port)
        {
            string url = $"http://{host}:{port}/gzip";
            Console.Write($"    [GZip XML    ]  {url}  ... ");
            try
            {
                var factory = new ChannelFactory<INetworkTestService>(
                    BindingFactory.CreateGZipBinding(), new EndpointAddress(url));
                var channel = factory.CreateChannel();
                Ping(channel);
                ((IClientChannel)channel).Close();
                PrintOk();
            }
            catch (Exception ex) { PrintFailed(ex); }
        }

        private static void CheckProtoBuf(string host, int port)
        {
            string url = $"http://{host}:{port}/protobuf";
            Console.Write($"    [ProtoBuf    ]  {url}  ... ");
            try
            {
                var factory = new ChannelFactory<INetworkTestServiceProtoBuf>(
                    BindingFactory.CreateProtoBufBinding(), new EndpointAddress(url));
                var channel = factory.CreateChannel();

                var request = new TestRequest { Payload = new byte[] { 0x01 }, ResponsePayloadSize = 1 };
                TestResponse response = channel.Echo(request);
                if (response?.Payload == null || response.Payload.Length == 0)
                    throw new Exception("Empty response received.");

                ((IClientChannel)channel).Close();
                PrintOk();
            }
            catch (Exception ex) { PrintFailed(ex); }
        }

        private static void Ping(INetworkTestService channel)
        {
            var request = new TestRequest { Payload = new byte[] { 0x01 }, ResponsePayloadSize = 1 };
            TestResponse response = channel.Echo(request);
            if (response?.Payload == null || response.Payload.Length == 0)
                throw new Exception("Empty response received.");
        }

        private static void PrintOk()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("OK");
            Console.ResetColor();
        }

        private static void PrintFailed(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"FAILED ({Truncate(ex.Message, 70)})");
            Console.ResetColor();
        }

        private static string Truncate(string s, int max) =>
            s.Length <= max ? s : s.Substring(0, max - 3) + "...";
    }
}
