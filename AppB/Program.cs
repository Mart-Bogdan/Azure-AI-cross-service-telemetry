using System;
using System.Threading;
using System.Threading.Tasks;
using CommonLib;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Diagnostics;

namespace AppB
{
    class Program
    { 
        private static TelemetryClient telemetryClient;
        static void Main(string[] args)
        {
            TelemetryHolder.Init("AppB");
            telemetryClient = TelemetryHolder.TelemetryClient;

            var client = new SubscriptionClient(Constants.SbConnectionString, "demo","sub1", ReceiveMode.ReceiveAndDelete);

            client.RegisterMessageHandler(ProcessAsync, new MessageHandlerOptions((ex)=>Task.CompletedTask));


            Console.ReadLine();
        }
        
        static async Task ProcessAsync(Message message, CancellationToken cancellation)
        {
            var activity = message.ExtractActivity();

            // If you are using Microsoft.ApplicationInsights package version 2.6-beta or higher, you should call StartOperation<RequestTelemetry>(activity) instead
            using (var operation = telemetryClient.StartOperation<RequestTelemetry>(activity))
            {
                telemetryClient.TrackTrace("Received message");
                try 
                {
                    // process message
                }
                catch (Exception ex)
                {
                    telemetryClient.TrackException(ex);
                    operation.Telemetry.Success = false;
                    throw;
                }

//                telemetryClient.TrackTrace("Done");
            }
        }
    }
}