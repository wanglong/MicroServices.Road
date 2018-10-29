using System.Collections.Generic;

namespace Rpc.Common.RuntimeType.Server
{
    /// <summary>
    /// 一个抽象的服务条目提供程序。
    /// </summary>
    public interface IServiceEntryProvider
    {
        /// <summary>
        /// 获取服务条目集合。
        /// </summary>
        /// <returns>服务条目集合。</returns>
        IEnumerable<ServiceEntry> GetEntries();
    }
}