using System;

namespace SurveySync.Soe.Models.Configuration {

    public class ApplicationFields {
        /// <summary>
        ///     Gets or sets the return fields.
        /// </summary>
        /// <value>
        ///     The fields to get the values from and return from the service.
        /// </value>
        public string[] ReturnFields { get; set; }

        /// <summary>
        ///     Gets or sets the property identifier.
        /// </summary>
        /// <value>
        ///     The property identifier.
        /// </value>
        public string PropertyId { get; set; }

        /// <summary>
        ///     Gets or sets the survey identifier.
        /// </summary>
        /// <value>
        ///     The survey identifier.
        /// </value>
        public string SurveyId { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("ReturnFields: {0},{3} PropertyId: {1},{3} SurveyId: {2}", ReturnFields, PropertyId, SurveyId, Environment.NewLine);
        }
    }

}