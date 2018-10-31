﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rpc.Common.Easy.Rpc.Communally.Entitys.Address;
using Rpc.Common.Easy.Rpc.Communally.Serialization;

namespace Rpc.Common.Easy.Rpc.Routing.Implementation
{
    /// <summary>
    /// 默认的服务路由工厂实现
    /// </summary>
    public class DefaultServiceRouteFactory : IServiceRouteFactory
    {
        private readonly ISerializer<string> _serializer;

        public DefaultServiceRouteFactory(ISerializer<string> serializer)
        {
            _serializer = serializer;
        }

        /// <summary>
        /// 根据服务路由描述符创建服务路由
        /// </summary>
        /// <param name="descriptors">服务路由描述符</param>
        /// <returns>服务路由集合</returns>
        public Task<IEnumerable<ServiceRoute>> CreateServiceRoutesAsync(IEnumerable<ServiceRouteDescriptor> descriptors)
        {
            if (descriptors == null) throw new ArgumentNullException(nameof(descriptors));

            descriptors = descriptors.ToArray();
            var routes = new List<ServiceRoute>(descriptors.Count());
            routes.AddRange(descriptors.Select(descriptor => new ServiceRoute
            {
                Address = CreateAddress(descriptor.AddressDescriptors),
                ServiceDescriptor = descriptor.ServiceDescriptor
            }));

            return Task.FromResult(routes.AsEnumerable());
        }

        private IEnumerable<AddressModel> CreateAddress(IEnumerable<ServiceAddressDescriptor> descriptors)
        {
            if (descriptors == null)
                yield break;

            foreach (var descriptor in descriptors)
            {
                var addressType = Type.GetType(descriptor.Type);
                yield return (AddressModel) _serializer.Deserialize(descriptor.Value, addressType);
            }
        }
    }
}