using Microsoft.Extensions.DependencyInjection;

namespace Rpc.Common.Easy.Rpc
{
    /// <summary>
    /// 一个抽象的Rpc服务构建者
    /// </summary>
    public interface IRpcBuilder
    {
        /// <summary>
        /// 服务集合
        /// </summary>
        IServiceCollection Services { get; }
    }
}