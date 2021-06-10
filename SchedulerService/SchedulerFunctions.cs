using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.DurableTask.ContextImplementations;
using Microsoft.Azure.WebJobs.Extensions.DurableTask.Options;
using Microsoft.Extensions.Logging;

namespace SchedulerService
{
    public interface ISchedulerFunctions
    {
        Task ScheduleAsync(string id);
        Task UnscheduleAsync(string id);
    }

    public class SchedulerFunctions : ISchedulerFunctions
    {
        private readonly ILogger _log;
        private readonly IDurableOrchestrationClient _client;

        public SchedulerFunctions(IDurableClientFactory durableClientFactory, ILogger<SchedulerFunctions> log)
        {
            _client = durableClientFactory.CreateClient(
                new DurableClientOptions()
                {
                    TaskHub = "SchedulerTaskHub"
                }
            );
            _log = log;
        }

        public async Task ScheduleAsync(string id)
        {
            await _client.StartNewAsync(nameof(ScheduledTaskInOneMinuteAsync), id);
            _log.LogInformation($"Task with id: {id} scheduled");
        }

        public async Task UnscheduleAsync(string id)
        {
            _log.LogInformation($"Task with id: {id} unscheduled");
            await _client.TerminateAsync(id, "Regrets were made");
        }

        private async Task ScheduledTaskInOneMinuteAsync([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            await context.CreateTimer(DateTime.Now.AddMinutes(1), CancellationToken.None);
            _log.LogInformation("Jobs done!");
        }
    }
}