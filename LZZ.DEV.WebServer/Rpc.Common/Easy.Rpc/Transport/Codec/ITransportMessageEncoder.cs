using System.IO;
using Rpc.Common.Easy.Rpc.Entitys.Messages;

namespace Rpc.Common.Easy.Rpc.Transport.Codec
{
    public interface ITransportMessageEncoder
    {
        byte[] Encode(TransportMessage message);
        
        Stream EncodeStream(TransportMessage message);
    }
}