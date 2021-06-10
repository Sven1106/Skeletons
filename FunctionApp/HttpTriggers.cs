using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SchedulerService;

namespace FunctionApp
{
    public class HttpTriggers
    {
        private ISchedulerFunctions _schedulerFunctions;

        public HttpTriggers(ISchedulerFunctions schedulerFunctions)
        {
            _schedulerFunctions = schedulerFunctions;
        }

        [FunctionName("schedule")]
        public async Task<IActionResult> ScheduleAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "schedule/{id}")]
            HttpRequest req,
            string id,
            ILogger log)
        {
            await _schedulerFunctions.ScheduleAsync(id);
            return new AcceptedResult();
        }


        [FunctionName("unschedule")]
        public async Task<IActionResult> UnscheduleAsync(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "schedule/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            await _schedulerFunctions.UnscheduleAsync(id);
            return new AcceptedResult();
        }

    }
}