using System;
using ESRI.ArcGIS;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace SurveySync.Soe.Tests {

    public static class TestHelper {
        public static IWorkspace GetSdeWorkspace(string connectionString)
        {
            RuntimeManager.Bind(ProductCode.Server);

            var init = new AoInitializeClass();
            init.Initialize(esriLicenseProductCode.esriLicenseProductCodeArcServer);

            var factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
            var workspaceFactory2 = (IWorkspaceFactory2)Activator.CreateInstance(factoryType);

            return workspaceFactory2.OpenFromString(connectionString, 0);
        }

    }

}