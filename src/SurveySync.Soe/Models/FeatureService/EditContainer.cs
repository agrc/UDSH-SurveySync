using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;

namespace SurveySync.Soe.Models.FeatureService {

    public class EditContainer {
        public EditContainer(ApplyEditActions[] applyEditActions)
        {
            Edits = applyEditActions;
        }

        /// <summary>
        /// Gets or sets the edits to be made to the feature service.
        /// </summary>
        /// <value>
        /// The edits.
        /// </value>
        public ApplyEditActions[] Edits { get; set; }

        /// <summary>
        /// Gets a value indicating whether to [rollback on failure].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [rollback on failure]; otherwise, <c>false</c>.
        /// </value>
        public bool RollbackOnFailure { get { return true; } }

        /// <summary>
        /// Gets the total # of edits and adds.
        /// </summary>
        /// <value>
        /// The total.
        /// </value>
        [JsonIgnore]
        public int Total { get { return Edits.Sum(x => x.Total); } }


        public IEnumerable<KeyValuePair<string,string>> AsFormData {
            get
            {
                var items = new Collection<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("f", "json"),
                        new KeyValuePair<string, string>("rollbackOnFailure", "true"),
                        new KeyValuePair<string, string>("edits", JsonConvert.SerializeObject(Edits)),
                    };

                return items;
            }
        }
    }

}