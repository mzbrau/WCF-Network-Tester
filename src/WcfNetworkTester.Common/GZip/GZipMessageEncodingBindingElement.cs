using System;
using System.ServiceModel.Channels;
using System.Xml;

namespace WcfNetworkTester.Common.GZip
{
    /// <summary>
    /// Binding element that substitutes the standard text message encoder with
    /// the GZip-wrapping encoder. Drop this into a custom binding in place of
    /// <see cref="TextMessageEncodingBindingElement"/>.
    /// </summary>
    public sealed class GZipMessageEncodingBindingElement : MessageEncodingBindingElement
    {
        private readonly TextMessageEncodingBindingElement _innerBindingElement;

        public GZipMessageEncodingBindingElement()
        {
            _innerBindingElement = new TextMessageEncodingBindingElement
            {
                MessageVersion = MessageVersion.Soap11
            };
        }

        private GZipMessageEncodingBindingElement(GZipMessageEncodingBindingElement source)
        {
            _innerBindingElement = (TextMessageEncodingBindingElement)source._innerBindingElement.Clone();
        }

        public override MessageVersion MessageVersion
        {
            get => _innerBindingElement.MessageVersion;
            set => _innerBindingElement.MessageVersion = value;
        }

        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new GZipMessageEncoderFactory(_innerBindingElement.CreateMessageEncoderFactory());
        }

        public override BindingElement Clone() => new GZipMessageEncodingBindingElement(this);

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(
            BindingContext context)
        {
            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context) => true;

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(
            BindingContext context)
        {
            context.BindingParameters.Add(this);
            return context.BuildInnerChannelListener<TChannel>();
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context) => true;

        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(XmlDictionaryReaderQuotas))
                return _innerBindingElement.GetProperty<T>(context);
            return base.GetProperty<T>(context);
        }
    }
}
