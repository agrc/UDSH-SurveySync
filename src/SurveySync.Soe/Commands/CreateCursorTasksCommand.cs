using System.Collections.Generic;
using System.Data;
using System.Linq;
using SurveySync.Soe.Infastructure.Commands;
using SurveySync.Soe.Models;

namespace SurveySync.Soe.Commands {

    public class CreateCursorTasksCommand : Command<Actions> {
        private readonly IEnumerable<Schema> _buildings;
        private readonly IEnumerable<Schema> _contributions;

        public CreateCursorTasksCommand(IEnumerable<Schema> contributions, IEnumerable<Schema> buildings)
        {
            _contributions = contributions;
            _buildings = buildings;
        }

        /// <summary>
        ///     code to execute when command is run.
        /// </summary>
        protected override void Execute()
        {
            var buildings = _buildings.Select(x => x.PropertyId).ToList();
            var contribs = _contributions.Select(x => x.PropertyId).ToList();

            var intersection = contribs.Intersect(buildings);
            var difference = contribs.Except(buildings);

            var actions = new Actions
                {
                    Create = _contributions.Where(x => difference.Contains(x.PropertyId)),
                    Update = _buildings.Where(x => intersection.Contains(x.PropertyId))
                };

            if (actions.Total != _contributions.Count())
            {
                throw new ConstraintException(
                    "Total number of actions created does not equal the number of actions required to " +
                    "complete this request. Corrupt data issue.");
            }

            if (actions.Total == 0)
            {
                Result = null;
                return;
            }

            Result = actions;
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, Contributions: {1}, Buildings: {2}", "CreateCursorTasksCommand", _contributions,
                                 _buildings);
        }
    }

}