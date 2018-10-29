using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rpc.Common.RuntimeType.Communally.Exceptions;

namespace Rpc.Common.RuntimeType.Communally.Convertibles.Impl
{
    public class DefaultTypeConvertibleService : ITypeConvertibleService
    {
        private readonly IEnumerable<TypeConvertDelegate> _converters;

        public DefaultTypeConvertibleService(IEnumerable<ITypeConvertibleProvider> providers)
        {
            _converters = providers.ToArray().SelectMany(p => p.GetConverters()).ToArray();
        }

        /// <summary>
        /// 转换。
        /// </summary>
        /// <param name="instance">需要转换的实例。</param>
        /// <param name="conversionType">转换的类型。</param>
        /// <returns>转换之后的类型，如果无法转换则返回null。</returns>
        public object Convert(object instance, Type conversionType)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (conversionType == null)
                throw new ArgumentNullException(nameof(conversionType));

            if (conversionType.GetTypeInfo().IsInstanceOfType(instance))
                return instance;

            object result = null;
            foreach (var converter in _converters)
            {
                result = converter(instance, conversionType);
                if (result != null)
                    break;
            }

            if (result != null)
                return result;
            var exception = new RpcException($"无法将实例：{instance}转换为{conversionType}。");
            
            throw exception;
        }
    }
}