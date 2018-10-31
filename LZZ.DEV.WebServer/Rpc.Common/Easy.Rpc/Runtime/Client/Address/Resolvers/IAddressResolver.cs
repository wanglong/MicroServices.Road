﻿using System.Threading.Tasks;
using Rpc.Common.Easy.Rpc.Communally.Entitys.Address;

namespace Rpc.Common.Easy.Rpc.Runtime.Client.Address.Resolvers
{
    /// <summary>
    /// 抽象的服务地址解析器
    /// </summary>
    public interface IAddressResolver
    {
        /// <summary>
        /// 解析服务地址
        /// </summary>
        /// <param name="serviceId">服务Id</param>
        /// <returns>服务地址模型</returns>
        Task<AddressModel> Resolver(string serviceId);
    }
}