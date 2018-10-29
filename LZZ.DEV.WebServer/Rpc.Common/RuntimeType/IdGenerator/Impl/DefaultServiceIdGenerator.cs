using System;
using System.Linq;
using System.Reflection;

namespace Rpc.Common.RuntimeType.IdGenerator.Impl
{
    public class DefaultServiceIdGenerator : IServiceIdGenerator
    {
        public string GenerateServiceId(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            var type = method.DeclaringType;
            if (type == null)
                throw new ArgumentNullException(nameof(method.DeclaringType), "方法的定义类型不能为空。");

            var id = $"{type.FullName}.{method.Name}";
            var parameters = method.GetParameters();
            if (parameters.Any())
            {
                id += "_" + string.Join("_", parameters.Select(i => i.Name));
            }
            return id;
        }
    }
}