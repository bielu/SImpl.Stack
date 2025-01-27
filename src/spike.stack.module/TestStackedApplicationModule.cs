using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SImpl.Application.Builders;
using SImpl.Modules;
using SImpl.Runtime;

namespace spike.stack.module
{
    [DependsOn(typeof(TestApplicationModule))]
    public class TestStackedApplicationModule : IApplicationModule, IServicesCollectionConfigureModule, IPreInitModule
    {
        public void Configure(ISImplApplicationBuilder builder)
        {
            builder.AttachNewAppModuleOrGetConfigured<TestApplicationModule>();
        }

        public string Name => nameof(TestStackedApplicationModule);
        public void PreInit()
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public Task StartAsync()
        {
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            return Task.CompletedTask;
        }
    }
}