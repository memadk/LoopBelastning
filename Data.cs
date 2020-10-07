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
    public static class Data
    {
        [FunctionName("Data")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB("Belastning", "belastning", ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "select * from c order by c.Timestamp")]
                IEnumerable<Belastning> belastning,
            ILogger log)
        {
            var queryDict = req.GetQueryParameterDictionary();
            if(queryDict.Keys.Contains("FilterDay"))
            {
                var filter = int.Parse(queryDict["FilterDay"]);
                return (ActionResult)new OkObjectResult(belastning.Where(b => b.Timestamp.Day == filter).Select(b => new {date = b.Timestamp.Subtract(new DateTime(1970, 1,1)).TotalMilliseconds, value = b.Value}));
            }
            if(queryDict.Keys.Contains("NumberOfDays"))
            {
                var filter = int.Parse(queryDict["NumberOfDays"]);
                return (ActionResult)new OkObjectResult(belastning.Where(b => b.Timestamp.AddDays(filter) > DateTime.Now ).Select(b => new {date = b.Timestamp.Subtract(new DateTime(1970, 1,1)).TotalMilliseconds, value = b.Value}));
            }

            return (ActionResult)new OkObjectResult(belastning.Select(b => new {date = b.Timestamp.Subtract(new DateTime(1970, 1,1)).TotalMilliseconds, value = b.Value}));
        }
    }
}
