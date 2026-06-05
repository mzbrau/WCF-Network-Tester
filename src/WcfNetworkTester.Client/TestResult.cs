namespace WcfNetworkTester.Client
{
    /// <summary>Result of a single test case (one request/response size combination).</summary>
    public class TestResult
    {
        public EncodingType Encoding      { get; set; }
        public PayloadSize  RequestSize   { get; set; }
        public PayloadSize  ResponseSize  { get; set; }
        public double       AverageDurationMs { get; set; }
        public double       MinDurationMs     { get; set; }
        public double       MaxDurationMs     { get; set; }
        public double       StdDevDurationMs  { get; set; }
        public bool         Success       { get; set; }
        public string       ErrorMessage  { get; set; }
    }
}
