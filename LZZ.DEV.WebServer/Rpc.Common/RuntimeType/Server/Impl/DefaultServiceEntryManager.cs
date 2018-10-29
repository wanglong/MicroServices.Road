using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpc.Common.RuntimeType.Server.Impl
{
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


        public IEnumerable<ServiceEntry> GetEntries()
        {
            return _serviceEntries;
        }
    }
}