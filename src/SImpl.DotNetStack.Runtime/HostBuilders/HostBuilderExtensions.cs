using System;
using Microsoft.Extensions.Hosting;
using SImpl.DotNetStack.HostBuilders;

namespace SImpl.DotNetStack.Runtime.HostBuilders
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder SImply(this IHostBuilder hostBuilder, Action<IDotNetStackHostBuilder> stackHostBuilder = null)
        {
            return hostBuilder.SImply(Array.Empty<string>(), stackHostBuilder);
        }
        
        public static IHostBuilder SImply(this IHostBuilder hostBuilder, string[] args, Action<IDotNetStackHostBuilder> stackHostBuilder = null)
        {
            return DotNetStackRuntime.Boot(hostBuilder, args, stackHostBuilder);
        }
    }
}