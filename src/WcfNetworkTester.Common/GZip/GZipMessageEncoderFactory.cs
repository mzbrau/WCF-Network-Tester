using System.ServiceModel.Channels;

namespace WcfNetworkTester.Common.GZip
{
    internal sealed class GZipMessageEncoderFactory : MessageEncoderFactory
    {
        private readonly GZipMessageEncoder _encoder;

        public GZipMessageEncoderFactory(MessageEncoderFactory innerFactory)
        {
            _encoder = new GZipMessageEncoder(innerFactory.Encoder);
        }

        public override MessageEncoder Encoder => _encoder;

        public override MessageVersion MessageVersion => _encoder.MessageVersion;
    }
}
