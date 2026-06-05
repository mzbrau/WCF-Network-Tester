using System.Runtime.Serialization;
using ProtoBuf;

namespace WcfNetworkTester.Contracts
{
    [DataContract]
    [ProtoContract]
    public class TestResponse
    {
        /// <summary>The response payload (size equals the ResponsePayloadSize requested).</summary>
        [DataMember(Order = 1)]
        [ProtoMember(1)]
        public byte[] Payload { get; set; }
    }
}
