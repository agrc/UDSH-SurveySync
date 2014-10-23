using System.Collections.ObjectModel;
using CommandPattern;
using SurveySync.Soe.Models;

namespace SurveySync.Soe.Commands {

    /// <summary>
    ///     Adds the field index property to the feature class to index map
    /// </summary>
    public class UpdateLayerMapWithFieldIndexMapCommand : Command {
        private readonly Collection<FeatureClassIndexMap> _map;

        public UpdateLayerMapWithFieldIndexMapCommand(Collection<FeatureClassIndexMap> map)
        {
            _map = map;
        }

        public override void Execute()
        {
            foreach (var item in _map)
            {
                var fields = new Collection<string>();
                for (var i = 0; i < item.FeatureClass.Fields.FieldCount; i++)
                {
                    // Get the field at the given index.
                    fields.Add(item.FeatureClass.Fields.Field[i].Name);
                }

                item.FieldMap =
                    CommandExecutor.ExecuteCommand(new FindIndexByFieldNameCommand(item.FeatureClass, fields));
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, Map count: {1}", "UpdateLayerMapWIthFieldIndexMapCommand", _map.Count);
        }
    }

}