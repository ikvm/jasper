using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Jasper.EnvironmentChecks
{
    public class LoggingHostedServiceDecorator : IHostedService
    {
        private readonly IHostedService _inner;
        private readonly IEnvironmentRecorder _recorder;

        public LoggingHostedServiceDecorator(IHostedService inner, IEnvironmentRecorder recorder)
        {
            _inner = inner;
            _recorder = recorder;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _inner.StartAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failure in {_inner}");
                _recorder.Failure($"Failure while running {_inner}.{nameof(IHostedService.StartAsync)}()", e);
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _inner.StopAsync(cancellationToken);
        }

        public override string ToString()
        {
            return _inner.ToString();
        }
    }
}
