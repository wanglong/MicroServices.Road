using System.IO;
using Rpc.Common.Easy.Rpc.Communally.Entitys.Messages;

namespace Rpc.Common.Easy.Rpc.Transport.Codec
{
    public interface ITransportMessageEncoder
    {
        byte[] Encode(TransportMessage message);
    }
}