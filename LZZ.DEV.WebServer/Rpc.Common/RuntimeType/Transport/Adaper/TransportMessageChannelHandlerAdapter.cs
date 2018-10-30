﻿using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Rpc.Common.RuntimeType.Transport.Codec;

namespace Rpc.Common.RuntimeType.Transport.Adaper
{
    internal class TransportMessageChannelHandlerAdapter : ChannelHandlerAdapter
    {
        private readonly ITransportMessageDecoder _transportMessageDecoder;

        public TransportMessageChannelHandlerAdapter(ITransportMessageDecoder transportMessageDecoder)
        {
            _transportMessageDecoder = transportMessageDecoder;
        }

        #region Overrides of ChannelHandlerAdapter

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = (IByteBuffer)message;
            var data = buffer.Array;
            var transportMessage = _transportMessageDecoder.Decode(data);
            context.FireChannelRead(transportMessage);
        }

        #endregion Overrides of ChannelHandlerAdapter
    }
}