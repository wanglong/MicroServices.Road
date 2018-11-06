using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rpc.Common.Easy.Rpc.Attributes;

namespace Rpc.Common
{
    /// <summary>
    /// 接口UserService的定义
    /// </summary>
    [RpcTagBundle]
    public interface IUserService
    {
        Task<string> GetUserName(int id);

        Task<int> GetUserId(string userName);

        Task<DateTime> GetUserLastSignInTime(int id);

        Task<UserModel> GetUser(int id);

        Task<bool> Update(int id, UserModel model);

        Task<IDictionary<string, string>> GetDictionary();

        Task Try();

        Task TryThrowException();
    }
}

/*
 * 不支持的数据类型为array的数据类型，但支持List<T>类型
 */