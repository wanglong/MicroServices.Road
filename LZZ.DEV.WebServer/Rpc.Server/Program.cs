﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rpc.Common;
using Rpc.Common.Easy.Rpc;
using Rpc.Common.Easy.Rpc.Communally.Entitys.Address;
using Rpc.Common.Easy.Rpc.Routing;
using Rpc.Common.Easy.Rpc.Runtime.Server;

namespace Rpc.Server
{
    static class Program
    {
        static void Main()
        {
            
            // 实现自动装配
            var serviceCollection = new ServiceCollection();
            {
                serviceCollection
                    .AddLogging()
                    .AddRpcCore()
                    .AddService()
                    .UseSharedFileRouteManager("d:\\routes.txt")
                    .UseDotNettyTransport();

                // ** 注入本地测试类
                serviceCollection.AddSingleton<IUserService, UserServiceImpl>();
            }

            // 构建当前容器
            var buildServiceProvider = serviceCollection.BuildServiceProvider();

            // 获取服务管理实体类
            var serviceEntryManager = buildServiceProvider.GetRequiredService<IServiceEntryManager>();
            var addressDescriptors = serviceEntryManager.GetEntries().Select(i => new ServiceRoute
            {
                Address = new[]
                {
                    new IpAddressModel {Ip = "127.0.0.1", Port = 9881}
                },
                ServiceDescriptor = i.Descriptor
            });
            var serviceRouteManager = buildServiceProvider.GetRequiredService<IServiceRouteManager>();
            serviceRouteManager.SetRoutesAsync(addressDescriptors).Wait();

            // 构建内部日志处理
            buildServiceProvider.GetRequiredService<ILoggerFactory>().AddConsole((console, logLevel) => (int) logLevel >= 0);

            // 获取服务宿主
            var serviceHost = buildServiceProvider.GetRequiredService<IServiceHost>();

            Task.Factory.StartNew(async () =>
            {
                //启动主机
                await serviceHost.StartAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9881));
            });

            Console.ReadLine();
        }
    }
}

//                #region 注入服务相关类
//
//                {
//                    /*
//                     * 用于提供标记为RpcTagBundle的特性自动扫描到服务管理器中
//                     * 通过ServiceCollection完成自动装配
//                     */
//                    // 注入服务ID生成器
//                    serviceCollection.AddSingleton<IServiceIdGenerator, DefaultServiceIdGenerator>();
//                    // 注入服务工厂器
//                    serviceCollection.AddSingleton<IServiceEntryFactory, ServiceEntryFactory>();
//                    // 注入服务转换器
//                    serviceCollection.AddSingleton<ITypeConvertibleService, DefaultTypeConvertibleService>();
//                    // 注入服务提供者
//                    serviceCollection.AddSingleton<IServiceEntryProvider>(provider =>
//                    {
//                        // 获取当前应用程序下的程序集，并排除动态（Dynamic）方法
//                        return new AttributeServiceEntryProvider(
//                            // List<Assembly>
//                            AppDomain.CurrentDomain.GetAssemblies().Where(i => i.IsDynamic == false)
//                                .SelectMany(i => i.ExportedTypes).ToArray(),
//                            // 获取的服务工厂器
//                            provider.GetRequiredService<IServiceEntryFactory>(),
//                            // 获取日志
//                            provider.GetRequiredService<ILogger<AttributeServiceEntryProvider>>()
//                        );
//                    });
//                    // 注入服务管理器
//                    serviceCollection.AddSingleton<IServiceEntryManager, DefaultServiceEntryManager>();
//                }
//
//                #endregion
//
//                #region 注入服务宿主和方法执行者
//
//                {
//                    /*
//                     * 用于提供本地服务宿主和服务管理器中的方法执行者
//                     * 通过ServiceCollection完成自动装配
//                     */
//                    // 注入服务执行者
//                    serviceCollection.AddSingleton<IServiceExecutor, DefaultServiceExecutor>();
//                    // 注入DotNetty消息监听器
//                    serviceCollection.AddSingleton<DefaultDotNettyServerMessageListener>();
//                    // 注入服务定位器
//                    serviceCollection.AddSingleton<IServiceEntryLocate, DefaultServiceEntryLocate>();
//                    // 注入服务执行者
//                    serviceCollection.AddSingleton<IServiceExecutor, DefaultServiceExecutor>();
//                    // 注入Json消息传输处理器
//                    serviceCollection.AddSingleton<ITransportMessageCodecFactory, JsonTransportMessageCodecFactory>();
//                    // 注入默认服务宿主
//                    serviceCollection.AddSingleton<IServiceHost, DefaultServiceHost>(
//                        provider => new DefaultServiceHost(
//                            async endPoint =>
//                            {
//                                var messageListener =
//                                    provider.GetRequiredService<DefaultDotNettyServerMessageListener>();
//                                await messageListener.StartAsync(endPoint);
//                                return messageListener;
//                            },
//                            provider.GetRequiredService<IServiceExecutor>()
//                        )
//                    );
//                }
//
//                #endregion