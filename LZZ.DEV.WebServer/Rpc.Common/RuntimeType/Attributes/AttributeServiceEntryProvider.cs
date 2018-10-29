using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rpc.Common.RuntimeType.Server;

namespace Rpc.Common.RuntimeType.Attributes
{
    public class AttributeServiceEntryProvider : IServiceEntryProvider
    {
        private readonly IEnumerable<Type> _types;
        private readonly IClrServiceEntryFactory _clrServiceEntryFactory;


        public AttributeServiceEntryProvider(IEnumerable<Type> types, IClrServiceEntryFactory clrServiceEntryFactory
//            ,ILogger<AttributeServiceEntryProvider> logger
        )
        {
            _types = types;
            _clrServiceEntryFactory = clrServiceEntryFactory;
        }

        public IEnumerable<ServiceEntry> GetEntries()
        {
            var services = _types.Where(i =>
            {
                var typeInfo = i.GetTypeInfo();
                return typeInfo.IsInterface && typeInfo.GetCustomAttribute<RpcTargetBundleAttribute>() != null;
            }).ToArray();
            var serviceImplementations = _types.Where(i =>
            {
                var typeInfo = i.GetTypeInfo();
                return typeInfo.IsClass && !typeInfo.IsAbstract && i.Namespace != null &&
                       !i.Namespace.StartsWith("System") &&
                       !i.Namespace.StartsWith("Microsoft");
            }).ToArray();

            var entries = new List<ServiceEntry>();
            foreach (var service in services)
            {
                foreach (var serviceImplementation in serviceImplementations.Where(i =>
                    service.GetTypeInfo().IsAssignableFrom(i)))
                {
                    entries.AddRange(_clrServiceEntryFactory.CreateServiceEntry(service, serviceImplementation));
                }
            }

            return entries;
        }
    }
}