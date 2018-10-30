using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
using Rpc.Common.RuntimeType.Transport.Codec.Implementation;

namespace Rpc.Server
{
    static class Program
    {
        static void Main()
        {
            // 实现自动装配
            var serviceCollection = new ServiceCollection();
            {
                #region 注入服务相关类，用于提供

                {
                    // 注入默认服务工厂
                    serviceCollection.AddSingleton<ITypeConvertibleService, DefaultTypeConvertibleService>();
                    // 注入服务ID生成器
                    serviceCollection.AddSingleton<IServiceIdGenerator, DefaultServiceIdGenerator>();
                    // 注入服务工厂器
                    serviceCollection.AddSingleton<IServiceEntryFactory, ServiceEntryFactory>();
                    // 注入服务转换器
                    serviceCollection.AddSingleton<ITypeConvertibleService, DefaultTypeConvertibleService>();
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

                #region 注入服务宿主和方法执行者

                {
                    // 注入服务执行者
                    serviceCollection.AddSingleton<IServiceExecutor, DefaultServiceExecutor>();
                    // 注入DotNetty消息监听器
                    serviceCollection.AddSingleton<DotNettyServerMessageListener>();
                    // 注入服务定位器
                    serviceCollection.AddSingleton<IServiceEntryLocate, DefaultServiceEntryLocate>();
                    // 注入服务执行者
                    serviceCollection.AddSingleton<IServiceExecutor, DefaultServiceExecutor>();
                    // 注入Json消息传输处理器
                    serviceCollection.AddSingleton<ITransportMessageCodecFactory, JsonTransportMessageCodecFactory>();
                    // 注入默认服务宿主
                    serviceCollection.AddSingleton<IServiceHost, DefaultServiceHost>(
                        provider => new DefaultServiceHost(
                            async endPoint =>
                            {
                                var messageListener = provider.GetRequiredService<DotNettyServerMessageListener>();
                                await messageListener.StartAsync(endPoint);
                                return messageListener;
                            },
                            provider.GetRequiredService<IServiceExecutor>()
                        )
                    );
                }

                #endregion

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

            // 获取服务管理
            var serviceHost = buildServiceProvider.GetRequiredService<IServiceHost>();
            Task.Factory.StartNew(async () =>
            {
                //启动主机
                serviceHost.StartAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9981)).Wait();
                Console.WriteLine($"服务端启动成功，{DateTime.Now}。");
            });
            Console.WriteLine("press enter key to exit");
            Console.ReadLine();
        }
    }
}