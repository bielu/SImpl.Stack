using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SImpl.Application.Builders;
using SImpl.Hosts.WebHost;
using SImpl.Hosts.WebHost.Startup;
using SImpl.Runtime;
using spike.stack.module;

namespace spike.stack.web
{
    public class Startup : IWebHostApplicationStartup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Works
            /*services.AddSingleton<IGreetingAppService, GreetingAppService>();
            services.AddSingleton<IGreetingService, SpanishGreetingService>();
            services.AddHostedService<GreetingHostedService>();*/
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }

        public void ConfigureStackApplication(IWebHostApplicationBuilder app)
        {
            app.UseWebHostAppModule<GreetingsWebModule>();
            
            app.UseAppModule<TestStackedApplicationModule>();
                    
            app.UseAppModule<ApplicationTestModule>();
            //stack.UseApplicationTestModule();
            
            // NOTE: Below is not supposed to work
            //stack.UseGenericHostStackAppModule<GenericHostApplicationTestModule>();
            //stack.UseGenericHostApplicationTestModule();
            
            app.UseWebHostAppModule<WebHostApplicationTestModule>();
            //stack.UseWebApplicationTestModule();
        }
    }
}
