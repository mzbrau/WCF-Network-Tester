using System;
using System.Collections.Generic;
using System.Linq;

namespace WcfNetworkTester.Client
{
    /// <summary>
    /// Renders a results table using box-drawing characters.
    /// Groups results by encoding type, with a separator row between groups.
    /// </summary>
    internal static class ResultDisplay
    {
        // Column widths (content only, excluding border chars and padding)
        private const int ColEncoding  = 14;
        private const int ColReqSize   = 12;
        private const int ColRespSize  = 13;
        private const int ColAverage   = 12;
        private const int ColMin       = 12;
        private const int ColMax       = 12;
        private const int ColStdDev    = 12;
        private const int ColStatus    = 18;

        public static void Show(List<TestResult> results)
        {
            if (results == null || results.Count == 0)
            {
                Console.WriteLine("  No results to display.");
                return;
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            PrintTopBorder();
            PrintTitle();
            PrintHeaderSeparator();
            PrintHeaderRow();
            PrintHeaderSeparator();
            Console.ResetColor();

            var groups = results.GroupBy(r => r.Encoding).ToList();

            for (int g = 0; g < groups.Count; g++)
            {
                if (g > 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    PrintGroupSeparator();
                    Console.ResetColor();
                }

                foreach (var result in groups[g])
                {
                    PrintDataRow(result);
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            PrintBottomBorder();
            Console.ResetColor();
            Console.WriteLine();
        }

        // ── Border helpers ───────────────────────────────────────────────────────

        private static int TotalWidth =>
            1 + ColEncoding + 2 +
            1 + ColReqSize  + 2 +
            1 + ColRespSize + 2 +
            1 + ColAverage  + 2 +
            1 + ColMin      + 2 +
            1 + ColMax      + 2 +
            1 + ColStdDev   + 2 +
            1 + ColStatus   + 2 + 1;

        private static void PrintTopBorder()
        {
            Console.Write("  ╔");
            Console.Write(new string('═', ColEncoding  + 2));
            Console.Write("╦");
            Console.Write(new string('═', ColReqSize   + 2));
            Console.Write("╦");
            Console.Write(new string('═', ColRespSize  + 2));
            Console.Write("╦");
            Console.Write(new string('═', ColAverage   + 2));
            Console.Write("╦");
            Console.Write(new string('═', ColMin       + 2));
            Console.Write("╦");
            Console.Write(new string('═', ColMax       + 2));
            Console.Write("╦");
            Console.Write(new string('═', ColStdDev    + 2));
            Console.Write("╦");
            Console.Write(new string('═', ColStatus    + 2));
            Console.WriteLine("╗");
        }

        private static void PrintTitle()
        {
            int inner = ColEncoding + ColReqSize + ColRespSize + ColAverage + ColMin + ColMax + ColStdDev + ColStatus + 7 * 3;
            string title = $"WCF Network Performance Test Results ({TestRunner.TotalRunsPerTest} runs, {TestRunner.WarmupRunsPerTest} warm-up)";
            string padded = title.PadLeft((inner + title.Length) / 2).PadRight(inner);
            Console.WriteLine($"  ║ {padded} ║");
        }

        private static void PrintHeaderSeparator()
        {
            Console.Write("  ╠");
            Console.Write(new string('═', ColEncoding  + 2));
            Console.Write("╦");
            Console.Write(new string('═', ColReqSize   + 2));
            Console.Write("╦");
            Console.Write(new string('═', ColRespSize  + 2));
            Console.Write("╦");
            Console.Write(new string('═', ColAverage   + 2));
            Console.Write("╦");
            Console.Write(new string('═', ColMin       + 2));
            Console.Write("╦");
            Console.Write(new string('═', ColMax       + 2));
            Console.Write("╦");
            Console.Write(new string('═', ColStdDev    + 2));
            Console.Write("╦");
            Console.Write(new string('═', ColStatus    + 2));
            Console.WriteLine("╣");
        }

        private static void PrintGroupSeparator()
        {
            Console.Write("  ╠");
            Console.Write(new string('═', ColEncoding  + 2));
            Console.Write("╬");
            Console.Write(new string('═', ColReqSize   + 2));
            Console.Write("╬");
            Console.Write(new string('═', ColRespSize  + 2));
            Console.Write("╬");
            Console.Write(new string('═', ColAverage   + 2));
            Console.Write("╬");
            Console.Write(new string('═', ColMin       + 2));
            Console.Write("╬");
            Console.Write(new string('═', ColMax       + 2));
            Console.Write("╬");
            Console.Write(new string('═', ColStdDev    + 2));
            Console.Write("╬");
            Console.Write(new string('═', ColStatus    + 2));
            Console.WriteLine("╣");
        }

        private static void PrintBottomBorder()
        {
            Console.Write("  ╚");
            Console.Write(new string('═', ColEncoding  + 2));
            Console.Write("╩");
            Console.Write(new string('═', ColReqSize   + 2));
            Console.Write("╩");
            Console.Write(new string('═', ColRespSize  + 2));
            Console.Write("╩");
            Console.Write(new string('═', ColAverage   + 2));
            Console.Write("╩");
            Console.Write(new string('═', ColMin       + 2));
            Console.Write("╩");
            Console.Write(new string('═', ColMax       + 2));
            Console.Write("╩");
            Console.Write(new string('═', ColStdDev    + 2));
            Console.Write("╩");
            Console.Write(new string('═', ColStatus    + 2));
            Console.WriteLine("╝");
        }

        private static void PrintHeaderRow()
        {
            Console.WriteLine(
                $"  ║ {"Encoding".PadRight(ColEncoding)} ║" +
                $" {"Req Size".PadRight(ColReqSize)} ║" +
                $" {"Resp Size".PadRight(ColRespSize)} ║" +
                $" {"Average".PadLeft(ColAverage)} ║" +
                $" {"Min".PadLeft(ColMin)} ║" +
                $" {"Max".PadLeft(ColMax)} ║" +
                $" {"Std Dev".PadLeft(ColStdDev)} ║" +
                $" {"Status".PadRight(ColStatus)} ║");
        }

        private static void PrintDataRow(TestResult result)
        {
            string encoding = TestRunner.EncodingLabel(result.Encoding).TrimEnd();
            string reqSize  = TestRunner.SizeLabel(result.RequestSize).TrimEnd();
            string respSize = TestRunner.SizeLabel(result.ResponseSize).TrimEnd();

            string average;
            string min;
            string max;
            string stdDev;
            string status;
            ConsoleColor statusColor;

            if (result.Success)
            {
                average     = $"{result.AverageDurationMs,8:F1} ms";
                min         = $"{result.MinDurationMs,8:F1} ms";
                max         = $"{result.MaxDurationMs,8:F1} ms";
                stdDev      = $"{result.StdDevDurationMs,8:F1} ms";
                status      = "OK";
                statusColor = ConsoleColor.Green;
            }
            else
            {
                average     = "      N/A   ";
                min         = "      N/A   ";
                max         = "      N/A   ";
                stdDev      = "      N/A   ";
                string msg  = result.ErrorMessage ?? "Unknown error";
                status      = msg.Length > ColStatus ? msg.Substring(0, ColStatus - 3) + "..." : msg;
                statusColor = ConsoleColor.Red;
            }

            // Print fixed columns
            Console.Write(
                $"  ║ {encoding.PadRight(ColEncoding)} ║" +
                $" {reqSize.PadRight(ColReqSize)} ║" +
                $" {respSize.PadRight(ColRespSize)} ║" +
                $" {average.PadLeft(ColAverage)} ║" +
                $" {min.PadLeft(ColMin)} ║" +
                $" {max.PadLeft(ColMax)} ║" +
                $" {stdDev.PadLeft(ColStdDev)} ║ ");

            // Print status in colour
            Console.ForegroundColor = statusColor;
            Console.Write(status.PadRight(ColStatus));
            Console.ResetColor();
            Console.WriteLine(" ║");
        }
    }
}
