using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SynchroLean
{
    public class RolloverService : HostedService
    {
        private readonly Rollover _rollover;

        public RolloverService(Rollover rollover)
        {
            _rollover = rollover;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                _rollover.RunRollover();
                await Task.Delay(DateTime.Today.AddDays(1) - DateTime.Now, cancellationToken);
                
                // Use this for testing purposes if you can change db externally
                //await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }
    }
}