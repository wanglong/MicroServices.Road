using Rpc.Common.RuntimeType.Entitys.Messages;

namespace Rpc.Common.RuntimeType.Transport.Codec
{
    public interface ITransportMessageDecoder
    {
        TransportMessage Decode(byte[] data);
    }
}