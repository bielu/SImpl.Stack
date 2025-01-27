using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SImpl.Runtime.Core;
using SImpl.Runtime.Extensions;

namespace SImpl.Runtime.Diagnostics
{
    public class DiagnosticsHost : IHost
    {
        private const string TimetableSectionKey = "Timetable";
        
        private readonly IHost _host;
        private readonly IModuleManager _moduleManager;
        private readonly IBootSequenceFactory _bootSequenceFactory;
        private readonly IDiagnosticsCollector _diagnostics;
        private readonly RuntimeFlags _runtimeFlags;
        private readonly ILogger<DiagnosticsHost> _logger;

        public DiagnosticsHost(IHost host, IModuleManager moduleManager, IBootSequenceFactory bootSequenceFactory, IDiagnosticsCollector diagnostics, RuntimeFlags runtimeFlags, ILogger<DiagnosticsHost> logger)
        {
            _host = host;
            _moduleManager = moduleManager;
            _bootSequenceFactory = bootSequenceFactory;
            _diagnostics = diagnostics;
            _runtimeFlags = runtimeFlags;
            _logger = logger;
        }
        
        public void Dispose()
        {
            _host.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            _diagnostics.RegisterLapTime("Host starting");
            await _host.StartAsync(cancellationToken);
            _diagnostics.RegisterLapTime("Host started");

            AddFlagsSection();
            AddModulesSection();
            AddTimetableSection();
            AddModuleDiagnostics();
            
            var builder = new StringBuilder();
            
            builder.AppendLine();
            builder.AppendLine("STARTUP DIAGNOSTICS");
            builder.AppendLine();
            
            AppendDiagnosticsSections(builder, _diagnostics.Sections);

            _logger.LogDebug($"{builder}");
        }

        public async Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            _diagnostics.RegisterLapTime("Host stopping");
            await _host.StopAsync(cancellationToken);
            _diagnostics.RegisterLapTime("Host stopped");

            AddTimetableSection();
                
            var builder = new StringBuilder();
            
            builder.AppendLine();
            builder.AppendLine("SHUTDOWN DIAGNOSTICS");
            builder.AppendLine();
            
            AppendDiagnosticsSections(builder, new [] { _diagnostics.Get(TimetableSectionKey)});
            
            _logger.LogDebug($"{builder}");
        }

        public IServiceProvider Services => _host.Services;
        
        private void AppendDiagnosticsSections(StringBuilder builder, IReadOnlyList<IDiagnosticsSection> sections)
        {
            foreach (var section in sections)
            {
                builder.AppendLine($"{section.Headline} ".PadRight(60, '-'));
                section.Append(new StringBuilderDiagnosticsWriter(builder));
            }
        }
        
        private void AddFlagsSection()
        {
            var section = new ValueDiagnosticsSection
            {
                Headline = "Flags",
            };
            
            section.Value.AppendLine($"- diagnostics: {_runtimeFlags.Diagnostics}");
            section.Value.AppendLine($"- verbose: {_runtimeFlags.Verbose}");

            _diagnostics.AddSection("Flags", section);
        }
        
        private void AddModulesSection()
        {
            var section = new ValueDiagnosticsSection
            {
                Headline = "Modules",
            };
            
            section.Value.AppendLine("- Enabled modules");
            foreach (var module in _moduleManager.EnabledModules)
            {
                section.Value.AppendLine($"   - {module.Name}");
            }
            
            section.Value.AppendLine("- Disabled modules");
            foreach (var module in _moduleManager.DisabledModules)
            {
                section.Value.AppendLine($"   - {module.Name}");
            }
            
            section.Value.AppendLine("- Autorun modules");
            // TODO:

            section.Value.AppendLine("- Boot sequence");
            foreach (var module in _bootSequenceFactory.New())
            {
                section.Value.AppendLine($"   - {module.Name}");
            }
            
            _diagnostics.AddSection("Modules", section);
        }

        private void AddTimetableSection()
        {
            var section = new ValueDiagnosticsSection
            {
                Headline = "Timetable",
            };
            
            foreach (var lapTime in _diagnostics.Timetable)
            {
                section.Value.AppendLine($"{lapTime.At:s} {lapTime.Elapsed} {lapTime.Name}");
            }

            _diagnostics.ClearTimetable();
            
            _diagnostics.AddSection(TimetableSectionKey, section);
        }
        
        private void AddModuleDiagnostics()
        {
            _bootSequenceFactory.New().ForEach<IDiagnosticsModule>(module =>
            {
                module.Diagnose(_diagnostics);
            });
        }
    }
}