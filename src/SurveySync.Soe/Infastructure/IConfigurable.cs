using ESRI.ArcGIS.esriSystem;
using SurveySync.Soe.Models.Configuration;

namespace SurveySync.Soe.Infastructure {

    public interface IConfigurable {
        ApplicationSettings GetSettings(IPropertySet set);
    }

}