using System;

namespace Rpc.Common.RuntimeType.Attributes
{
    /// <summary>
    /// 服务集标记
    /// </summary>
    /// <remarks>
    /// 一个没有任何实现的rpc目标标记特性，用于在发射中查找对应的特性的接口标注（非接口实现）
    /// </remarks>
    [AttributeUsage(AttributeTargets.Interface)]
    public class RpcTagBundleAttribute : Attribute
    {
    }
}