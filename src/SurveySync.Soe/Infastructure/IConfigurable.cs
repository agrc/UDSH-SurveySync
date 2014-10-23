using SurveySync.Soe.Models.Configuration;

namespace SurveySync.Soe.Infastructure {

    public interface IConfigurable {
        ApplicationSettings Settings { get; }
    }

}