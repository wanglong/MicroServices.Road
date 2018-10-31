using System.Threading.Tasks;
using Rpc.Common.Easy.Rpc.Communally.Entitys.Messages;
using Rpc.Common.Easy.Rpc.Transport;

namespace Rpc.Common.Easy.Rpc.Runtime.Server
{
    /// <summary>
    //抽象的服务执行器
    /// </summary>
    public interface IServiceExecutor
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="sender">消息发送者</param>
        /// <param name="message">调用消息</param>
        Task ExecuteAsync(IMessageSender sender,TransportMessage message);
    }
}