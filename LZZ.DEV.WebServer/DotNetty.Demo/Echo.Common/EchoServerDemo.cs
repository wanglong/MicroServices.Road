using DotNetty.Transport.Channels;

namespace Echo.Common
{
    public class EchoServerDemo : ChannelHandlerAdapter, IEchoServerDemo
    {
        public int Summing(int i1, int i2)
        {
            return i1 + i2;
        }
    }
}