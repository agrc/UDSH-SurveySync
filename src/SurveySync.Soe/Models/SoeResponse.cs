using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SurveySync.Soe.Models.FeatureService;

namespace SurveySync.Soe.Models {

    public class SoeResponse {
        public SoeResponse(IList<FeatureServiceEditResponse> response)
        {
            var errorActions = response.SelectMany(a => a.AddResults.Where(x => !x.Success))
                                       .Union(response.SelectMany(u => u.UpdateResults.Where(x => !x.Success)))
                                       .Union(response.SelectMany(u => u.DeleteResults.Where(x => !x.Success)));

            Message = string.Join(" ", errorActions.Select(x => x.Error.Description));
            Successful = string.IsNullOrEmpty(Message);

            Updated = response.Sum(u => u.UpdateResults.Count(x => x.Success));
            Deleted = response.Sum(d => d.DeleteResults.Count(x => x.Success));
            Created = response.Sum(a => a.AddResults.Count(x => x.Success));
        }

        [JsonProperty(PropertyName = "successful")]
        public bool Successful { get; private set; }
        [JsonProperty(PropertyName = "updated")]
        public int Updated { get; private set; }
        [JsonProperty(PropertyName = "deleted")]
        public int Deleted { get; private set; }
        [JsonProperty(PropertyName = "created")]
        public int Created { get; private set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; private set; }

        public bool ShouldSerializeMessage()
        {
            return !string.IsNullOrEmpty(Message);
        }
    }

}