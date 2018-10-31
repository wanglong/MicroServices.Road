using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Rpc.Common.RuntimeType.Transport.Codec;

namespace Rpc.Common.RuntimeType.Transport.InternalAdaper
{
    /// <summary>
    /// 一个标准通道的处理适配器
    /// </summary>
    internal class TransportMessageChannelHandlerAdapter : ChannelHandlerAdapter
    {
        private readonly ITransportMessageDecoder _transportMessageDecoder;

        public TransportMessageChannelHandlerAdapter(ITransportMessageDecoder transportMessageDecoder)
        {
            _transportMessageDecoder = transportMessageDecoder;
        }
        
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            context.FireChannelRead(_transportMessageDecoder.Decode(((IByteBuffer) message).Array));
        }
    }
}