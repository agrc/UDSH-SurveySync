using System.Data.SqlClient;
using System.Linq;
using CommandPattern;
using Dapper;
using SurveySync.Soe.Models.Configuration;

namespace SurveySync.Soe.Commands.Searches {

    public class GetPropertyIdsFromSurveyCommand : Command<int[]> {
        private readonly double _id;
        private readonly ApplicationSettings _settings;

        public GetPropertyIdsFromSurveyCommand(ApplicationSettings settings, double id)
        {
            _settings = settings;
            _id = id;
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, Settings: {1}, Id: {2}", "GetPropertyIdsFromSurveyCommand", _settings, _id);
        }

        //      select PropertyRecordID from PROPERTYSURVEYRECORD where SurveyRecordID = 126
        /// <summary>
        ///     code to execute when command is run.
        /// </summary>
        public override void Execute()
        {
            var fields = _settings.SurveyToPropertyIdFields;

            var query = string.Format("select {0} from {1} where {2} = {3}", string.Join(",", fields.ReturnFields),
                                      _settings.SurveyToPropertyIdLookupTable, fields.SurveyId, _id);

            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                Result = connection.Query<int>(query).ToArray();

                if (!Result.Any())
                {
                    Result = null;
                }
            }
        }
    }

}