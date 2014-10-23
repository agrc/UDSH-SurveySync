using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace SurveySync.Soe.Models.FeatureService {

    public class ApplyEditActions {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public ApplyEditActions()
        {
            Adds = new Collection<FeatureAction>();
            Updates = new Collection<FeatureAction>();
            Deletes = new Collection<int>();
        }

        /// <summary>
        ///     Gets or sets the layer index in the mxd.
        /// </summary>
        /// <value>
        ///     The layer index in the mxd.
        /// </value>
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the actions the will be created in the layer.
        /// </summary>
        /// <value>
        ///     The features to be added.
        /// </value>
        [JsonProperty(PropertyName = "adds", NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<FeatureAction> Adds { get; set; }

        /// <summary>
        ///     Gets or sets the update actions. These actions must have an object id
        /// </summary>
        /// <value>
        ///     The features update.
        /// </value>
        [JsonProperty(PropertyName = "updates", NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<FeatureAction> Updates { get; set; }

        /// <summary>
        ///     Gets or sets the features to delete by ObjectId.
        /// </summary>
        /// <value>
        ///     A list of Object id's to delete from the feature class.
        /// </value>
        [JsonProperty(PropertyName = "deletes", NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<int> Deletes { get; set; }

        [JsonIgnore]
        public int Total
        {
            get { return Adds.Count + Updates.Count; }
        }

        public bool ShouldSerializeAdds()
        {
            return Adds.Count > 0;
        }

        public bool ShouldSerializeUpdates()
        {
            return Updates.Count > 0;
        }

        public bool ShouldSerializeDeletes()
        {
            return Deletes.Count > 0;
        }
    }

}