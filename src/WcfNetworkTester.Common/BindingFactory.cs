using System.ServiceModel;
using System.ServiceModel.Channels;
using WcfNetworkTester.Common.GZip;

namespace WcfNetworkTester.Common
{
    /// <summary>
    /// Creates the three WCF bindings used by both the server and the client.
    /// All bindings share a generous MaxReceivedMessageSize to support the
    /// 20 MB "enormous" test payload.
    /// </summary>
    public static class BindingFactory
    {
        private const long MaxMessageSize = 50 * 1024 * 1024; // 50 MB headroom

        /// <summary>Standard BasicHttpBinding with text/XML encoding.</summary>
        public static BasicHttpBinding CreateXmlBinding()
        {
            return new BasicHttpBinding
            {
                MaxReceivedMessageSize = MaxMessageSize,
                MaxBufferSize          = (int)MaxMessageSize,
                ReaderQuotas           =
                {
                    MaxArrayLength       = (int)MaxMessageSize,
                    MaxBytesPerRead      = (int)MaxMessageSize,
                    MaxStringContentLength = (int)MaxMessageSize
                }
            };
        }

        /// <summary>
        /// Custom binding identical to BasicHttp but with GZip message encoding.
        /// The HTTP transport still uses SOAP 1.1 over HTTP.
        /// </summary>
        public static CustomBinding CreateGZipBinding()
        {
            var gzipEncoding = new GZipMessageEncodingBindingElement
            {
                MessageVersion = MessageVersion.Soap11
            };

            var transport = new HttpTransportBindingElement
            {
                MaxReceivedMessageSize = MaxMessageSize,
                MaxBufferSize          = (int)MaxMessageSize
            };

            return new CustomBinding(gzipEncoding, transport);
        }

        /// <summary>
        /// BasicHttpBinding used for the ProtoBuf endpoint.
        /// The [ProtoBehavior] attribute on the contract overrides the serialiser;
        /// the binding itself is a standard BasicHttpBinding.
        /// </summary>
        public static BasicHttpBinding CreateProtoBufBinding()
        {
            return new BasicHttpBinding
            {
                MaxReceivedMessageSize = MaxMessageSize,
                MaxBufferSize          = (int)MaxMessageSize,
                ReaderQuotas           =
                {
                    MaxArrayLength       = (int)MaxMessageSize,
                    MaxBytesPerRead      = (int)MaxMessageSize,
                    MaxStringContentLength = (int)MaxMessageSize
                }
            };
        }
    }
}
