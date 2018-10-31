using Microsoft.Extensions.DependencyInjection;
using Rpc.Common.Easy.Rpc.Communally.Convertibles;
using Rpc.Common.Easy.Rpc.Communally.Convertibles.Impl;
using Rpc.Common.Easy.Rpc.Communally.IdGenerator;
using Rpc.Common.Easy.Rpc.Communally.IdGenerator.Impl;

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