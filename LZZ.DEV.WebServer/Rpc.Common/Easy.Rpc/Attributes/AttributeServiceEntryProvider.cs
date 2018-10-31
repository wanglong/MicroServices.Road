using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Rpc.Common.Easy.Rpc.Communally.Entitys;
using Rpc.Common.Easy.Rpc.Runtime.Server;

namespace Rpc.Common.Easy.Rpc.Attributes
{
    /// <summary>
    /// 特性提供者实现类
    /// </summary>
    public class AttributeServiceEntryProvider : IServiceEntryProvider
    {
        private readonly IEnumerable<Type> _types;
        private readonly IServiceEntryFactory _serviceEntryFactory;
        private readonly ILogger<AttributeServiceEntryProvider> _logger;

        public AttributeServiceEntryProvider(IEnumerable<Type> types, IServiceEntryFactory serviceEntryFactory, ILogger<AttributeServiceEntryProvider> logger)
        {
            _types = types;
            _serviceEntryFactory = serviceEntryFactory;
            _logger = logger;
        }

        public IEnumerable<ServiceEntity> GetEntries()
        {
            // 获取程序集中服务接口
            // 限定为打上RpcTagBundleAttribute特性的接口
            var services = _types.Where(type =>
                {
                    var typeInfo = type.GetTypeInfo();
                    // 限定为打上RpcTagBundleAttribute特性的接口
                    return typeInfo.IsInterface && typeInfo.GetCustomAttribute<RpcTagBundleAttribute>() != null;
                }
            ).ToArray();

            // 获取程序集中所有服务接口实现类
            // 排除系统(System)和(Microsoft)为命名空间的程序集|当然还可以追加更多系统命名空间名排除
            var serviceImplementations = _types.Where(type =>
                {
                    var typeInfo = type.GetTypeInfo();
                    return typeInfo.IsClass
                           && !typeInfo.IsAbstract
                           && type.Namespace != null
                           && !type.Namespace.StartsWith("System")
                           && !type.Namespace.StartsWith("Microsoft")
                           && !type.Namespace.StartsWith("Autofac")
                           && !type.Namespace.StartsWith("Internal");
                }
            ).ToArray();

            var entries = new List<ServiceEntity>();
            foreach (var service in services)
            {
                foreach (var serviceImplementation in serviceImplementations.Where(i =>
                    service.GetTypeInfo().IsAssignableFrom(i)))
                {
                    entries.AddRange(_serviceEntryFactory.CreateServiceEntry(service, serviceImplementation));
                }
            }

            return entries;
        }
    }
}