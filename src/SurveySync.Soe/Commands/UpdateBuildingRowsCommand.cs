using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using SurveySync.Soe.Cache;
using SurveySync.Soe.Commands.Sql;
using SurveySync.Soe.Infastructure.Commands;
using SurveySync.Soe.Models;

namespace SurveySync.Soe.Commands {

    public class UpdateBuildingRowsCommand : Command<int>{
        private readonly IFeatureClass _sourceTable;
        private readonly IFeatureClass _destinationTable;
        private readonly string _whereClause;
        private readonly Dictionary<string, IndexFieldMap> _destinationFieldMap;
        private readonly Dictionary<string, IndexFieldMap> _sourceFieldMap;

        public UpdateBuildingRowsCommand(FeatureClassIndexMap source, FeatureClassIndexMap destination,
                                            IEnumerable<Schema> contributions)
        {
            _sourceTable = source.FeatureClass;
            _destinationTable = destination.FeatureClass;
            _whereClause = CommandExecutor.ExecuteCommand(
                    new ComposeInSetQueryCommand(ApplicationCache.Settings.ContributionPropertyPointFields.PropertyId,
                                                 contributions.Select(x => x.PropertyId).Cast<object>()));
            _destinationFieldMap = destination.FieldMap;
            _sourceFieldMap = source.FieldMap;
        }
        /// <summary>
        ///     code to execute when command is run.
        /// </summary>
        protected override void Execute()
        {
            // TODO create query filter for each class
            var queryFilter = new QueryFilter
            {
                WhereClause = _whereClause
            };

            var orderBy = queryFilter as IQueryFilterDefinition;
            if (orderBy == null)
            {
                throw new ArgumentNullException("orderBy", "There was a problem casting the query filter to a query filter definition.");
            }

            orderBy.PostfixClause = string.Format("order by {0}",
                                                   ApplicationCache.Settings.ContributionPropertyPointFields.PropertyId);

            var destinationCursor = _destinationTable.Update(queryFilter, true);
            var sourceCursor = _sourceTable.Search(queryFilter, true);
            var destinationPkLocation =
                _destinationFieldMap[ApplicationCache.Settings.ContributionPropertyPointFields.PropertyId].Index;
            //TODO create app setting for buildings
            var sourcePkLocation =
                _sourceFieldMap[ApplicationCache.Settings.ContributionPropertyPointFields.PropertyId].Index;

            // fail if update sets are not identical
            var count = 0;
            IFeature sourceFeature;
            var destinationFeature = destinationCursor.NextFeature();
            while ((sourceFeature = sourceCursor.NextFeature()) != null)
            {
                if (destinationFeature == null)
                {
                    Result = count;

                    Marshal.ReleaseComObject(sourceCursor);
                    Marshal.ReleaseComObject(destinationCursor);
                    
                    throw new DataMisalignedException(string.Format("Update sets are not identical. Investigate {0}", _whereClause));
                }

                var sourcePropertyId = Convert.ToInt32(sourceFeature.Value[sourcePkLocation]);
                var destPropertyId = Convert.ToInt32(destinationFeature.Value[destinationPkLocation]);

                if (sourcePropertyId != destPropertyId)
                {
                    Result = count;
                    
                    Marshal.ReleaseComObject(sourceCursor);
                    Marshal.ReleaseComObject(destinationCursor);
                    
                    throw new DataMisalignedException(string.Format("Update sets are not identical. Investigate {0}", _whereClause));
                }

                UpdateFeature(sourceFeature, destinationFeature);
                destinationFeature.Shape = sourceFeature.ShapeCopy;
                destinationCursor.UpdateFeature(destinationFeature);

                count++;
                destinationFeature = destinationCursor.NextFeature();
            }

            Marshal.ReleaseComObject(sourceCursor);
            Marshal.ReleaseComObject(destinationCursor);

            Result = count;
        }

        private void UpdateFeature(IFeature source, IFeature destination)
        {
            var fields = source.Fields;

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

                destination.Value[_destinationFieldMap[fieldKey].Index] = source.Value[i];
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("UpdateBuildingRowsCommand, where {0}", _whereClause);
        }
    }

}