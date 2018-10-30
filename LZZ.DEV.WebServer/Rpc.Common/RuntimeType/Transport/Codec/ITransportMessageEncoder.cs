using Rpc.Common.RuntimeType.Entitys.Messages;

namespace Rpc.Common.RuntimeType.Transport.Codec
{
    public interface ITransportMessageEncoder
    {
        byte[] Encode(TransportMessage message);
    }
}