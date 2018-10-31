using Rpc.Common.Easy.Rpc.Communally.Entitys.Messages;

namespace Rpc.Common.Easy.Rpc.Runtime.Client
{
    /// <summary>
    /// 远程调用上下文
    /// </summary>
    public class RemoteInvokeContext
    {
        /// <summary>
        /// 远程调用消息
        /// </summary>
        public RemoteInvokeMessage InvokeMessage { get; set; }
    }
}