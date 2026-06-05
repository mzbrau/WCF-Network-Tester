using System;
using System.Collections.Generic;

namespace WcfNetworkTester.Client
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "WCF Network Tester — Client";

            string host = ParseArg(args, "--host", "localhost");
            int    port = int.TryParse(ParseArg(args, "--port", "8080"), out int p) ? p : 8080;

            PrintBanner(host, port);

            ConnectivityChecker.Check(host, port);

            bool exit = false;
            List<TestResult> lastResults = null;

            while (!exit)
            {
                PrintMenu();

                string input = Console.ReadLine()?.Trim();
                Console.WriteLine();

                switch (input)
                {
                    case "1":
                        lastResults = RunTests(EncodingType.StandardXml, host, port);
                        break;

                    case "2":
                        lastResults = RunTests(EncodingType.GZipXml, host, port);
                        break;

                    case "3":
                        lastResults = RunTests(EncodingType.ProtoBuf, host, port);
                        break;

                    case "4":
                        lastResults = RunAllTests(host, port);
                        break;

                    case "5":
                        exit = true;
                        Console.WriteLine("  Goodbye!");
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("  Invalid option. Please choose 1–5.");
                        Console.ResetColor();
                        break;
                }
            }
        }

        private static List<TestResult> RunTests(EncodingType encoding, string host, int port)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  Running {TestRunner.EncodingLabel(encoding).TrimEnd()} tests " +
                              $"(16 combinations)...");
            Console.ResetColor();
            Console.WriteLine();

            var results = TestRunner.Run(encoding, host, port);
            ResultDisplay.Show(results);

            return results;
        }

        private static List<TestResult> RunAllTests(string host, int port)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("  Running all tests (48 combinations across 3 encodings)...");
            Console.ResetColor();
            Console.WriteLine();

            var all = new List<TestResult>();

            foreach (EncodingType encoding in new[]
            {
                EncodingType.StandardXml,
                EncodingType.GZipXml,
                EncodingType.ProtoBuf
            })
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"  ─── {TestRunner.EncodingLabel(encoding).TrimEnd()} ───");
                Console.ResetColor();
                all.AddRange(TestRunner.Run(encoding, host, port));
                Console.WriteLine();
            }

            ResultDisplay.Show(all);
            return all;
        }

        // ── UI helpers ───────────────────────────────────────────────────────────

        private static void PrintBanner(string host, int port)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════╗");
            Console.WriteLine("  ║       WCF Network Tester — Client        ║");
            Console.WriteLine("  ╚══════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine($"  Server: {host}:{port}");
            Console.WriteLine();
        }

        private static void PrintMenu()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("  ┌─────────────────────────────┐");
            Console.WriteLine("  │        Select a Test        │");
            Console.WriteLine("  ├─────────────────────────────┤");
            Console.WriteLine("  │  [1] Standard XML           │");
            Console.WriteLine("  │  [2] GZip XML               │");
            Console.WriteLine("  │  [3] ProtoBuf               │");
            Console.WriteLine("  │  [4] Run All Tests          │");
            Console.WriteLine("  │  [5] Exit                   │");
            Console.WriteLine("  └─────────────────────────────┘");
            Console.ResetColor();
            Console.Write("  Choose option: ");
        }

        // ── Argument parsing ─────────────────────────────────────────────────────

        private static string ParseArg(string[] args, string flag, string defaultValue)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i].Equals(flag, StringComparison.OrdinalIgnoreCase))
                    return args[i + 1];
            }
            return defaultValue;
        }
    }
}
