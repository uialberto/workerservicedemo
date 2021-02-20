using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerServiceSample
{
    public class WorkerStatus : BackgroundService
    {
        private readonly ILogger<WorkerStatus> _logger;
        private HttpClient client;
        public WorkerStatus(ILogger<WorkerStatus> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            client = new HttpClient();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            client.Dispose();
            _logger.LogInformation("Servicio WorkerStatus ha sido detenido....");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = await client.GetAsync("https://develop.farmacorp.com/apifarmapub/");
                if (result.IsSuccessStatusCode)
                {
                    _logger.LogInformation("API Farmapub is Up. StatusCode: {StatusCode}", result.StatusCode);
                }
                else
                {
                    _logger.LogError("API Farmapub is Down. StatusCode: {StatusCode}", result.StatusCode);
                }
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
