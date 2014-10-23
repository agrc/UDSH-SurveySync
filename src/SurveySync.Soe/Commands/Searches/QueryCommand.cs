using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SurveySync.Soe.Cache;
using SurveySync.Soe.Infastructure.Commands;
using SurveySync.Soe.Models;
using SurveySync.Soe.Models.Search;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace SurveySync.Soe.Commands.Searches {
    public class QueryCommand : Command {
        private readonly IEnumerable<SearchLayerProperties> _layerProperties;

        private readonly ISpatialFilter _queryFilter;

        public QueryCommand(ISpatialFilter queryFilter, IEnumerable<SearchLayerProperties> layerProperties) {
            _queryFilter = queryFilter;
            _layerProperties = layerProperties;
        }

        protected override void Execute() {
//            var results = _layerProperties
//                .Select(x => new {
//                    map = ApplicationCache.FeatureClassIndexMap.Single(y => y.Index == x.Index),
//                    definitionExpression = x.DefQuery
//                })
//                .ToDictionary(result => result.map.Index,
//                              result => Query(result.map.FeatureClass, result.map, result.definitionExpression));
//
//            Result = results;
        }

//        private IEnumerable<Graphic> Query(IFeatureClass featureClass, FeatureClassIndexMap map,
//                                           string definitionExpression) {
//             var featureLayer = new FeatureLayer {
//                    FeatureClass = featureClass
//                };
//
//            if (!string.IsNullOrEmpty(definitionExpression)) {
//                var featureDefinition = featureLayer as IFeatureLayerDefinition2;
//                if (featureDefinition != null) {
//                    featureDefinition.DefinitionExpression = definitionExpression;
//                }
//            }
//
//            var cursor = featureLayer.Search(_queryFilter, true);
//
//            var results = new List<Graphic>();
//
//            IFeature feature;
//            while ((feature = cursor.NextFeature()) != null) {
//                var indexes = map.FieldMap.Values.ToList();
//
//                var attributes = CommandExecutor.ExecuteCommand(new GetValueAtIndexCommand(indexes, feature))
//                                                .ToDictionary(k => k.Key, v => v.Value);
//                var geometry =
//                    CommandExecutor.ExecuteCommand(new CreateJsFormatGeometryCommand(feature.ShapeCopy,
//                                                                                     feature.ShapeCopy.SpatialReference));
//
//                var graphic = new Graphic(geometry, attributes);
//
//                results.Add(graphic);
//            }
//
//            Marshal.ReleaseComObject(cursor);
//
//            return results;
//        }

        public override string ToString() {
            return string.Format("{0}, Query: {1}", "QueryCommand", _queryFilter);
        }
    }
}
