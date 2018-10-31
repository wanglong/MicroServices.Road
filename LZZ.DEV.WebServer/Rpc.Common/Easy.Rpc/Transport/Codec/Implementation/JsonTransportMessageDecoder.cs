using System.IO;
using System.Text;
using Newtonsoft.Json;
using Rpc.Common.Easy.Rpc.Communally.Entitys.Messages;

namespace Rpc.Common.Easy.Rpc.Transport.Codec.Implementation
{
    public sealed class JsonTransportMessageDecoder : ITransportMessageDecoder
    {
        public TransportMessage Decode(byte[] data)
        {
            var content = Encoding.UTF8.GetString(data);
            return InvokeMessage(JsonConvert.DeserializeObject<TransportMessage>(content));
        }

        public TransportMessage Decode(Stream data)
        {
            using (var reader = new StreamReader(data))
            {
                var content = reader.ReadToEnd();
                return InvokeMessage(JsonConvert.DeserializeObject<TransportMessage>(content));
            }
        }

        public TransportMessage Decode(string data)
        {
            return InvokeMessage(JsonConvert.DeserializeObject<TransportMessage>(data));
        }

        private TransportMessage InvokeMessage(TransportMessage message)
        {
            if (message.IsInvokeMessage())
            {
                message.Content = JsonConvert.DeserializeObject<RemoteInvokeMessage>(message.Content.ToString());
            }

            if (message.IsInvokeResultMessage())
            {
                message.Content = JsonConvert.DeserializeObject<RemoteInvokeResultMessage>(message.Content.ToString());
            }

            return message;
        }
    }
}