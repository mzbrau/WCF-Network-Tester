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
            string hostName = ParseHost(args, "localhost");
            int port = ParsePort(args, defaultPort: 8080);

            Console.Title = "WCF Network Tester — Server";
            PrintBanner(hostName, port);

            string baseAddress = $"http://{hostName}:{port}/";

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
                    PrintEndpoints(hostName, port);

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

        private static void PrintBanner(string hostName, int port)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════╗");
            Console.WriteLine("  ║       WCF Network Tester — Server        ║");
            Console.WriteLine("  ╚══════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine($"  Starting on {hostName}:{port}...");
            Console.WriteLine();
        }

        private static void PrintEndpoints(string hostName, int port)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  Server started successfully. Listening on:");
            Console.ResetColor();
            Console.WriteLine($"    [XML]      http://{hostName}:{port}/xml");
            Console.WriteLine($"    [GZip]     http://{hostName}:{port}/gzip");
            Console.WriteLine($"    [ProtoBuf] http://{hostName}:{port}/protobuf");
        }

        private static string ParseHost(string[] args, string defaultHost)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if (TryParseHostToken(arg, out string host))
                    return host;

                if ((arg.Equals("--host", StringComparison.OrdinalIgnoreCase) ||
                     arg.Equals("-h", StringComparison.OrdinalIgnoreCase)) &&
                    i + 1 < args.Length)
                    return args[i + 1];
            }

            return defaultHost;
        }

        private static int ParsePort(string[] args, int defaultPort)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (TryParsePortToken(arg, out int port))
                    return port;

                if ((arg.Equals("--port", StringComparison.OrdinalIgnoreCase) ||
                     arg.Equals("-p", StringComparison.OrdinalIgnoreCase)) &&
                    i + 1 < args.Length &&
                    TryParsePortValue(args[i + 1], out port))
                    return port;
            }

            return defaultPort;
        }

        private static bool TryParsePortToken(string arg, out int port)
        {
            const StringComparison comparison = StringComparison.OrdinalIgnoreCase;

            if (arg.StartsWith("--port=", comparison))
                return TryParsePortValue(arg.Substring("--port=".Length), out port);

            if (arg.StartsWith("-p=", comparison))
                return TryParsePortValue(arg.Substring("-p=".Length), out port);

            port = 0;
            return false;
        }

        private static bool TryParsePortValue(string value, out int port)
        {
            return int.TryParse(value, out port) && port > 0 && port < 65536;
        }

        private static bool TryParseHostToken(string arg, out string host)
        {
            const StringComparison comparison = StringComparison.OrdinalIgnoreCase;

            if (arg.StartsWith("--host=", comparison))
            {
                host = arg.Substring("--host=".Length);
                return !string.IsNullOrWhiteSpace(host);
            }

            if (arg.StartsWith("-h=", comparison))
            {
                host = arg.Substring("-h=".Length);
                return !string.IsNullOrWhiteSpace(host);
            }

            host = null;
            return false;
        }
    }
}
