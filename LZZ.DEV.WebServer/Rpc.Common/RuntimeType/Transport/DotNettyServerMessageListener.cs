using System;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Rpc.Common.RuntimeType.Entitys.Messages;
using Rpc.Common.RuntimeType.Transport.Adaper;
using Rpc.Common.RuntimeType.Transport.Codec;

namespace Rpc.Common.RuntimeType.Transport
{
    public class DotNettyServerMessageListener : IMessageListener, IDisposable
    {
        private readonly ITransportMessageDecoder _transportMessageDecoder;
        private readonly ITransportMessageEncoder _transportMessageEncoder;
        private IChannel _channel;

        public DotNettyServerMessageListener(
//            ILogger<DotNettyServerMessageListener> logger,
            ITransportMessageCodecFactory codecFactory)
        {
            _transportMessageEncoder = codecFactory.GetEncoder();
            _transportMessageDecoder = codecFactory.GetDecoder();
        }

        public event ReceivedDelegate Received;

        /// <summary>
        /// 触发接收到消息事件。
        /// </summary>
        /// <param name="sender">消息发送者。</param>
        /// <param name="message">接收到的消息。</param>
        /// <returns>一个任务。</returns>
        public async Task OnReceived(IMessageSender sender, TransportMessage message)
        {
            if (Received == null)
                return;
            await Received(sender, message);
        }

        public async Task StartAsync(EndPoint endPoint)
        {
            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup();
            var bootstrap = new ServerBootstrap();
            bootstrap
                .Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 100)
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddLast(new LengthFieldPrepender(4));
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 2, 0, 2));
                    pipeline.AddLast(new TransportMessageChannelHandlerAdapter(_transportMessageDecoder));
                    pipeline.AddLast(new ServerHandler(async (contenxt, message) =>
                    {
                        var sender = new DotNettyServerMessageSender(_transportMessageEncoder, contenxt);
                        await OnReceived(sender, message);
                    }));
                }));
            _channel = await bootstrap.BindAsync(endPoint);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Task.Run(async () => { await _channel.DisconnectAsync(); }).Wait();
        }

        private class ServerHandler : ChannelHandlerAdapter
        {
            private readonly Action<IChannelHandlerContext, TransportMessage> _readAction;

            public ServerHandler(Action<IChannelHandlerContext, TransportMessage> readAction)
            {
                _readAction = readAction;
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
//                if (_logger.IsEnabled(LogLevel.Error))
//                    _logger.LogError($"与服务器：{context.Channel.RemoteAddress}通信时发送了错误。", exception);
            }
        }
    }
}