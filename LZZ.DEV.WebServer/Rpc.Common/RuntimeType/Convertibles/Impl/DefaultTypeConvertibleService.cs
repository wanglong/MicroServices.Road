using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rpc.Common.RuntimeType.Exceptions;

namespace Rpc.Common.RuntimeType.Convertibles.Impl
{
    public class DefaultTypeConvertibleService : ITypeConvertibleService
    {
        private readonly IEnumerable<TypeConvertDelegate> _converters;


        public DefaultTypeConvertibleService(IEnumerable<ITypeConvertibleProvider> providers
//            , ILogger<DefaultTypeConvertibleService> logger
        )
        {
//            _logger = logger;
            providers = providers.ToArray();
//            if (_logger.IsEnabled(LogLevel.Debug))
//                _logger.LogDebug($"发现了以下类型转换提供程序：{string.Join(",", providers.Select(p => p.ToString()))}。");
            _converters = providers.SelectMany(p => p.GetConverters()).ToArray();
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

//            if (_logger.IsEnabled(LogLevel.Debug))
//                _logger.LogDebug($"准备将 {instance.GetType()} 转换为：{conversionType}。");

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

//            if (_logger.IsEnabled(LogLevel.Error))
//                _logger.LogError($"将 {instance.GetType()} 转换成 {conversionType} 时发生了错误。", exception);
            throw exception;
        }
    }

    /// <summary>
    /// 类型转换。
    /// </summary>
    /// <param name="instance">需要转换的实例。</param>
    /// <param name="conversionType">转换的类型。</param>
    /// <returns>转换之后的类型，如果无法转换则返回null。</returns>
    public delegate object TypeConvertDelegate(object instance, Type conversionType);

    /// <summary>
    /// 一个抽象的类型转换提供程序。
    /// </summary>
    public interface ITypeConvertibleProvider
    {
        /// <summary>
        /// 获取类型转换器。
        /// </summary>
        /// <returns>类型转换器集合。</returns>
        IEnumerable<TypeConvertDelegate> GetConverters();
    }
}