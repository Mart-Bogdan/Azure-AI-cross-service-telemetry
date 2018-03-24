using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonLib;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Diagnostics;

namespace AppA
{
    class Program
    {
        static void Main(string[] args)
        {
            TelemetryHolder.Init("AppA");
            Console.WriteLine("Hello World!");
            
            TelemetryHolder.TelemetryClient.TrackTrace("AppA Started!");
            TelemetryHolder.TelemetryClient.Flush();
            MainAsync().Wait();

            Thread.Sleep(10000);
        }

        public static async Task MainAsync()
        {
            
            var queueClient = new TopicClient(Constants.SbConnectionString, "demo");
            using(TelemetryHolder.TelemetryClient.StartOperation(new RequestTelemetry(){Name =  "AppA"}))
            {
                using (var wc = new WebClient())
                {
                    using (TelemetryHolder.TelemetryClient.StartOperation(
                        new DependencyTelemetry() {Name = "WebApiProj", Data = "GetValue"}))
                    {
                        var downloadString = wc.DownloadString("http://localhost:5000/api/values/1");
                        Console.WriteLine(downloadString);
                        TelemetryHolder.TelemetryClient.TrackTrace("Dep called!");
                    }
                
                    var message = new Message(Encoding.UTF8.GetBytes("Hi"));
                    
                    await queueClient.SendAsync(message); 
                    message = new Message(Encoding.UTF8.GetBytes("Hi2"));
                    await queueClient.SendAsync(message);

                }

                TelemetryHolder.TelemetryClient.TrackTrace("AppA Requests sent!");
                
            }
            TelemetryHolder.TelemetryClient.Flush();
            TelemetryHolder.TelemetryClient.Flush();
        }
    }
}