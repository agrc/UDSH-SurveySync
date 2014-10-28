using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using SurveySync.Soe.Cache;
using SurveySync.Soe.Commands.Sql;
using SurveySync.Soe.Infastructure.Commands;
using SurveySync.Soe.Models;

namespace SurveySync.Soe.Commands {

    public class CreateNewBuildingRowsCommand : Command<int> {
        private readonly IFeatureClass _destinationTable;
        private readonly IFeatureClass _sourceTable;
        private readonly string _whereClause;
        private readonly Dictionary<string, IndexFieldMap> _destinationFieldMap;

        public CreateNewBuildingRowsCommand(FeatureClassIndexMap source, FeatureClassIndexMap destination,
                                            IEnumerable<Schema> contributions)
        {
            _sourceTable = source.FeatureClass;
            _destinationTable = destination.FeatureClass;
            _whereClause = CommandExecutor.ExecuteCommand(
                    new ComposeInSetQueryCommand(ApplicationCache.Settings.ContributionPropertyPointFields.PropertyId,
                                                 contributions.Select(x => x.PropertyId).Cast<object>()));
            _destinationFieldMap = destination.FieldMap;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "CreateNewBuildingRowsCommand";
        }

        /// <summary>
        ///     code to execute when command is run.
        /// </summary>
        protected override void Execute()
        {
            //http://help.arcgis.com/en/sdk/10.0/arcobjects_net/conceptualhelp/index.html#//00010000049v000000
            const bool useBuffering = false;
            
            var insertCursor = _destinationTable.Insert(useBuffering);
            var featureBuffer = _destinationTable.CreateFeatureBuffer();

            var queryFilter = new QueryFilter
                {
                    WhereClause = _whereClause
                };

            var cursor = _sourceTable.Search(queryFilter, true);

            var count = 0;
            IFeature feature;
            while ((feature = cursor.NextFeature()) != null)
            {
                ResetAndUpdateBuffer(feature, featureBuffer);
                featureBuffer.Shape = feature.ShapeCopy;
                insertCursor.InsertFeature(featureBuffer);
                count++;
            }

            insertCursor.Flush();

            Marshal.ReleaseComObject(featureBuffer);
            Marshal.ReleaseComObject(cursor);
            Marshal.ReleaseComObject(insertCursor);

            Result = count;
        }

        private void ResetAndUpdateBuffer(IFeature sourceData, IFeatureBuffer featureBuffer)
        {
            var fields = sourceData.Fields;

            for (var i = 0; i < fields.FieldCount; i++)
            {
                var field = fields.Field[i];

                var fieldKey = field.Name.ToUpper();
                if (!_destinationFieldMap.ContainsKey(fieldKey))
                {
                    continue;
                }

                if (!field.Editable)
                {
                    continue;
                }

                featureBuffer.Value[_destinationFieldMap[fieldKey].Index] = field.DefaultValue;
                featureBuffer.Value[_destinationFieldMap[fieldKey].Index] = sourceData.Value[i];
            }
        }
    }

}