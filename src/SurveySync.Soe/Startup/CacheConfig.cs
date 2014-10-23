using ESRI.ArcGIS.Server;
using SurveySync.Soe.Cache;
using SurveySync.Soe.Commands;
using SurveySync.Soe.Infastructure;
using SurveySync.Soe.Infastructure.Commands;
using SurveySync.Soe.Infastructure.IOC;

namespace SurveySync.Soe.Startup {

    public static class CacheConfig {
        public static void Cache(IServerObjectHelper soeHelper, Container kernel)
        {
            var config = kernel.Create<IConfigurable>();

            ApplicationCache.Settings = config.Settings;
            ApplicationCache.FeatureClassIndexMap = CommandExecutor.ExecuteCommand(new CreateLayerMapCommand(soeHelper));
//            ApplicationCache.Layers.Buildings =
//                CommandExecutor.ExecuteCommand(new FindLayerByNameCommand("Buildings", soeHelper));
//            ApplicationCache.Layers.ContributionPropertyPoint =
//                CommandExecutor.ExecuteCommand(new FindLayerByNameCommand("cpp", soeHelper));
        }
    }

}