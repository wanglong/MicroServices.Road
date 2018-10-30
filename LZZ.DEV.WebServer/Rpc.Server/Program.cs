using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Rpc.Common;
using Rpc.Common.RuntimeType.Attributes;
using Rpc.Common.RuntimeType.Communally.Convertibles;
using Rpc.Common.RuntimeType.Communally.Convertibles.Impl;
using Rpc.Common.RuntimeType.Communally.IdGenerator;
using Rpc.Common.RuntimeType.Communally.IdGenerator.Impl;
using Rpc.Common.RuntimeType.Server;
using Rpc.Common.RuntimeType.Server.Impl;
using Rpc.Common.RuntimeType.Transport;
using Rpc.Common.RuntimeType.Transport.Codec;

namespace Rpc.Server
{
    static class Program
    {
        static void Main()
        {
            // 实现自动装配
            var serviceCollection = new ServiceCollection();
            {
                #region 注入服务相关类

                {
                    // 注入默认服务工厂
                    serviceCollection.AddSingleton<ITypeConvertibleService, DefaultTypeConvertibleService>();
                    // 注入服务ID生成器
                    serviceCollection.AddSingleton<IServiceIdGenerator, DefaultServiceIdGenerator>();
                    // 注入服务工厂器
                    serviceCollection.AddSingleton<IServiceEntryFactory, ServiceEntryFactory>();
                    // 注入服务提供者
                    serviceCollection.AddSingleton<IServiceEntryProvider>(provider =>
                    {
                        // 获取当前应用程序下的程序集，并排除动态（Dynamic）方法
                        return new AttributeServiceEntryProvider(
                            // List<Assembly>
                            AppDomain.CurrentDomain.GetAssemblies()
                                .Where(i => i.IsDynamic == false)
                                .SelectMany(i => i.ExportedTypes)
                                .ToArray(),
                            // 获取的服务工厂器
                            provider.GetRequiredService<IServiceEntryFactory>()
                        );
                    });
                    // 注入服务管理器
                    serviceCollection.AddSingleton<IServiceEntryManager, DefaultServiceEntryManager>();
                }

                #endregion

                {
//                    serviceCollection.AddSingleton<ITransportMessageCodecFactory, DotNettyTransportClientFactory>();
                    serviceCollection.AddSingleton<DotNettyServerMessageListener>();
                }
                // ** 注入本地测试类
                serviceCollection.AddSingleton<IUserService, UserServiceImpl>();
            }

            // 构建当前容器
            var buildServiceProvider = serviceCollection.BuildServiceProvider();

            // 获取服务管理实体类
            var serviceEntryManager = buildServiceProvider.GetRequiredService<IServiceEntryManager>();

            // 获取所有打上RpcTargetBundle特性的服务实体
            foreach (var entry in serviceEntryManager.GetEntries())
            {
                Console.WriteLine($"Id: {entry.Descriptor.Id}");
            }
        }
    }
}