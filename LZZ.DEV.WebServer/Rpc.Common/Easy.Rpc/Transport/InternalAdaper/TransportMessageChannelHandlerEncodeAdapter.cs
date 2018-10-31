using System;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Rpc.Common.Easy.Rpc.Communally.Entitys.Messages;

namespace Rpc.Common.Easy.Rpc.Transport.InternalAdaper
{
    /// <summary>
    //默认通道的解码器
    /// </summary>
    public class TransportMessageChannelHandlerEncodeAdapter : ChannelHandlerAdapter
    {
        private readonly Action<IChannelHandlerContext, TransportMessage> _readAction;
        private readonly ILogger _logger;

        public TransportMessageChannelHandlerEncodeAdapter(Action<IChannelHandlerContext, TransportMessage> readAction,
            ILogger logger)
        {
            _readAction = readAction;
            _logger = logger;
        }

        /// <summary>
        /// received messages
        /// </summary>
        public override void ChannelRead(IChannelHandlerContext context, object message) =>
            Task.Run(() =>
            {
                _readAction(context, (TransportMessage) message);
                if (message is IByteBuffer buffer)
                    _logger.LogInformation($"messages capacity: {buffer.Capacity} byte");
                // ReSharper disable once IsExpressionAlwaysTrue
                if (message is TransportMessage transportMessage)
                    _logger.LogInformation($"messages type： {transportMessage.ContentType}");
                if (((TransportMessage) message)?.Content is string content)
                    _logger.LogInformation($"messages content： {content.Length}");
            });

        /// <summary>
        /// read complete happened
        /// </summary>
        public override void ChannelReadComplete(IChannelHandlerContext context) =>
            context.Flush();

        /// <summary>
        /// exception happened
        /// </summary>
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) =>
            _logger.LogError($"与远程服务器：{context.Channel.RemoteAddress} 通信时发送了错误", exception);
    }
}