using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SImpl.DotNetStack.Verbosity
{
    public class VerboseHost : IHost
    {
        private readonly IHost _host;
        private readonly ILogger<VerboseHost> _logger;

        public VerboseHost(IHost host, ILogger<VerboseHost> logger)
        {
            _host = host;
            _logger = logger;
        }
        
        public void Dispose()
        {
            _host.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            _logger.LogDebug("> Host starting");
            await _host.StartAsync(cancellationToken);
            _logger.LogDebug("> Host started");
        }

        public async Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            _logger.LogDebug("> Host stopping");
            await _host.StopAsync(cancellationToken);
            _logger.LogDebug("> Host stopped");
        }

        public IServiceProvider Services => _host.Services;
    }
}