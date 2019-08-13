using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace dk.mema.loop
{
    public static class Seneste
    {
        [FunctionName("Seneste")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB("Belastning", "belastning", ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "select top 1 * from c order by c.Timestamp desc")]
                IEnumerable<Belastning> belastning,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return (ActionResult)new OkObjectResult(belastning.SingleOrDefault());
        }
    }
}
