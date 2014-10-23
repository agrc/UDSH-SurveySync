using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using CommandPattern;
using ESRI.ArcGIS.Geodatabase;
using SurveySync.Soe.Models.FeatureService;

namespace SurveySync.Soe.Commands.Searches {

    public class GetRecordsCommand : Command<Collection<FeatureAction>> {
        private readonly IFeatureClass _table;
        private readonly string _whereClause;

        public GetRecordsCommand(IFeatureClass table, string whereClause)
        {
            _table = table;
            _whereClause = whereClause;
        }

        /// <summary>
        ///     code to execute when command is run.
        /// </summary>
        public override void Execute()
        {
            var queryFilter = new QueryFilter
                {
                    WhereClause = _whereClause
                };

            var cursor = _table.Search(queryFilter, true);

            var ids = new Collection<FeatureAction>();

            IFeature feature;
            while ((feature = cursor.NextFeature()) != null)
            {
                ids.Add(FeatureAction.Create(feature));
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