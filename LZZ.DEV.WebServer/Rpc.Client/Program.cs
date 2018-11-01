using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rpc.Common;
using Rpc.Common.Easy.Rpc;
using Rpc.Common.Easy.Rpc.ProxyGenerator;

namespace Rpc.Client
{
    static class Program
    {
        static void Main()
        {
            var serviceCollection = new ServiceCollection();
            {
//                // 注入日志
//                serviceCollection.AddLogging();
//                // 注入ID生成器
//                serviceCollection.AddSingleton<IServiceIdGenerator, DefaultServiceIdGenerator>();
//                // 注入类型转换器
//                serviceCollection.AddSingleton<ITypeConvertibleService, DefaultTypeConvertibleService>();
//                // 注入健康状态检查
//                serviceCollection.AddSingleton<IHealthCheckService, DefaultHealthCheckService>();
//                // 注入地址解析器
//                serviceCollection.AddSingleton<IAddressResolver, DefaultAddressResolver>();
//                // 注入远程调用服务
//                serviceCollection.AddSingleton<IRemoteInvokeService, RemoteInvokeService>();
//                // 注入客户端传输工厂服务
//                serviceCollection.AddSingleton<ITransportClientFactory, DefaultDotNettyTransportClientFactory>();
//                // 注入服务代理生成器
//                serviceCollection.AddSingleton<IServiceProxyGenerater, ServiceProxyGenerater>();
//                // 注入服务代理工厂
//                serviceCollection.AddSingleton<IServiceProxyFactory, ServiceProxyFactory>();
//                // 注入DotNetty服务监听者
//                serviceCollection.AddSingleton<DefaultDotNettyServerMessageListener>();
//                // 注入默认宿主
//                serviceCollection.AddSingleton<IServiceHost, DefaultServiceHost>(
//                    provider => new DefaultServiceHost(
//                        async endPoint =>
//                        {
//                            var messageListener = provider.GetRequiredService<DefaultDotNettyServerMessageListener>();
//                            await messageListener.StartAsync(endPoint);
//                            return messageListener;
//                        },
//                        provider.GetRequiredService<IServiceExecutor>()
//                    )
//                );


                serviceCollection
                    .AddLogging() // 添加日志
                    .AddClient() // 添加客户端
                    .UseSharedFileRouteManager(@"d:\routes.txt") // 添加共享路由
                    .UseDotNettyTransport(); // 添加DotNetty通信传输
            }

            var serviceProvider = serviceCollection.BuildServiceProvider();

            serviceProvider.GetRequiredService<ILoggerFactory>().AddConsole((console, logLevel) => (int) logLevel >= 0);

            var services = serviceProvider.GetRequiredService<IServiceProxyGenerater>()
                .GenerateProxys(new[] {typeof(IUserService)}).ToArray();

            var userService = serviceProvider.GetRequiredService<IServiceProxyFactory>().CreateProxy<IUserService>(
                services.Single(typeof(IUserService).GetTypeInfo().IsAssignableFrom)
            );

            while (true)
            {
                Task.Run(async () =>
                {
                    Console.WriteLine($"userService.GetUserName:{await userService.GetUserName(1)}");
                    Console.WriteLine($"userService.GetUserId:{await userService.GetUserId("rabbit")}");
                    Console.WriteLine($"userService.GetUserLastSignInTime:{await userService.GetUserLastSignInTime(1)}");
                    var user = await userService.GetUser(1);
                    Console.WriteLine($"userService.GetUser:name={user.Name},age={user.Age}");
                    Console.WriteLine($"userService.Update:{await userService.Update(1, user)}");
                    Console.WriteLine($"userService.GetDictionary:{(await userService.GetDictionary())["key"]}");
                    await userService.Try();
                    Console.WriteLine("client function completed！");
                }).Wait();
                Console.ReadKey();
            }
        }
    }
}