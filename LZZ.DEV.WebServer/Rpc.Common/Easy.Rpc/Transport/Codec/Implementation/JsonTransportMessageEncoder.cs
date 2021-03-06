﻿using System.Text;
using Newtonsoft.Json;
using Rpc.Common.Easy.Rpc.Communally.Entitys.Messages;

namespace Rpc.Common.Easy.Rpc.Transport.Codec.Implementation
{
    public sealed class JsonTransportMessageEncoder : ITransportMessageEncoder
    {
        public byte[] Encode(TransportMessage message)
        {
            var content = JsonConvert.SerializeObject(message);
            return Encoding.UTF8.GetBytes(content);
        }
    }
}