using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace dk.mema.loop
{
    public static class LoopTimerTrigger
    {
        private const string TimerSchedule = "0 */2 * * * *";
        private static HttpClient _client = new HttpClient();

        [FunctionName("LoopTimerTrigger")]
        public static async Task RunAsync([TimerTrigger(TimerSchedule)]TimerInfo myTimer, ILogger log)
        {
            string html = await _client.GetStringAsync("https://loopfitness.com/da/aarhus-v/");
	
		    var text = html.Substring(html.IndexOf("new JustGage")+93, 5);
		    var belastning = text.Substring(0, text.IndexOf(","));
            
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            log.LogInformation($"C# Timer trigger function: {belastning}");
        }
    }
}
