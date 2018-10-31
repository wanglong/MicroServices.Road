using System.Net;
using System.Threading.Tasks;
using Rpc.Common.Easy.Rpc.Transport;
using Rpc.Common.Easy.Rpc.Transport.Impl;

namespace Rpc.Common.Easy.Rpc.Runtime.Server.Impl
{
    /// <summary>
    /// 服务主机基类
    /// </summary>
    public abstract class ServiceHostAbstract : IServiceHost
    {
        /// <summary>
        /// 消息监听者
        /// </summary>
        protected IMessageListener MessageListener { get; } = new MessageListener();

        protected ServiceHostAbstract(IServiceExecutor serviceExecutor)
        {
            MessageListener.Received += async (sender, message) =>
            {
                await serviceExecutor.ExecuteAsync(sender, message);
            };
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public abstract void Dispose();

        /// <summary>
        /// 启动主机
        /// </summary>
        /// <param name="endPoint">主机终结点</param>
        /// <returns>一个任务</returns>
        public abstract Task StartAsync(EndPoint endPoint);
    }
}