using System.ServiceModel;

namespace WcfNetworkTester.Contracts
{
    [ServiceContract]
    public interface INetworkTestService
    {
        [OperationContract]
        TestResponse Echo(TestRequest request);
    }
}
