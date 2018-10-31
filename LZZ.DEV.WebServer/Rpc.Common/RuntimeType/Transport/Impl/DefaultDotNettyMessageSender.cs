using DotNetty.Buffers;
using Rpc.Common.RuntimeType.Entitys.Messages;
using Rpc.Common.RuntimeType.Transport.Codec;

namespace Rpc.Common.RuntimeType.Transport.Impl
{
    /// <summary>
    /// 基于DotNetty的消息发送者基类
    /// </summary>
    public abstract class DefaultDotNettyMessageSender
    {
        private readonly ITransportMessageEncoder _transportMessageEncoder;

        protected DefaultDotNettyMessageSender(ITransportMessageEncoder transportMessageEncoder)
        {
            _transportMessageEncoder = transportMessageEncoder;
        }

        protected IByteBuffer GetByteBuffer(TransportMessage message)
        {
            var data = _transportMessageEncoder.Encode(message);

            var buffer = Unpooled.Buffer(data.Length, data.Length);
            return buffer.WriteBytes(data);
        }
    }
}