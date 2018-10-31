using System;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Rpc.Common.RuntimeType.Entitys.Messages;

namespace Rpc.Common.RuntimeType.Transport.InternalAdaper
{
    public class TransportMessageChannelHandlerEncodeAdapter : ChannelHandlerAdapter
    {
        private readonly Action<IChannelHandlerContext, TransportMessage> _readAction;
        private readonly ILogger _logger;

        public TransportMessageChannelHandlerEncodeAdapter(Action<IChannelHandlerContext, TransportMessage> readAction, ILogger logger)
        {
            _readAction = readAction;
            _logger = logger;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            Task.Run(() =>
            {
                var transportMessage = (TransportMessage) message;

                _readAction(context, transportMessage);
            });
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            _logger.LogError($"与服务器：{context.Channel.RemoteAddress}通信时发送了错误。", exception);
        }
    }
}