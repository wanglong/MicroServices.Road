using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpc.Common.RuntimeType.Server.Impl
{
    /// <summary>
    /// 实现一个默认的Services实体对象管理器
    /// </summary>
    public class DefaultServiceEntryManager : IServiceEntryManager
    {
        private readonly IEnumerable<ServiceEntry> _serviceEntries;

        public DefaultServiceEntryManager(IEnumerable<IServiceEntryProvider> providers)
        {
            var list = new List<ServiceEntry>();
            foreach (var provider in providers)
            {
                var entries = provider.GetEntries().ToArray();
                foreach (var entry in entries)
                {
                    if (list.Any(i => i.Descriptor.Id == entry.Descriptor.Id))
                        throw new InvalidOperationException($"本地包含多个Id为：{entry.Descriptor.Id} 的服务条目。");
                }

                list.AddRange(entries);
            }

            _serviceEntries = list.ToArray();
        }

        /// <summary>
        /// 获取服务实体对象
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ServiceEntry> GetEntries()
        {
            return _serviceEntries;
        }
    }
}