using System;
using System.Collections.Generic;
using System.Reflection;
using CommandPattern;
using SurveySync.Soe.Attributes;
using SurveySync.Soe.Extensions;

namespace SurveySync.Soe.Commands {

    /// <summary>
    ///     Command that finds all classes that are decorate by the Endpoint attribute
    /// </summary>
    public class FindAllEndpointsCommand : Command<IEnumerable<Type>> {
        /// <summary>
        ///     The _assembly to scan
        /// </summary>
        private readonly Assembly _assemblyToScan;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FindAllEndpointsCommand" /> class.
        /// </summary>
        /// <param name="assemblyToScan"> The assembly to scan. </param>
        public FindAllEndpointsCommand(Assembly assemblyToScan)
        {
            _assemblyToScan = assemblyToScan;
        }

        /// <summary>
        ///     code to execute when command is run.
        /// </summary>
        public override void Execute()
        {
            Result = _assemblyToScan.FindTypesWithAttribute(typeof (EndpointAttribute));
        }

        public override string ToString()
        {
            return "FindAllEndpointsCommand";
        }
    }

}