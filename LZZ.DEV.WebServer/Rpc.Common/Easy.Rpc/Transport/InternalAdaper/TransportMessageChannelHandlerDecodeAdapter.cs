using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Rpc.Common.Easy.Rpc.Transport.Codec;

namespace Rpc.Common.Easy.Rpc.Transport.InternalAdaper
{
    /// <summary>
    //标准通道的编码适配器
    /// </summary>
    internal class TransportMessageChannelHandlerDecodeAdapter : ChannelHandlerAdapter
    {
        private readonly ITransportMessageDecoder _transportMessageDecoder;

        public TransportMessageChannelHandlerDecodeAdapter(ITransportMessageDecoder transportMessageDecoder)
        {
            _transportMessageDecoder = transportMessageDecoder;
        }
        
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            context.FireChannelRead(_transportMessageDecoder.Decode(((IByteBuffer) message).Array));
        }
    }
}