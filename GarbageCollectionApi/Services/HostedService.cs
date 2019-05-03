namespace GarbageCollectionApi.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    public abstract class HostedService : IHostedService
    {
        private Task executingTask;
        private CancellationTokenSource cts;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a linked token so we can trigger cancellation outside of this token's cancellation
            this.cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Store the task we're executing
            this.executingTask = this.ExecuteAsync(this.cts.Token);

            // If the task is completed then return it
            if (this.executingTask.IsCompleted)
            {
                return this.executingTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (this.executingTask == null)
            {
                return;
            }

            // Signal cancellation to the executing method
            this.cts.Cancel();

            // Wait until the task completes or the stop token triggers
            await Task.WhenAny(this.executingTask, Task.Delay(-1, cancellationToken)).ConfigureAwait(false);

            // Throw if cancellation triggered
            cancellationToken.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Derived classes should override this and execute a long running method until cancellation is requested
        /// </summary>
        /// <param name="cancellationToken">Cancel</param>
        /// <returns>Task</returns>
        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }
}