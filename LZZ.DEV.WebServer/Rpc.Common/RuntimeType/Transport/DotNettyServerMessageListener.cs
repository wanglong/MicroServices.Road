﻿using System;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using Rabbit.Rpc.Transport.Codec;
using Rpc.Common.RuntimeType.Entitys.Messages;
using Rpc.Common.RuntimeType.Transport.Codec;

namespace Rpc.Common.RuntimeType.Transport
{
    public class DotNettyServerMessageListener : IMessageListener, IDisposable
    {
        #region Field

        private readonly ITransportMessageDecoder _transportMessageDecoder;
        private readonly ITransportMessageEncoder _transportMessageEncoder;
        private IChannel _channel;

        #endregion Field

        #region Constructor

        public DotNettyServerMessageListener(ILogger<DotNettyServerMessageListener> logger,
            ITransportMessageCodecFactory codecFactory)
        {
            _transportMessageEncoder = codecFactory.GetEncoder();
            _transportMessageDecoder = codecFactory.GetDecoder();
        }

        #endregion Constructor

        #region Implementation of IMessageListener

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

        #endregion Implementation of IMessageListener

        public async Task StartAsync(EndPoint endPoint)
        {
//            if (_logger.IsEnabled(LogLevel.Debug))
//                _logger.LogDebug($"准备启动服务主机，监听地址：{endPoint}。");

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
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                    pipeline.AddLast(new TransportMessageChannelHandlerAdapter(_transportMessageDecoder));
                    pipeline.AddLast(new ServerHandler(async (contenxt, message) =>
                    {
                        var sender = new DotNettyServerMessageSender(_transportMessageEncoder, contenxt);
                        await OnReceived(sender, message);
                    }, _logger));
                }));
            _channel = await bootstrap.BindAsync(endPoint);

//            if (_logger.IsEnabled(LogLevel.Debug))
//                _logger.LogDebug($"服务主机启动成功，监听地址：{endPoint}。");
        }

        #region Implementation of IDisposable

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Task.Run(async () => { await _channel.DisconnectAsync(); }).Wait();
        }

        #endregion Implementation of IDisposable

        #region Help Class

        private class ServerHandler : ChannelHandlerAdapter
        {
            private readonly Action<IChannelHandlerContext, TransportMessage> _readAction;
            private readonly ILogger _logger;

            public ServerHandler(Action<IChannelHandlerContext, TransportMessage> readAction, ILogger logger)
            {
                _readAction = readAction;
                _logger = logger;
            }

            #region Overrides of ChannelHandlerAdapter

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
                if (_logger.IsEnabled(LogLevel.Error))
                    _logger.LogError($"与服务器：{context.Channel.RemoteAddress}通信时发送了错误。", exception);
            }

            #endregion Overrides of ChannelHandlerAdapter
        }

        #endregion Help Class
    }
}