using System.IO;
using Rpc.Common.RuntimeType.Entitys.Messages;

namespace Rpc.Common.RuntimeType.Transport.Codec
{
    public interface ITransportMessageDecoder
    {
        TransportMessage Decode(byte[] data);

        TransportMessage Decode(Stream data);

        TransportMessage Decode(string data);
    }
}