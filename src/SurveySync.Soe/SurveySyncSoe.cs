using System.Runtime.InteropServices;
using CommandPattern;
using SurveySync.Soe.Commands;
using SurveySync.Soe.Configuration;
using SurveySync.Soe.Infastructure;
using SurveySync.Soe.Infastructure.IOC;
using ESRI.ArcGIS.SOESupport;
using ESRI.ArcGIS.Server;
using ESRI.ArcGIS.esriSystem;
using SurveySync.Soe.Startup;

namespace SurveySync.Soe {
    /// <summary>
    ///     The main server object extension
    /// </summary>
    [ComVisible(true), Guid("a48951e3-a7a8-4d30-8331-a43ad0122a3b"), ClassInterface(ClassInterfaceType.None),
     ServerObjectExtension("MapServer",
         AllCapabilities = "",
         //These create checkboxes to determine allowed functionality
         DefaultCapabilities = "",
         Description = "Keeps HistoricSurvey's in sync with the Contribution Container",
         //shows up in manager under capabilities
         DisplayName = "UDSH Survey Sync",
         //Properties that can be set on the capabilities tab in manager.
         Properties = @"connectionString=Data Source=localhost\sqlexpress::Initial Catalog=UDSHHistoricBuildings::Trusted_Connection=Yes;
propertySurveyRecordTableName=PROPERTYSURVEYRECORD;
featureServiceUrl=http://localhost/arcgis/rest/services/UDSH/soe/FeatureServer/applyEdits;
ContributionPropertyPoint.PropertyId=PropertyId;
Survey.SurveyId=SurveyRecordID;
Survey.PropertyId=PropertyRecordID;
Survey.ReturnFields=PropertyRecordID;
Buildings.PropertyId=PropertyId",
         SupportsREST = true,
         SupportsSOAP = false)]
    public class SurveySyncSoe : SoeBase, IServerObjectExtension, IObjectConstruct, IRESTRequestHandler {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SurveySyncSoe" /> class. If you have business logic
        ///     that you want to run when the SOE first becomes enabled, don’t here; instead, use the following
        ///     IObjectConstruct.Construct() method found in SoeBase.cs
        /// </summary>
        public SurveySyncSoe() {
            ReqHandler = CommandExecutor.ExecuteCommand(
                new CreateRestImplementationCommand(typeof (FindAllEndpointsCommand).Assembly));
            Kernel = new Container();
#if DEBUG
            Kernel.Register<IConfigurable>(x => new DebugConfiguration());
#else
            Kernel.Register<IConfigurable>(x => new RestEndPointConfiguration());
#endif
        }

        private Container Kernel { get; set; }

        #region IObjectConstruct Members
        /// <summary>
        ///     This is where you put any expensive business logic that you don’t need to run on each request. For example, if you
        ///     know you’re always working with the same layer in the map, you can put the code to get the layer here.
        /// </summary>
        /// <param name="props"> The props. </param>
        public override void Construct(IPropertySet props) {
            base.Construct(props);

            CacheConfig.Cache(ServerObjectHelper, props, Kernel);
        }
        #endregion
    }
}
