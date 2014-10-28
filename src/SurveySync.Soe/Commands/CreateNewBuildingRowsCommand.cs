using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using SurveySync.Soe.Cache;
using SurveySync.Soe.Commands.Sql;
using SurveySync.Soe.Infastructure.Commands;
using SurveySync.Soe.Models;

namespace SurveySync.Soe.Commands {

    public class CreateNewBuildingRowsCommand : Command {
        private readonly IFeatureClass _sourceTable;
        private readonly IFeatureClass _destinationTable;
        private readonly string _whereClause;

        public CreateNewBuildingRowsCommand(FeatureClassIndexMap source, FeatureClassIndexMap destination, IEnumerable<Schema> contributions)
        {
            _sourceTable = source.FeatureClass;
            _destinationTable = destination.FeatureClass;
            _whereClause =
                CommandExecutor.ExecuteCommand(
                    new ComposeInSetQueryCommand(ApplicationCache.Settings.ContributionPropertyPointFields.PropertyId,
                                                 contributions.Select(x => x.PropertyId).Cast<object>()));
        }

        /// <summary>
        ///     code to execute when command is run.
        /// </summary>
        protected override void Execute()
        {
            //http://help.arcgis.com/en/sdk/10.0/arcobjects_net/conceptualhelp/index.html#//00010000049v000000
            const bool useBuffering = false;
            var insertCursor = _destinationTable.Insert(useBuffering);

            var queryFilter = new QueryFilter
            {
                WhereClause = _whereClause
            };

            var cursor = _sourceTable.Search(queryFilter, true);

            IFeature feature;
            while ((feature = cursor.NextFeature()) != null)
            {
                
            }

            Marshal.ReleaseComObject(cursor);
        }
    }

}