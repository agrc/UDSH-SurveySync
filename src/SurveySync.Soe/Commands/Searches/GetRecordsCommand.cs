using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using SurveySync.Soe.Infastructure.Commands;
using SurveySync.Soe.Models;

namespace SurveySync.Soe.Commands.Searches {

    public class GetRecordsCommand : Command<Collection<Schema>>
    {
        private readonly IFeatureClass _table;
        private readonly string _whereClause;
        private readonly Dictionary<string, IndexFieldMap> _indexFieldMaps;

        public GetRecordsCommand(FeatureClassIndexMap map, string whereClause)
        {
            _table = map.FeatureClass;
            _whereClause = whereClause;
            _indexFieldMaps = map.FieldMap;
        }

        /// <summary>
        ///     code to execute when command is run.
        /// </summary>
        protected override void Execute()
        {
            var queryFilter = new QueryFilter
                {
                    WhereClause = _whereClause
                };

            var cursor = _table.Search(queryFilter, true);

            var ids = new Collection<Schema>();

            IFeature feature;
            while ((feature = cursor.NextFeature()) != null)
            {
                ids.Add(Schema.Map(feature, _indexFieldMaps));
            }

            Marshal.ReleaseComObject(cursor);

            Result = ids;
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, WhereClause: {1}", "GetRecordsCommand", _whereClause);
        }
    }

}