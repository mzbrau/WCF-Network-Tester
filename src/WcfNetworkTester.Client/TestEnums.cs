namespace WcfNetworkTester.Client
{
    /// <summary>Defines the four test payload sizes (uncompressed bytes).</summary>
    public enum PayloadSize
    {
        Small    = 1 * 1024,            //   1 KB
        Medium   = 100 * 1024,          // 100 KB
        Large    = 1 * 1024 * 1024,     //   1 MB
        Enormous = 20 * 1024 * 1024     //  20 MB
    }

    /// <summary>The three WCF encoding types under test.</summary>
    public enum EncodingType
    {
        StandardXml,
        GZipXml,
        ProtoBuf
    }
}
