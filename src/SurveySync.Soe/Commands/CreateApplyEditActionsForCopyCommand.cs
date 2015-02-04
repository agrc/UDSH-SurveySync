using System.Collections.Generic;
using CommandPattern;
using SurveySync.Soe.Models.FeatureService;

namespace SurveySync.Soe.Commands
{

    public class CreateApplyEditActionsForCopyCommand : Command<EditContainer>
    {
        private readonly ICollection<FeatureAction> _buildings;
        private readonly int _contribLayerIndex;

        public CreateApplyEditActionsForCopyCommand(int contribLayerIndex, ICollection<FeatureAction> buildings)
        {
            _contribLayerIndex = contribLayerIndex;
            _buildings = buildings;
        }

        /// <summary>
        ///     code to execute when command is run.
        /// </summary>
        public override void Execute()
        {
            var addActions = new ApplyEditActions { 
                Id = _contribLayerIndex,
                Adds = _buildings
            };

            var total = addActions.Total;

            if (total == 0)
            {
                Result = null;
                return;
            }

            Result = new EditContainer(new[] { addActions });
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, Buildings: {1}", "CreateApplyEditActionsForCopyCommand", _buildings);
        }
    }

}