using CommandPattern;
using ESRI.ArcGIS.SOESupport;
using ESRI.ArcGIS.Server;
using ESRI.ArcGIS.esriSystem;
using SurveySync.Soe.Cache;
using SurveySync.Soe.Commands;
using SurveySync.Soe.Infastructure;
using SurveySync.Soe.Infastructure.IOC;

namespace SurveySync.Soe.Startup {

    public static class CacheConfig {
        public static void Cache(IServerObjectHelper soeHelper, IPropertySet props, Container kernel)
        {
            var logger = new ServerLogger();      
            var config = kernel.Create<IConfigurable>();

            ApplicationCache.Settings = config.GetSettings(props);

#if !DEBUG
            logger.LogMessage(ServerLogger.msgType.infoSimple, "CacheConfig.Cache", 2472, ApplicationCache.Settings.ToString());
#endif

            ApplicationCache.FeatureClassIndexMap = CommandExecutor.ExecuteCommand(new CreateLayerMapCommand(soeHelper));
        }
    }

}