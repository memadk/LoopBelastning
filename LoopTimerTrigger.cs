using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace dk.mema.loop
{
    public static class LoopTimerTrigger
    {
        private const string TimerSchedule = "0 */10 * * * *";
        private static HttpClient _client = new HttpClient();

        [FunctionName("LoopTimerTrigger")]
        public static void Run([TimerTrigger(TimerSchedule)]TimerInfo myTimer, [CosmosDB(
                databaseName: "Belastning",
                collectionName: "belastning",
                ConnectionStringSetting = "CosmosDBConnection")]out dynamic document, ILogger log)
        {
            var belastning = Task.Run(async () => await GetBelastning(log)).Result;
            
            document = new Belastning {
                Timestamp = DateTime.Now,
                Value = belastning
            };

            log.LogInformation($"C# Timer trigger function executed at: {document.Timestamp}");
            log.LogInformation($"C# Timer trigger function: {document.Value}");
        }

        private static async Task<int> GetBelastning(ILogger log)
        {
            string html = await _client.GetStringAsync("https://loopfitness.dk/centre/loop-fitness-aarhus-v/");
            log.LogInformation("Got HTML. Length: " + html.Length);
	
		    var text = html.Substring(html.IndexOf("<p class=\"overview-value ff-secondary bold para\">")+50, 8);
            log.LogInformation("The Substring: " + text);
		    var belastning = int.Parse(text.Substring(0, text.IndexOf("%</")));
            log.LogInformation("Parsed belastning: " + belastning);

            return belastning;
        }
    }

    public class Belastning{
        public DateTime Timestamp {get;set;}
        public int Value {get;set;}
    }
}
