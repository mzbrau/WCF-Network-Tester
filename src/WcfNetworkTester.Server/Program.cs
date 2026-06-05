using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using WcfNetworkTester.Common;
using WcfNetworkTester.Contracts;

namespace WcfNetworkTester.Server
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            int port = ParsePort(args, defaultPort: 8080);

            Console.Title = "WCF Network Tester — Server";
            PrintBanner(port);

            string baseAddress = $"http://localhost:{port}/";

            using (var host = new ServiceHost(typeof(NetworkTestService), new Uri(baseAddress)))
            {
                // Enable metadata so the service is discoverable (optional, useful for debugging)
                var smb = new ServiceMetadataBehavior { HttpGetEnabled = true };
                host.Description.Behaviors.Add(smb);

                // Increase timeouts for large payload transfers (20 MB)
                var debugBehavior = host.Description.Behaviors.Find<ServiceDebugBehavior>();
                if (debugBehavior == null)
                {
                    debugBehavior = new ServiceDebugBehavior();
                    host.Description.Behaviors.Add(debugBehavior);
                }
                debugBehavior.IncludeExceptionDetailInFaults = true;

                // Endpoint 1: Standard XML
                host.AddServiceEndpoint(
                    typeof(INetworkTestService),
                    BindingFactory.CreateXmlBinding(),
                    "xml");

                // Endpoint 2: GZip-compressed XML
                host.AddServiceEndpoint(
                    typeof(INetworkTestService),
                    BindingFactory.CreateGZipBinding(),
                    "gzip");

                // Endpoint 3: ProtoBuf (same BasicHttpBinding; [ProtoBehavior] handles serialisation)
                host.AddServiceEndpoint(
                    typeof(INetworkTestServiceProtoBuf),
                    BindingFactory.CreateProtoBufBinding(),
                    "protobuf");

                try
                {
                    host.Open();
                    PrintEndpoints(host, port);

                    Console.WriteLine();
                    Console.WriteLine("  Press ENTER to stop the server...");
                    Console.ReadLine();

                    host.Close();
                    Console.WriteLine("  Server stopped.");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  ERROR: {ex.Message}");
                    Console.ResetColor();
                    host.Abort();
                    Environment.Exit(1);
                }
            }
        }

        private static void PrintBanner(int port)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════╗");
            Console.WriteLine("  ║       WCF Network Tester — Server        ║");
            Console.WriteLine("  ╚══════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine($"  Starting on port {port}...");
            Console.WriteLine();
        }

        private static void PrintEndpoints(ServiceHost host, int port)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  Server started successfully. Listening on:");
            Console.ResetColor();
            Console.WriteLine($"    [XML]     http://localhost:{port}/xml");
            Console.WriteLine($"    [GZip]    http://localhost:{port}/gzip");
            Console.WriteLine($"    [ProtoBuf] http://localhost:{port}/protobuf");
        }

        private static int ParsePort(string[] args, int defaultPort)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if ((args[i] == "--port" || args[i] == "-p") &&
                    int.TryParse(args[i + 1], out int p) && p > 0 && p < 65536)
                    return p;
            }
            return defaultPort;
        }
    }
}
