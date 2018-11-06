using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Rpc.Common.Easy.Rpc.Attributes;
using Rpc.Common.Easy.Rpc.Communally.Convertibles;
using Rpc.Common.Easy.Rpc.Communally.Convertibles.Impl;
using Rpc.Common.Easy.Rpc.Communally.IdGenerator;
using Rpc.Common.Easy.Rpc.Communally.IdGenerator.Impl;
using Rpc.Common.Easy.Rpc.Communally.Serialization;
using Rpc.Common.Easy.Rpc.Communally.Serialization.Implementation;
using Rpc.Common.Easy.Rpc.ProxyGenerator;
using Rpc.Common.Easy.Rpc.ProxyGenerator.Implementation;
using Rpc.Common.Easy.Rpc.Routing;
using Rpc.Common.Easy.Rpc.Routing.Impl;
using Rpc.Common.Easy.Rpc.Runtime.Client;
using Rpc.Common.Easy.Rpc.Runtime.Client.Address.Resolvers;
using Rpc.Common.Easy.Rpc.Runtime.Client.Address.Resolvers.Implementation;
using Rpc.Common.Easy.Rpc.Runtime.Client.Address.Resolvers.Implementation.Selectors;
using Rpc.Common.Easy.Rpc.Runtime.Client.Address.Resolvers.Implementation.Selectors.Implementation;
using Rpc.Common.Easy.Rpc.Runtime.Client.HealthChecks;
using Rpc.Common.Easy.Rpc.Runtime.Client.HealthChecks.Implementation;
using Rpc.Common.Easy.Rpc.Runtime.Client.Implementation;
using Rpc.Common.Easy.Rpc.Runtime.Server;
using Rpc.Common.Easy.Rpc.Runtime.Server.Impl;
using Rpc.Common.Easy.Rpc.Transport;
using Rpc.Common.Easy.Rpc.Transport.Codec;
using Rpc.Common.Easy.Rpc.Transport.Codec.Implementation;
using Rpc.Common.Easy.Rpc.Transport.Impl;

namespace Rpc.Common.Easy.Rpc
{
    public static class RpcServiceCollectionExtensions
    {
        /// <summary>
        /// 添加Json序列化支持
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder AddJsonSerialization(this IRpcBuilder builder)
        {
            var services = builder.Services;

            services.AddSingleton<ISerializer<string>, JsonSerializer>();
            services.AddSingleton<ISerializer<byte[]>, StringByteArraySerializer>();
            services.AddSingleton<ISerializer<object>, StringObjectSerializer>();

            return builder;
        }

        #region RouteManager

        /// <summary>
        /// 设置服务路由管理者
        /// </summary>
        /// <typeparam name="T">服务路由管理者实现</typeparam>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseRouteManager<T>(this IRpcBuilder builder) where T : class, IServiceRouteManager
        {
            builder.Services.AddSingleton<IServiceRouteManager, T>();
            return builder;
        }

        /// <summary>
        /// 设置服务路由管理者
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <param name="factory">服务路由管理者实例工厂</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseRouteManager(this IRpcBuilder builder, Func<IServiceProvider, IServiceRouteManager> factory)
        {
            builder.Services.AddSingleton(factory);
            return builder;
        }

        /// <summary>
        /// 设置服务路由管理者
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <param name="instance">服务路由管理者实例</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseRouteManager(this IRpcBuilder builder, IServiceRouteManager instance)
        {
            builder.Services.AddSingleton(instance);
            return builder;
        }

        #endregion RouteManager

        /// <summary>
        /// 设置共享文件路由管理者
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseSharedFileRouteManager(this IRpcBuilder builder, string filePath)
        {
            return builder.UseRouteManager(provider =>
                new SharedFileServiceRouteManager(
                    filePath,
                    provider.GetRequiredService<ISerializer<string>>(),
                    provider.GetRequiredService<IServiceRouteFactory>(),
                    provider.GetRequiredService<ILogger<SharedFileServiceRouteManager>>()
                ));
        }

        #region AddressSelector

        /// <summary>
        /// 设置服务地址选择器
        /// </summary>
        /// <typeparam name="T">地址选择器实现类型</typeparam>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseAddressSelector<T>(this IRpcBuilder builder) where T : class, IAddressSelector
        {
            builder.Services.AddSingleton<IAddressSelector, T>();
            return builder;
        }

        /// <summary>
        /// 设置服务地址选择器
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <param name="factory">服务地址选择器实例工厂</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseAddressSelector(this IRpcBuilder builder,
            Func<IServiceProvider, IAddressSelector> factory)
        {
            builder.Services.AddSingleton(factory);

            return builder;
        }

        /// <summary>
        /// 设置服务地址选择器
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <param name="instance">地址选择器实例</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseAddressSelector(this IRpcBuilder builder, IAddressSelector instance)
        {
            builder.Services.AddSingleton(instance);

            return builder;
        }

        #endregion AddressSelector

        /// <summary>
        /// 使用轮询的地址选择器
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UsePollingAddressSelector(this IRpcBuilder builder)
        {
            builder.Services.AddSingleton<IAddressSelector, PollingAddressSelector>();
            return builder;
        }

        /// <summary>
        /// 使用随机的地址选择器
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseRandomAddressSelector(this IRpcBuilder builder)
        {
            builder.Services.AddSingleton<IAddressSelector, RandomAddressSelector>();
            return builder;
        }

        #region Codec Factory

        /// <summary>
        /// 使用编解码器
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <param name="codecFactory"></param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseCodec(this IRpcBuilder builder, ITransportMessageCodecFactory codecFactory)
        {
            builder.Services.AddSingleton(codecFactory);
            return builder;
        }

        /// <summary>
        /// 使用编解码器
        /// </summary>
        /// <typeparam name="T">编解码器工厂实现类型</typeparam>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseCodec<T>(this IRpcBuilder builder) where T : class, ITransportMessageCodecFactory
        {
            builder.Services.AddSingleton<ITransportMessageCodecFactory, T>();
            return builder;
        }

        /// <summary>
        /// 使用编解码器
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <param name="codecFactory">编解码器工厂创建委托</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseCodec(this IRpcBuilder builder,
            Func<IServiceProvider, ITransportMessageCodecFactory> codecFactory)
        {
            builder.Services.AddSingleton(codecFactory);

            return builder;
        }

        #endregion Codec Factory

        /// <summary>
        /// 使用Json编解码器
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseJsonCodec(this IRpcBuilder builder)
        {
            return builder.UseCodec<JsonTransportMessageCodecFactory>();
        }

        /// <summary>
        /// 添加客户端运行时服务
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder AddClientRuntime(this IRpcBuilder builder)
        {
            var services = builder.Services;

            services.AddSingleton<IHealthCheckService, DefaultHealthCheckService>();
            services.AddSingleton<IAddressResolver, DefaultAddressResolver>();
            services.AddSingleton<IRemoteInvokeService, RemoteInvokeService>();

            return builder.UsePollingAddressSelector();
        }

        /// <summary>
        /// 添加服务运行时服务
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder AddService(this IRpcBuilder builder)
        {
            var services = builder.Services;

            services.AddSingleton<IServiceEntryFactory, ServiceEntryFactory>();
            services.AddSingleton<IServiceEntryProvider>(provider =>
            {
                var assemblys = DependencyContext.Default.RuntimeLibraries.SelectMany(i =>
                    i.GetDefaultAssemblyNames(DependencyContext.Default).Select(z => Assembly.Load(new AssemblyName(z.Name))));

                var types = assemblys.Where(i => i.IsDynamic == false).SelectMany(i => i.ExportedTypes).ToArray();

                return new AttributeServiceEntryProvider(types, provider.GetRequiredService<IServiceEntryFactory>(),
                    provider.GetRequiredService<ILogger<AttributeServiceEntryProvider>>());
            });
            services.AddSingleton<IServiceEntryManager, DefaultServiceEntryManager>();
            services.AddSingleton<IServiceEntryLocate, DefaultServiceEntryLocate>();
            services.AddSingleton<IServiceExecutor, DefaultServiceExecutor>();

            return builder;
        }

        /// <summary>
        /// 添加RPC核心服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder AddRpcCore(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IServiceIdGenerator, DefaultServiceIdGenerator>();

            services.AddSingleton<ITypeConvertibleProvider, DefaultTypeConvertibleProvider>();
            services.AddSingleton<ITypeConvertibleService, DefaultTypeConvertibleService>();

            services.AddSingleton<IServiceRouteFactory, DefaultServiceRouteFactory>();

            return new RpcBuilder(services)
                .AddJsonSerialization()
                .UseJsonCodec();
        }

        public static IRpcBuilder AddClientProxy(this IRpcBuilder builder)
        {
            var services = builder.Services;

            services.AddSingleton<IServiceProxyGenerater, ServiceProxyGenerater>();
            services.AddSingleton<IServiceProxyFactory, ServiceProxyFactory>();

            return builder;
        }

        public static IRpcBuilder AddClient(this IServiceCollection services)
        {
            return services
                .AddRpcCore()
                .AddClientRuntime()
                .AddClientProxy();
        }

        /// <summary>
        /// 使用DotNetty进行传输。
        /// </summary>
        /// <param name="builder">Rpc服务构建者。</param>
        /// <returns>Rpc服务构建者。</returns>
        public static IRpcBuilder UseDotNettyTransport(this IRpcBuilder builder)
        {
            var services = builder.Services;

            services.AddSingleton<ITransportClientFactory, DefaultDotNettyTransportClientFactory>();

            services.AddSingleton<DefaultDotNettyServerMessageListener>();

            services.AddSingleton<IServiceHost, DefaultServiceHost>(
                provider => new DefaultServiceHost(
                    async endPoint =>
                    {
                        var messageListener = provider.GetRequiredService<DefaultDotNettyServerMessageListener>();
                        await messageListener.StartAsync(endPoint);
                        return messageListener;
                    },
                    provider.GetRequiredService<IServiceExecutor>()
                )
            );

            return builder;
        }
    }
}