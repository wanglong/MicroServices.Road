﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Rpc.Common.Easy.Rpc.Communally.Convertibles;
using Rpc.Common.Easy.Rpc.Communally.Entitys.Messages;
using Rpc.Common.Easy.Rpc.Runtime.Client;

namespace Rpc.Common.Easy.Rpc.ProxyGenerator.Implementation
{
    /// <summary>
    /// 一个抽象的服务代理基类
    /// </summary>
    public abstract class ServiceProxyBase
    {
        private readonly IRemoteInvokeService _remoteInvokeService;
        private readonly ITypeConvertibleService _typeConvertibleService;

        protected ServiceProxyBase(IRemoteInvokeService remoteInvokeService, ITypeConvertibleService typeConvertibleService)
        {
            _remoteInvokeService = remoteInvokeService;
            _typeConvertibleService = typeConvertibleService;
        }

        /// <summary>
        /// 远程调用
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="parameters">参数字典</param>
        /// <param name="serviceId">服务Id</param>
        /// <returns>调用结果</returns>
        protected async Task<T> Invoke<T>(IDictionary<string, object> parameters, string serviceId)
        {
            var message = await _remoteInvokeService.InvokeAsync(new RemoteInvokeContext
            {
                InvokeMessage = new RemoteInvokeMessage
                {
                    Parameters = parameters,
                    ServiceId = serviceId
                }
            });

            if (message == null) return default(T);

            var result = _typeConvertibleService.Convert(message.Result, typeof(T));

            return (T) result;
        }

        /// <summary>
        /// 远程调用
        /// </summary>
        /// <param name="parameters">参数字典</param>
        /// <param name="serviceId">服务Id</param>
        /// <returns>调用任务</returns>
        protected async Task Invoke(IDictionary<string, object> parameters, string serviceId)
        {
            await _remoteInvokeService.InvokeAsync(new RemoteInvokeContext
            {
                InvokeMessage = new RemoteInvokeMessage
                {
                    Parameters = parameters,
                    ServiceId = serviceId
                }
            });
        }
    }
}