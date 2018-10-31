using System.Collections.Generic;
using Rpc.Common.Easy.Rpc.Communally.Entitys;

namespace Rpc.Common.Easy.Rpc.Runtime.Server
{
    /// <summary>
    //抽象的服务条目提供程序
    /// </summary>
    public interface IServiceEntryProvider
    {
        /// <summary>
        /// 获取服务条目集合
        /// </summary>
        /// <returns>服务条目集合</returns>
        IEnumerable<ServiceEntity> GetEntries();
    }
}