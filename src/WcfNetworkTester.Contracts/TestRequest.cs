using System.Runtime.Serialization;
using ProtoBuf;

namespace WcfNetworkTester.Contracts
{
    [DataContract]
    [ProtoContract]
    public class TestRequest
    {
        /// <summary>The payload to send (determines uncompressed request size).</summary>
        [DataMember(Order = 1)]
        [ProtoMember(1)]
        public byte[] Payload { get; set; }

        /// <summary>Number of bytes the server should include in its response payload.</summary>
        [DataMember(Order = 2)]
        [ProtoMember(2)]
        public int ResponsePayloadSize { get; set; }
    }
}
