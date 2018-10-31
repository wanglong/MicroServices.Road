﻿using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Rpc.Common.RuntimeType.Transport.Codec;

namespace Rpc.Common.RuntimeType.Transport.InternalAdaper
{
    /// <summary>
    /// 一个标准通道的编码适配器
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