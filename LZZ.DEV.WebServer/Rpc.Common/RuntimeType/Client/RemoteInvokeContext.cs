using Rpc.Common.RuntimeType.Entitys.Messages;

namespace Rpc.Common.RuntimeType.Client
{
    /// <summary>
    /// 远程调用上下文。
    /// </summary>
    public class RemoteInvokeContext
    {
        /// <summary>
        /// 远程调用消息。
        /// </summary>
        public RemoteInvokeMessage InvokeMessage { get; set; }
    }
}