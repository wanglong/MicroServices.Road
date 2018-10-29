using System;

namespace Rpc.Common.RuntimeType.Attributes
{
    /// <summary>
    /// Rpc服务描述符标记。
    /// </summary>
    public abstract class RpcServiceDescriptorAttribute : Attribute
    {
        /// <summary>
        /// 应用标记。
        /// </summary>
        /// <param name="descriptor">服务描述符。</param>
        public abstract void Apply(ServiceDescriptor descriptor);
    }
}