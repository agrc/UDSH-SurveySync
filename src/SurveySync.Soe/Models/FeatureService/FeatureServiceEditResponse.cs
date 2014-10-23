using System.Collections.Generic;

namespace SurveySync.Soe.Models.FeatureService {

    public class FeatureServiceEditResponse {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public FeatureServiceEditResponse()
        {
            AddResults = new List<FeatureServiceActionResponse>();
            UpdateResults = new List<FeatureServiceActionResponse>();
            DeleteResults = new List<FeatureServiceActionResponse>();
        }

        /// <summary>
        /// Gets or sets the layer identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the results of the insert operation.
        /// </summary>
        /// <value>
        /// The add results.
        /// </value>
        public List<FeatureServiceActionResponse> AddResults { get; set; }

        /// <summary>
        /// Gets or sets the results of the update operation.
        /// </summary>
        /// <value>
        /// The update results.
        /// </value>
        public List<FeatureServiceActionResponse> UpdateResults { get; set; }

        /// <summary>
        /// Gets or sets the results of the delete operation.
        /// </summary>
        /// <value>
        /// The delete results.
        /// </value>
        public List<FeatureServiceActionResponse> DeleteResults { get; set; }
    }

}