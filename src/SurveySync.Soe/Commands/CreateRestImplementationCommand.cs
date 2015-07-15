using System;
using System.Reflection;
using CommandPattern;
using ESRI.ArcGIS.SOESupport;
using SurveySync.Soe.Endpoints;
using SurveySync.Soe.Extensions;
using SurveySync.Soe.Infastructure.Endpoints;

namespace SurveySync.Soe.Commands {

    /// <summary>
    ///     Creates the SOE rest implementation. Searches the assembly and adds all endpoints denoted
    ///     with the Endpoint attribute.
    /// </summary>
    public class CreateRestImplementationCommand : Command<SoeRestImpl> {
        private const string Restoperation = "RestOperation";
        private readonly Assembly _assemblyToScan;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CreateRestImplementationCommand" /> class.
        /// </summary>
        /// <param name="assembly"> The assembly. </param>
        public CreateRestImplementationCommand(Assembly assembly)
        {
            _assemblyToScan = assembly;
        }

        /// <summary>
        ///     code to execute when command is run. adds a default ResourceHandler and scans assembly
        ///     for all OperationHandlers marked with the EndpointAttribute
        /// </summary>
        public override void Execute()
        {
            var resource = new RestResource(typeof (VersionEndpoint).Assembly.FullName, false, VersionEndpoint.Handler);

            foreach (var type in CommandExecutor.ExecuteCommand(
                new FindAllEndpointsCommand(_assemblyToScan)))
            {
                if (!typeof (IRestEndpoint).IsAssignableFrom(type))
                {
                    continue;
                }

                var methodInfo = type.GetMethod(Restoperation);
                var instance = Activator.CreateInstance(type);

                var restOperation = methodInfo.Invoke(instance, null) as RestOperation;

                if (restOperation == null)
                {
#if !DEBUG
            Logger.LogMessage(ServerLogger.msgType.infoSimple, "CreateRestImplementationCommand.Execute", 2472, "Rest operation is null");
#endif
                    continue;
                }

#if !DEBUG
            Logger.LogMessage(ServerLogger.msgType.infoSimple, "CreateRestImplementationCommand.Execute", 2472, "Adding rest operation {0}".With(restOperation.name));
#endif

                resource.operations.Add(restOperation);
            }

            Result = new SoeRestImpl(Assembly.GetExecutingAssembly().FullName, resource);
        }

        public override string ToString()
        {
            return "CreateRestImplementationCommand";
        }
    }

}