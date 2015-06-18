using System.Collections.Generic;
using System.Data;
using System.Linq;
using CommandPattern;
using ESRI.ArcGIS.SOESupport;
using SurveySync.Soe.Extensions;
using SurveySync.Soe.Models.FeatureService;

namespace SurveySync.Soe.Commands {

    public class CreateApplyEditActionsCommand : Command<EditContainer> {
        private readonly IEnumerable<FeatureAction> _buildings;
        private readonly int _buildingLayerIndex;
        private readonly IEnumerable<FeatureAction> _contributions;
        private readonly int _contribLayerIndex;

        public CreateApplyEditActionsCommand(IEnumerable<FeatureAction> contributions, int contribLayerIndex,
                                             IEnumerable<FeatureAction> buildings, int buildingLayerIndex)
        {
            _contributions = contributions;
            _contribLayerIndex = contribLayerIndex;
            _buildings = buildings;
            _buildingLayerIndex = buildingLayerIndex;
        }

        /// <summary>
        /// code to execute when command is run.
        /// </summary>
        /// <exception cref="System.Data.ConstraintException">Total number of actions created does not equal the number of actions required to  +
        ///                     complete this request. Corrupt data issue.</exception>
        public override void Execute()
        {
            var editActions = new ApplyEditActions {Id = _buildingLayerIndex};
            var deleteActions = new ApplyEditActions {Id = _contribLayerIndex};

#if !DEBUG
            Logger.LogMessage(ServerLogger.msgType.infoSimple, "CreateApplyEditActionsCommand", 2472, "edit action total {0}".With(editActions.Total));
            Logger.LogMessage(ServerLogger.msgType.infoSimple, "CreateApplyEditActionsCommand", 2472, "delete action total {0}".With(deleteActions.Total));
#endif

            var buildings = _buildings.Select(x => x.Attributes["PropertyId"]).ToList();
            var contribs = _contributions.Select(x => x.Attributes["PropertyId"]).ToList();

            var intersection = contribs.Intersect(buildings);
            var difference = contribs.Except(buildings);

            var recordsToUpdate = _buildings.Where(x => intersection.Contains(x.Attributes["PropertyId"])).ToList();

#if !DEBUG
            Logger.LogMessage(ServerLogger.msgType.infoSimple, "CreateApplyEditActionsCommand", 2472, "records to update".With(recordsToUpdate.Count));
#endif

            editActions.Adds = _contributions.Where(x => difference.Contains(x.Attributes["PropertyId"])).ToList();
            editActions.Updates =
                CommandExecutor.ExecuteCommand(new UpdateBuildingAttributesCommand(_contributions, recordsToUpdate));
            deleteActions.Deletes = _contributions.Select(x => x.Attributes["OBJECTID"]).Cast<int>().ToList();

            var total = editActions.Total;
            if (total != _contributions.Count())
            {
                throw new ConstraintException(
                    "Total number of actions created does not equal the number of actions required to " +
                    "complete this request. Corrupt data issue.");
            }

            if (total == 0)
            {
                Result = null;
                return;
            }

            Result = new EditContainer(new[] {editActions, deleteActions});
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "CreateApplyEditActionsCommand";
        }
    }

}