using System.IO;
using Rpc.Common.Easy.Rpc.Entitys.Messages;

namespace Rpc.Common.Easy.Rpc.Transport.Codec
{
    public interface ITransportMessageDecoder
    {
        TransportMessage Decode(byte[] data);

        TransportMessage Decode(Stream data);

        TransportMessage Decode(string data);
    }
}