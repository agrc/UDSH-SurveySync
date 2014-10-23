namespace SurveySync.Soe.Models.FeatureService {

    public class FeatureServiceActionResponse {
        public int ObjectId { get; set; }
        public bool Success { get; set; }
        public FeatureServiceError Error { get; set; }
    }

}