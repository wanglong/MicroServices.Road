using Microsoft.Extensions.DependencyInjection;
using Rpc.Common.Easy.Rpc.Communally.Convertibles;
using Rpc.Common.Easy.Rpc.Communally.Convertibles.Impl;
using Rpc.Common.Easy.Rpc.Communally.IdGenerator;
using Rpc.Common.Easy.Rpc.Communally.IdGenerator.Impl;
using Rpc.Common.Easy.Rpc.Runtime.Client;
using Rpc.Common.Easy.Rpc.Runtime.Client.Address.Resolvers;
using Rpc.Common.Easy.Rpc.Runtime.Client.Address.Resolvers.Implementation;
using Rpc.Common.Easy.Rpc.Runtime.Client.HealthChecks;
using Rpc.Common.Easy.Rpc.Runtime.Client.HealthChecks.Implementation;
using Rpc.Common.Easy.Rpc.Runtime.Client.Implementation;

namespace Rpc.Client
{
    static class Program
    {
        static void Main()
        {
            var serviceCollection = new ServiceCollection();
            {
                serviceCollection.AddLogging();
                // 注入ID生成器
                serviceCollection.AddSingleton<IServiceIdGenerator, DefaultServiceIdGenerator>();
                // 注入类型转换器
                serviceCollection.AddSingleton<ITypeConvertibleService, DefaultTypeConvertibleService>();
                // 注入健康状态检查
                serviceCollection.AddSingleton<IHealthCheckService, DefaultHealthCheckService>();
                // 注入地址解析器
                serviceCollection.AddSingleton<IAddressResolver, DefaultAddressResolver>();
                // 注入远程调用服务
                serviceCollection.AddSingleton<IRemoteInvokeService, RemoteInvokeService>();
            }
        }
    }
}