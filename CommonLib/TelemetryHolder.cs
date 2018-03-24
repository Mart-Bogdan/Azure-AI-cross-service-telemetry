using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;

namespace CommonLib
{
    public static class TelemetryHolder
    {
        public static TelemetryClient TelemetryClient { get; private set; }
        
        public static void Init(String cliudRoleName)
        {
            var configuration = TelemetryConfiguration.Active;
            configuration.InstrumentationKey = Constants.AiKey;
            var module = new DependencyTrackingTelemetryModule();
            
            // prevent Correlation Id to be sent to certain endpoints. You may add other domains as needed.
            module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("core.windows.net");
            //...
            
            // enable known dependency tracking, note that in future versions, we will extend this list. 
            // please check default settings in https://github.com/Microsoft/ApplicationInsights-dotnet-server/blob/develop/Src/DependencyCollector/NuGet/ApplicationInsights.config.install.xdt#L20
            module.IncludeDiagnosticSourceActivities.Add("Microsoft.Azure.ServiceBus");
            module.IncludeDiagnosticSourceActivities.Add("Microsoft.Azure.EventHubs");
            
            module.Initialize(configuration);
            
            // stamps telemetry with correlation identifiers
            TelemetryConfiguration.Active.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
            configuration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());
            configuration.TelemetryInitializers.Add(new AppInstIniter(cliudRoleName));
            
            
            
            
            TelemetryClient = new TelemetryClient();
        }
    }

    class AppInstIniter : ITelemetryInitializer
    {
        private readonly string _cliudRoleName;

        public AppInstIniter(string cliudRoleName)
        {
            _cliudRoleName = cliudRoleName;
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = _cliudRoleName;
        }
    }
}