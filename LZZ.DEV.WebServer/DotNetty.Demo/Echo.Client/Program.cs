namespace Echo.Client
{
    using System;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using DotNetty.Buffers;
    using DotNetty.Codecs;
    using DotNetty.Handlers.Logging;
    using DotNetty.Transport.Bootstrapping;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Sockets;

    static class Program
    {
        static async Task RunClientAsync()
        {
            // ExampleHelper.SetConsoleLogger();

            var group = new MultithreadEventLoopGroup();

            try
            {
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(group)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true)
                    .Handler(
                        new ActionChannelInitializer<ISocketChannel>(
                            channel =>
                            {
                                IChannelPipeline pipeline = channel.Pipeline;
                                pipeline.AddLast(new LoggingHandler());
                                pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                                pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

                                pipeline.AddLast("echo", new EchoClientHandler());
                            }));

                IChannel clientChannel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8007));

                // 建立死循环，类同于While(true)
                for (;;)
                {
                    Console.WriteLine("input you data:");
                    // 根据设置建立缓存区大小
                    IByteBuffer initialMessage = Unpooled.Buffer(256);
                    string r = Console.ReadLine();
                    // 将数据流写入缓冲区
                    initialMessage.WriteBytes(Encoding.UTF8.GetBytes(r ?? throw new InvalidOperationException()));
                    // 将缓冲区数据流写入到管道中
                    await clientChannel.WriteAndFlushAsync(initialMessage);
                    if (r.Contains("bye"))
                        break;
                }

                Console.WriteLine("byebye");


                await clientChannel.CloseAsync();
            }
            finally
            {
                await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }
        }

        static void Main() => RunClientAsync().Wait();
    }
}