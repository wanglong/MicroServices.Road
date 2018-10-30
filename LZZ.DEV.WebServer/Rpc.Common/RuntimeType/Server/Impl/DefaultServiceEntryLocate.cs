using System.Linq;
using Rpc.Common.RuntimeType.Entitys;
using Rpc.Common.RuntimeType.Entitys.Messages;

namespace Rpc.Common.RuntimeType.Server.Impl
{
    /// <summary>
    /// 默认的服务条目定位器。
    /// </summary>
    public class DefaultServiceEntryLocate : IServiceEntryLocate
    {
        private readonly IServiceEntryManager _serviceEntryManager;

        public DefaultServiceEntryLocate(IServiceEntryManager serviceEntryManager)
        {
            _serviceEntryManager = serviceEntryManager;
        }

        #region Implementation of IServiceEntryLocate

        /// <summary>
        /// 定位服务条目。
        /// </summary>
        /// <param name="invokeMessage">远程调用消息。</param>
        /// <returns>服务条目。</returns>
        public ServiceEntity Locate(RemoteInvokeMessage invokeMessage)
        {
            var serviceEntries = _serviceEntryManager.GetEntries();
            return serviceEntries.SingleOrDefault(i => i.Descriptor.Id == invokeMessage.ServiceId);
        }

        #endregion Implementation of IServiceEntryLocate
    }
}