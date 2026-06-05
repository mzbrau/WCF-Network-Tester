using System.ServiceModel;
using ProtoBuf.ServiceModel;

namespace WcfNetworkTester.Contracts
{
    // Same contract name as INetworkTestService so both share the same service implementation.
    [ServiceContract(Name = "INetworkTestService")]
    public interface INetworkTestServiceProtoBuf
    {
        [OperationContract]
        [ProtoBehavior]
        TestResponse Echo(TestRequest request);
    }
}
