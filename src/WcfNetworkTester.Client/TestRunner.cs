using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using WcfNetworkTester.Common;
using WcfNetworkTester.Contracts;

namespace WcfNetworkTester.Client
{
    /// <summary>
    /// Runs all 16 request × response size combinations for a given encoding type
    /// and returns the collected <see cref="TestResult"/> list.
    /// </summary>
    internal static class TestRunner
    {
        internal const int TotalRunsPerTest = 12;
        internal const int WarmupRunsPerTest = 2;
        internal const int MeasuredRunsPerTest = TotalRunsPerTest - WarmupRunsPerTest;

        private static readonly PayloadSize[] Sizes =
        {
            PayloadSize.Small,
            PayloadSize.Medium,
            PayloadSize.Large,
            PayloadSize.Enormous
        };

        public static List<TestResult> Run(EncodingType encoding, string host, int port)
        {
            var results = new List<TestResult>();

            foreach (PayloadSize reqSize in Sizes)
            {
                foreach (PayloadSize respSize in Sizes)
                {
                    var result = RunOne(encoding, host, port, reqSize, respSize);
                    results.Add(result);
                }
            }

            return results;
        }

        private static TestResult RunOne(
            EncodingType encoding, string host, int port,
            PayloadSize reqSize, PayloadSize respSize)
        {
            string label = $"[{EncodingLabel(encoding)}] {SizeLabel(reqSize),7} → {SizeLabel(respSize),7}";
            Console.Write($"    Running {label} ...");

            var result = new TestResult
            {
                Encoding     = encoding,
                RequestSize  = reqSize,
                ResponseSize = respSize
            };

            try
            {
                byte[] payload = PayloadGenerator.Generate(reqSize);
                var request = new TestRequest
                {
                    Payload             = payload,
                    ResponsePayloadSize = (int)respSize
                };

                var durations = new List<double>(TotalRunsPerTest);

                for (int run = 0; run < TotalRunsPerTest; run++)
                {
                    var sw = Stopwatch.StartNew();
                    TestResponse response = CallEndpoint(encoding, host, port, request);
                    sw.Stop();

                    if (response?.Payload == null)
                        throw new Exception("Null response received.");

                    durations.Add(sw.Elapsed.TotalMilliseconds);
                }

                List<double> measuredDurations = durations.Skip(WarmupRunsPerTest).ToList();
                result.AverageDurationMs = measuredDurations.Average();
                result.MinDurationMs = measuredDurations.Min();
                result.MaxDurationMs = measuredDurations.Max();
                result.StdDevDurationMs = CalculateStandardDeviation(measuredDurations, result.AverageDurationMs);
                result.Success    = true;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  avg {result.AverageDurationMs,9:F1} ms");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                result.Success      = false;
                result.ErrorMessage = ex.Message;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  FAILED");
                Console.ResetColor();
            }

            return result;
        }

        private static TestResponse CallEndpoint(
            EncodingType encoding, string host, int port, TestRequest request)
        {
            string url = $"http://{host}:{port}/{EndpointPath(encoding)}";
            var address = new EndpointAddress(url);

            switch (encoding)
            {
                case EncodingType.StandardXml:
                {
                    var factory = new ChannelFactory<INetworkTestService>(
                        BindingFactory.CreateXmlBinding(), address);
                    var channel = factory.CreateChannel();
                    try     { return channel.Echo(request); }
                    finally { TryClose((IClientChannel)channel); }
                }

                case EncodingType.GZipXml:
                {
                    var factory = new ChannelFactory<INetworkTestService>(
                        BindingFactory.CreateGZipBinding(), address);
                    var channel = factory.CreateChannel();
                    try     { return channel.Echo(request); }
                    finally { TryClose((IClientChannel)channel); }
                }

                case EncodingType.ProtoBuf:
                {
                    var factory = new ChannelFactory<INetworkTestServiceProtoBuf>(
                        BindingFactory.CreateProtoBufBinding(), address);
                    var channel = factory.CreateChannel();
                    try     { return channel.Echo(request); }
                    finally { TryClose((IClientChannel)channel); }
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(encoding));
            }
        }

        private static void TryClose(IClientChannel channel)
        {
            try { channel.Close(); }
            catch { channel.Abort(); }
        }

        private static double CalculateStandardDeviation(IReadOnlyList<double> values, double mean)
        {
            if (values == null || values.Count <= 1)
                return 0;

            double sumOfSquares = 0;

            foreach (double value in values)
            {
                double delta = value - mean;
                sumOfSquares += delta * delta;
            }

            return Math.Sqrt(sumOfSquares / (values.Count - 1));
        }

        // ── Labels ──────────────────────────────────────────────────────────────

        internal static string EncodingLabel(EncodingType encoding)
        {
            switch (encoding)
            {
                case EncodingType.StandardXml: return "Standard XML";
                case EncodingType.GZipXml:     return "GZip XML    ";
                case EncodingType.ProtoBuf:    return "ProtoBuf    ";
                default:                       return encoding.ToString();
            }
        }

        internal static string SizeLabel(PayloadSize size)
        {
            switch (size)
            {
                case PayloadSize.Small:    return "1 KB   ";
                case PayloadSize.Medium:   return "100 KB ";
                case PayloadSize.Large:    return "1 MB   ";
                case PayloadSize.Enormous: return "20 MB  ";
                default:                  return size.ToString();
            }
        }

        private static string EndpointPath(EncodingType encoding)
        {
            switch (encoding)
            {
                case EncodingType.StandardXml: return "xml";
                case EncodingType.GZipXml:     return "gzip";
                case EncodingType.ProtoBuf:    return "protobuf";
                default: throw new ArgumentOutOfRangeException(nameof(encoding));
            }
        }
    }
}
