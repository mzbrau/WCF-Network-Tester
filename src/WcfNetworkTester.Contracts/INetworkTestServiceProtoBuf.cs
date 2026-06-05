using System.ServiceModel;
using ProtoBuf.ServiceModel;

namespace WcfNetworkTester.Contracts
{
    [ServiceContract]
    public interface INetworkTestServiceProtoBuf
    {
        [OperationContract]
        [ProtoBehavior]
        TestResponse Echo(TestRequest request);
    }
}
