using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using System;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Newtonsoft.Json;

namespace Echo.Client
{
    public class EchoClientHandler : ChannelHandlerAdapter
    {
        IByteBuffer _initialMessage;

        public EchoClientHandler()
        {
            var hello = new Dictionary<string, string>
            {
                {"func", "sayHello"},
                {"username", "stevelee"}
            };
            SendMessage(ToStream(JsonConvert.SerializeObject(hello)));
        }

        private byte[] ToStream(string msg)
        {
            Console.WriteLine($"string length is {msg.Length}");
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, msg);
                return stream.ToArray();
            }
        }

        private void SendMessage(byte[] msg)
        {
            Console.WriteLine($"byte length is {msg.Length}");
            _initialMessage = Unpooled.Buffer(msg.Length, msg.Length);
            _initialMessage.WriteBytes(msg);
        }

        public override void ChannelActive(IChannelHandlerContext context) => context.WriteAndFlushAsync(_initialMessage);

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is IByteBuffer byteBuffer)
            {
                Console.WriteLine($"message length is {byteBuffer.Capacity}");
                Console.WriteLine("Received from server: " + byteBuffer.ToString(Encoding.UTF8));
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }
    }
}