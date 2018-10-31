using DotNetty.Buffers;
using Rpc.Common.Easy.Rpc.Communally.Entitys.Messages;
using Rpc.Common.Easy.Rpc.Transport.Codec;

namespace Rpc.Common.Easy.Rpc.Transport.Impl
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