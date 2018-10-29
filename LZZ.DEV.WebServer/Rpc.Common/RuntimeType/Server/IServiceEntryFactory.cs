﻿using System;
using System.Collections.Generic;
using Rpc.Common.RuntimeType.Entitys;

namespace Rpc.Common.RuntimeType.Server
{
    /// <summary>
    /// 一个抽象的服务条目工厂。
    /// </summary>
    public interface IServiceEntryFactory
    {
        /// <summary>
        /// 创建服务条目。
        /// </summary>
        /// <param name="service">服务类型。</param>
        /// <param name="serviceImplementation">服务实现类型。</param>
        /// <returns>服务条目集合。</returns>
        IEnumerable<ServiceEntity> CreateServiceEntry(Type service, Type serviceImplementation);
    }
}