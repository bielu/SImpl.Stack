using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SImpl.DotNetStack.Dependencies;
using SImpl.DotNetStack.Modules;

namespace SImpl.DotNetStack.Core
{
    /// <summary>
    /// Responsible for maintaining runtime information about attached modules
    /// </summary>
    public class ModuleRuntimeInfo
    {
        public ModuleRuntimeInfo(IDotNetStackModule module)
        {
            Module = module;
            State = ModuleState.New;
            
            var dependsOn = module.GetType().GetCustomAttribute<DependsOnAttribute>();
            Dependencies = dependsOn?.Dependencies ?? Array.Empty<Type>();
        }
        
        public IDotNetStackModule Module { get; }

        public Type[] Dependencies { get; }
        
        public Type ModuleType => Module.GetType();

        public bool IsEnabled { get; private set; } = true;
        
        public void Disable()
        {
            IsEnabled = false;
        }
        
        public ModuleState State { get; private set; }

        public void SetState(ModuleState state)
        {
            State = state;
        }
    }
}