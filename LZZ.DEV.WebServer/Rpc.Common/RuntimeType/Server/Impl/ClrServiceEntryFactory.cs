using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rpc.Common.RuntimeType.Convertibles;
using Rpc.Common.RuntimeType.IdGenerator;
using Rpc.Common.RuntimeType.Server.Attributes;

namespace Rpc.Common.RuntimeType.Server.Impl
{
    public class ClrServiceEntryFactory : IClrServiceEntryFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceIdGenerator _serviceIdGenerator;
        private readonly ITypeConvertibleService _typeConvertibleService;

        public ClrServiceEntryFactory(IServiceProvider serviceProvider, IServiceIdGenerator serviceIdGenerator,
            ITypeConvertibleService typeConvertibleService)
        {
            _serviceProvider = serviceProvider;
            _serviceIdGenerator = serviceIdGenerator;
            _typeConvertibleService = typeConvertibleService;
        }

        public IEnumerable<ServiceEntry> CreateServiceEntry(Type service, Type serviceImplementation)
        {
            foreach (var methodInfo in service.GetTypeInfo().GetMethods())
            {
                var implementationMethodInfo = serviceImplementation.GetTypeInfo().GetMethod(methodInfo.Name,
                    methodInfo.GetParameters().Select(p => p.ParameterType).ToArray());
                yield return Create(methodInfo, implementationMethodInfo);
            }
        }

        private ServiceEntry Create(MethodInfo method, MethodBase implementationMethod)
        {
            var serviceId = _serviceIdGenerator.GenerateServiceId(method);

            var serviceDescriptor = new ServiceDescriptor
            {
                Id = serviceId
            };

            var descriptorAttributes = method.GetCustomAttributes<RpcServiceDescriptorAttribute>();
            foreach (var descriptorAttribute in descriptorAttributes)
            {
                descriptorAttribute.Apply(serviceDescriptor);
            }

            return new ServiceEntry
            {
                Descriptor = serviceDescriptor,
                Func = parameters =>
                {
                    var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        var instance = scope.ServiceProvider.GetRequiredService(method.DeclaringType);

                        var list = new List<object>();
                        foreach (var parameterInfo in implementationMethod.GetParameters())
                        {
                            var value = parameters[parameterInfo.Name];
                            var parameterType = parameterInfo.ParameterType;

                            var parameter = _typeConvertibleService.Convert(value, parameterType);
                            list.Add(parameter);
                        }

                        var result = implementationMethod.Invoke(instance, list.ToArray());

                        return Task.FromResult(result);
                    }
                }
            };
        }
    }
}