using System;

namespace Rpc.Common.RuntimeType.Communally.Convertibles.Impl
{
    /// <summary>
    /// 类型转换代理类
    /// </summary>
    /// <param name="instance">需要转换的实例。</param>
    /// <param name="conversionType">转换的类型。</param>
    /// <returns>转换之后的类型，如果无法转换则返回null。</returns>
    public delegate object TypeConvertDelegate(object instance, Type conversionType);
}