﻿using System;
using System.Net;
using System.Threading.Tasks;

namespace Rpc.Common.Easy.Rpc.Runtime.Server
{
    /// <summary>
    //抽象的服务主机
    /// </summary>
    public interface IServiceHost : IDisposable
    {
        /// <summary>
        /// 启动主机
        /// </summary>
        /// <param name="endPoint">主机终结点</param>
        /// <returns>一个任务</returns>
        Task StartAsync(EndPoint endPoint);
    }
}