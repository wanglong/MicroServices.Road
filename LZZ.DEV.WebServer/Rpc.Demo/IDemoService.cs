using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rabbit.Rpc.Runtime.Server.Implementation.ServiceDiscovery.Attributes;

namespace Rpc.Demo
{
    /// <summary>
    /// 接口DemoService的定义，一些常用类型的传参，返回测试
    /// </summary>
    [RpcServiceBundle] // 在 protobuf 的服务绑定中，需要对传输的数据结构（或模板）进行rpc特性绑定, 该特性在Rabbit.Rpc框架中
    public interface IDemoService
    {
        #region 简单数据类型

        Task<string> TestInt(int id);

        Task<int> TestString(string str);

        Task<bool> TestBool(bool log);

        Task<DateTime> TestDateTime(DateTime dateTime);

        Task<byte> TestByte(byte by);

        #endregion

        #region 复杂数据类型

        Task<UserModel> TestLong(UserModel userModel);

        Task<List<string>> TestDictionary(List<string> dictionary);

        Task<Dictionary<string, string>> TestDictionary(Dictionary<string, string> dictionary);

        Task<List<Dictionary<string, string>>> TestCollections(List<Dictionary<string, string>> collection);

        #endregion

        [RpcService(IsWaitExecution = false)]
        Task Try();
    }
}

/*
 * 不支持的数据类型为array的数据类型，但支持List<T>类型
 */