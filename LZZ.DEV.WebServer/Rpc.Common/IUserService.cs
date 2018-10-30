using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rpc.Common.RuntimeType;
using Rpc.Common.RuntimeType.Attributes;

namespace Rpc.Common
{
    /// <summary>
    /// 接口UserService的定义
    /// </summary>
    // 在 protobuf 的服务绑定中，需要对传输的数据结构（或模板）进行rpc特性绑定
    [RpcTagBundle]
    public interface IUserService
    {
        Task<string> GetUserName(int id);

        Task<int> GetUserId(string userName);

        Task<DateTime> GetUserLastSignInTime(int id);

        Task<UserModel> GetUser(int id);

        Task<bool> Update(int id, UserModel model);

        Task<IDictionary<string, string>> GetDictionary();

//        [RpcTagBundle(IsWaitExecution = false)]
        Task Try();

        Task TryThrowException();
    }
}

/*
 * 不支持的数据类型为array的数据类型，但支持List<T>类型
 */