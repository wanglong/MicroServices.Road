using System.Reflection;

namespace Rpc.Common.RuntimeType.Communally.IdGenerator
{
    /// <summary>
    /// 一个抽象的服务Id生成器
    /// </summary>
    public interface IServiceIdGenerator
    {
        /// <summary>
        /// 生成一个服务Id
        /// </summary>
        /// <param name="method">本地方法信息</param>
        /// <returns>对应方法的唯一服务Id</returns>
        string GenerateServiceId(MethodInfo method);       
    }
}