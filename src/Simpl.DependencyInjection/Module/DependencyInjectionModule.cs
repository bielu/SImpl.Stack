﻿using Microsoft.Extensions.DependencyInjection;
using SImpl.Modules;

namespace Simpl.DependencyInjection.Module
{
    public class DependencyInjectionModule : IServicesCollectionConfigureModule
    {
        public string Name { get; } = nameof(DependencyInjectionModule);
        public void ConfigureServices(IServiceCollection services)
        {
           
            services.AddSingleton<IContainerService, ContainerService>(provider => new ContainerService(provider));
            foreach (var dependency in ContainerRegisterService.Current.Dependencies)
            {
                switch (dependency.Lifestyle)
                {
                    case Lifestyle.Scoped:
                        if (dependency.Abstraction != dependency.Implementation)
                        {
                            services.AddScoped(dependency.Abstraction, dependency.Implementation);
                        }
                        else
                        {
                            services.AddScoped(dependency.Implementation);
                        }
                        break;
                    case Lifestyle.Singleton:
                        if (dependency.Abstraction != dependency.Implementation)
                        {
                            services.AddSingleton(dependency.Abstraction, dependency.Implementation);
                        }
                        else
                        {
                            services.AddSingleton(dependency.Implementation);
                        }
                        break;
                    case Lifestyle.Transient:
                        if (dependency.Abstraction != dependency.Implementation)
                        {
                            services.AddTransient(dependency.Abstraction, dependency.Implementation);
                        }
                        else
                        {
                            services.AddTransient(dependency.Implementation);
                        }
                        break;
                }  
            }
         
        }
        
        public DependencyInjectionModule(DependencyInjectionConfig config)
        {
        }
    }
}