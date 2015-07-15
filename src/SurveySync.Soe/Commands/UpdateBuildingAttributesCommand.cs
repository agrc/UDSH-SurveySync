using System;
using System.Collections.Generic;
using System.Linq;
using CommandPattern;
using SurveySync.Soe.Models.FeatureService;

namespace SurveySync.Soe.Commands {

    public class UpdateBuildingAttributesCommand : Command<ICollection<FeatureAction>> {
        private readonly IEnumerable<FeatureAction> _destination;
        private readonly IEnumerable<FeatureAction> _source;

        public UpdateBuildingAttributesCommand(IEnumerable<FeatureAction> source, IEnumerable<FeatureAction> destination)
        {
            _source = source;
            _destination = destination;
        }

        /// <summary>
        ///     code to execute when command is run.
        /// </summary>
        public override void Execute()
        {
            foreach (var feature in _destination)
            {
                var destinationPk = Convert.ToInt32(feature.Attributes["PropertyId"]);

                FeatureAction updatedFeature;
                try
                {
                    updatedFeature =
                        _source.DefaultIfEmpty(null).SingleOrDefault(
                            x => Convert.ToInt32(x.Attributes["PropertyId"]) == destinationPk);
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException(
                        string.Format("There is more than one record with the Property {0}", destinationPk), ex);
                }

                if (updatedFeature == null)
                {
                    throw new InvalidOperationException(
                        string.Format("No record found in the source data for Property {0}", destinationPk));
                }

                feature.Attributes = updatedFeature.Attributes;
                feature.Geometry = updatedFeature.Geometry;
            }

            Result = _destination.ToList();
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "UpdateBuildingAttributesCommand";
        }
    }

}