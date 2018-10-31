﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Rpc.Common.Easy.Rpc.Communally.Entitys;
using Rpc.Common.Easy.Rpc.Communally.Entitys.Address;

namespace Rpc.Common.Easy.Rpc.Runtime.Client.Address.Resolvers.Implementation.Selectors
{
    /// <summary>
    /// 地址选择上下文
    /// </summary>
    public class AddressSelectContext
    {
        /// <summary>
        /// 服务描述符
        /// </summary>
        public ServiceDescriptor Descriptor { get; set; }

        /// <summary>
        /// 服务可用地址
        /// </summary>
        public IEnumerable<AddressModel> Address { get; set; }
    }

    /// <summary>
    //抽象的地址选择器
    /// </summary>
    public interface IAddressSelector
    {
        /// <summary>
        /// 选择一个地址
        /// </summary>
        /// <param name="context">地址选择上下文</param>
        /// <returns>地址模型</returns>
        Task<AddressModel> SelectAsync(AddressSelectContext context);
    }
}