using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DotNetty.Common.Utilities;
using Newtonsoft.Json;
using ProtoBuf;

namespace Echo.Server
{
    using System;
    using System.Text;
    using DotNetty.Buffers;
    using DotNetty.Transport.Channels;

    /// <summary>
    /// 服务端处理事件函数
    /// </summary>
    public class EchoServerHandler : ChannelHandlerAdapter
    {
        /// <summary>
        /// 管道开始读
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is IByteBuffer buffer)
            {
                Console.WriteLine($"message length is {buffer.Capacity}");
                var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(buffer.ToString(Encoding.UTF8).Replace(")", ""));

                byte[] msg = null;
                if (json["func"].Contains("sayHello"))
                {
                    msg = Encoding.UTF8.GetBytes(Say.SayHello(json["username"]));
                }

                if (json["func"].Contains("sayByebye"))
                {
                    msg = Encoding.UTF8.GetBytes(Say.SayByebye(json["username"]));
                }

                // 设置数据大小
                if (msg == null) return;
                var b = Unpooled.Buffer(msg.Length, msg.Length);
                IByteBuffer byteBuffer = b.WriteBytes(msg);
                context.WriteAsync(byteBuffer);
            }
        }

        private Dictionary<string, string> ToObject(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                var fw = Serializer.Deserialize<Dictionary<string, string>>(stream);
                return fw;
            }
        }

        /// <summary>
        /// 管道读取完成 
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override Task WriteAsync(IChannelHandlerContext context, object message)
        {
            if (message is IByteBuffer buffer)
            {
                var content = buffer.ToString(Encoding.UTF8);
                Console.WriteLine(content);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 出现异常
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }

        public byte[] ToArray(IByteBuffer iByteBuffer)
        {
            int readableBytes = iByteBuffer.ReadableBytes;
            if (readableBytes == 0)
            {
                return ArrayExtensions.ZeroBytes;
            }

            if (iByteBuffer.HasArray)
            {
                return iByteBuffer.Array.Slice(iByteBuffer.ArrayOffset + iByteBuffer.ReaderIndex, readableBytes);
            }

            var bytes = new byte[readableBytes];
            iByteBuffer.GetBytes(iByteBuffer.ReaderIndex, bytes);
            return bytes;
        }
    }

    public static class Say
    {
        public static string SayHello(string content)
        {
            return $"hello {content}";
        }

        public static string SayByebye(string content)
        {
            return $"byebye {content}";
        }
    }
}