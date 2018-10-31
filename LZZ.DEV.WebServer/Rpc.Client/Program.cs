using Microsoft.Extensions.DependencyInjection;
using Rpc.Common.RuntimeType.Communally.Convertibles;
using Rpc.Common.RuntimeType.Communally.Convertibles.Impl;
using Rpc.Common.RuntimeType.Communally.IdGenerator;
using Rpc.Common.RuntimeType.Communally.IdGenerator.Impl;

namespace Rpc.Client
{
    static class Program
    {
        static void Main()
        {
            var serviceCollection = new ServiceCollection();
            {
                serviceCollection.AddLogging();
                serviceCollection.AddSingleton<IServiceIdGenerator, DefaultServiceIdGenerator>();

//                serviceCollection.AddSingleton<ITypeConvertibleProvider, DefaultTypeConvertibleProvider>();
                serviceCollection.AddSingleton<ITypeConvertibleService, DefaultTypeConvertibleService>();
            }
        }
    }
}