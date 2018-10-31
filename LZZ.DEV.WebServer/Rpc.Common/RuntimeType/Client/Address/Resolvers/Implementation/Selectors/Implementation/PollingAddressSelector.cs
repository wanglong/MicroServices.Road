﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rpc.Common.RuntimeType.Client.HealthChecks;
using Rpc.Common.RuntimeType.Entitys;
using Rpc.Common.RuntimeType.Entitys.Address;
using Rpc.Common.RuntimeType.Routing;
using Rpc.Common.RuntimeType.Routing.Implementation;

namespace Rpc.Common.RuntimeType.Client.Address.Resolvers.Implementation.Selectors.Implementation
{
    /// <summary>
    /// 轮询的地址选择器
    /// </summary>
    public class PollingAddressSelector : AddressSelectorBase
    {
        private readonly IHealthCheckService _healthCheckService;

        private readonly ConcurrentDictionary<string, Lazy<AddressEntry>> _concurrent =
            new ConcurrentDictionary<string, Lazy<AddressEntry>>();

        public PollingAddressSelector(IServiceRouteManager serviceRouteManager, IHealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
            //路由发生变更时重建地址条目
            serviceRouteManager.Changed += ServiceRouteManager_Removed;
            serviceRouteManager.Removed += ServiceRouteManager_Removed;
        }

        #region Overrides of AddressSelectorBase

        /// <summary>
        /// 选择一个地址
        /// </summary>
        /// <param name="context">地址选择上下文</param>
        /// <returns>地址模型</returns>
        protected override async Task<AddressModel> SelectAsync(AddressSelectContext context)
        {
            var key = GetCacheKey(context.Descriptor);
            //根据服务id缓存服务地址
            var addressEntry = _concurrent.GetOrAdd(key, k => new Lazy<AddressEntry>(() => new AddressEntry(context.Address))).Value;

            AddressModel addressModel;
            do
            {
                addressModel = addressEntry.GetAddress();
            } while (await _healthCheckService.IsHealth(addressModel) == false);
            return addressModel;
        }

        #endregion Overrides of AddressSelectorBase

        #region Private Method

        private static string GetCacheKey(ServiceDescriptor descriptor)
        {
            return descriptor.Id;
        }

        private void ServiceRouteManager_Removed(object sender, ServiceRouteEventArgs e)
        {
            var key = GetCacheKey(e.Route.ServiceDescriptor);
            Lazy<AddressEntry> value;
            _concurrent.TryRemove(key, out value);
        }

        #endregion Private Method

        #region Help Class

        protected class AddressEntry
        {
            #region Field

            private int _index;
            private int _lock;
            private readonly int _maxIndex;
            private readonly AddressModel[] _address;

            #endregion Field

            #region Constructor

            public AddressEntry(IEnumerable<AddressModel> address)
            {
                _address = address.ToArray();
                _maxIndex = _address.Length - 1;
            }

            #endregion Constructor

            #region Public Method

            public AddressModel GetAddress()
            {
                while (true)
                {
                    //如果无法得到锁则等待
                    if (Interlocked.Exchange(ref _lock, 1) != 0)
                    {
                        default(SpinWait).SpinOnce();
                        continue;
                    }

                    var address = _address[_index];

                    //设置为下一个
                    if (_maxIndex > _index)
                        _index++;
                    else
                        _index = 0;

                    //释放锁
                    Interlocked.Exchange(ref _lock, 0);

                    return address;
                }
            }

            #endregion Public Method
        }

        #endregion Help Class
    }
}