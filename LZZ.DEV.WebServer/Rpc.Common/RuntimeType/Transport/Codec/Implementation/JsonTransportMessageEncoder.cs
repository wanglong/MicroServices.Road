using System.Text;
using Newtonsoft.Json;
using Rpc.Common.RuntimeType.Entitys.Messages;

namespace Rpc.Common.RuntimeType.Transport.Codec.Implementation
{
    public sealed class JsonTransportMessageEncoder : ITransportMessageEncoder
    {
        #region Implementation of ITransportMessageEncoder

        public byte[] Encode(TransportMessage message)
        {
            var content = JsonConvert.SerializeObject(message);
            return Encoding.UTF8.GetBytes(content);
        }

        #endregion Implementation of ITransportMessageEncoder
    }
}