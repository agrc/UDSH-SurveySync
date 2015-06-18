using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Net;
using CommandPattern;
using Containers;
using ESRI.ArcGIS.SOESupport;
using SurveySync.Soe.Attributes;
using SurveySync.Soe.Cache;
using SurveySync.Soe.Commands;
using SurveySync.Soe.Commands.Http;
using SurveySync.Soe.Commands.Searches;
using SurveySync.Soe.Commands.Sql;
using SurveySync.Soe.Extensions;
using SurveySync.Soe.Infastructure.Endpoints;
using SurveySync.Soe.Models;
using SurveySync.Soe.Models.FeatureService;

namespace SurveySync.Soe.Endpoints {

    /// <summary>
    ///     A rest endpoint. All endpoints are marked with the [Endpoint] attribute and dynamically added to the implmentation
    ///     at registration time
    /// </summary>
    [Endpoint]
    public class SurveySyncEndpoint : JsonEndpoint, IRestEndpoint {
        /// <summary>
        /// Makes the endpoint answer only over http POST requests
        /// </summary>
        private const bool PostOnly = true;

        /// <summary>
        ///     The resource name that displays for Supported Operations
        /// </summary>
        private const string ResourceName = "Create";

        #region IRestEndpoint Members

        /// <summary>
        ///     A method that the dynamic endpoint setup uses for registering the rest endpoing operation details.
        /// </summary>
        /// <returns> </returns>
        public RestOperation RestOperation()
        {
            return new RestOperation(ResourceName,
                                     new[]
                                         {
                                             "surveyId"
                                         },
                                     new[]
                                         {
                                             "json"
                                         },
                                     Handler,
                                     PostOnly);
        }

        #endregion

        /// <summary>
        ///     Handles the incoming rest requests
        ///     Takes the incomming survey id and queries the PROPERTYSURVEYRECORD table to get the property ids
        ///     Uses the resulting id's to query CONTRIBUTION_PROPERTY_POINT for data
        ///     Update and create the records in BUILDINGS
        ///     If successful delete CONTRIBUTION_PROPERTY_POINT records and return update/creation counts
        /// </summary>
        /// <param name="boundVariables"> The bound variables. </param>
        /// <param name="operationInput"> The operation input. </param>
        /// <param name="outputFormat"> The output format. </param>
        /// <param name="requestProperties"> The request properties. </param>
        /// <param name="responseProperties"> The response properties. </param>
        /// <returns> </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static byte[] Handler(NameValueCollection boundVariables, JsonObject operationInput,
                                     string outputFormat, string requestProperties,
                                     out string responseProperties)
        {
            var logger = new ServerLogger();
            responseProperties = null;
            var errors = new ResponseContainer(HttpStatusCode.BadRequest, "");

#if !DEBUG
            logger.LogMessage(ServerLogger.msgType.infoSimple, "SurveyCreateEndpoint.Sync", 2472, "Inside Create rest handler");
#endif
            double surveyId;
            try
            {
                surveyId = operationInput.GetNumberValue("surveyId");
            }
            catch (ArgumentException)
            {
                errors.Message = "Must contain 'surveyId' to find properties";
#if !DEBUG
                logger.LogMessage(ServerLogger.msgType.infoSimple, "SurveyCreateEndpoint.Sync", 2472, errors.Message);
#endif
                return Json(errors);
            }

#if !DEBUG
            logger.LogMessage(ServerLogger.msgType.infoSimple, "SurveyCreateEndpoint.Sync", 2472, "Quering for propertyid {0}".With(surveyId));
#endif

            var propertyIds =
                CommandExecutor.ExecuteCommand(new GetPropertyIdsFromSurveyCommand(ApplicationCache.Settings, surveyId));

            if (propertyIds == null)
            {
                errors.Message = "No properties found for survey {0}".With(surveyId);
#if !DEBUG
                logger.LogMessage(ServerLogger.msgType.infoSimple, "SurveyCreateEndpoint.Sync", 2472, errors.Message);
#endif
                return Json(errors);
            }

#if !DEBUG
            logger.LogMessage(ServerLogger.msgType.infoSimple, "SurveyCreateEndpoint.Sync", 2472, "{0} property id's found".With(propertyIds.Length));
#endif

            var propertyIdWhereClause =
                CommandExecutor.ExecuteCommand(
                    new ComposeInSetQueryCommand(ApplicationCache.Settings.ContributionPropertyPointFields.PropertyId,
                                                 propertyIds.Cast<object>()));

            if (propertyIdWhereClause == null)
            {
                errors.Message = "No properties found for survey {0}. {1}".With(surveyId, propertyIds);
#if !DEBUG
                logger.LogMessage(ServerLogger.msgType.infoSimple, "SurveyCreateEndpoint.Sync", 2472, errors.Message);
#endif
                return Json(errors);
            }

#if !DEBUG
            logger.LogMessage(ServerLogger.msgType.infoSimple, "SurveyCreateEndpoint.Sync", 2472, "PropertyId Where clause {0}".With(propertyIdWhereClause));
#endif

            var contributionFeatureClass =
                ApplicationCache.FeatureClassIndexMap.Single(x => x.Name == ApplicationCache.Settings.ContributionPropertyPointLayerName);
            var buildingsFeatureClass =
                ApplicationCache.FeatureClassIndexMap.Single(x => x.Name == ApplicationCache.Settings.BuildingLayerName);
#if !DEBUG
            logger.LogMessage(ServerLogger.msgType.infoSimple, "SurveyCreateEndpoint.Sync", 2472, "Reference to cpp and buildings received.");
#endif
            Collection<FeatureAction> contributions;
            try
            {
                contributions = CommandExecutor.ExecuteCommand(
                    new GetRecordsCommand(contributionFeatureClass.FeatureClass, propertyIdWhereClause));
            }
            catch (ArgumentException ex)
            {
                errors.Message = ex.Message;
#if !DEBUG
                logger.LogMessage(ServerLogger.msgType.infoSimple, "SurveyCreateEndpoint.Sync", 2472, errors.Message);
#endif
                return Json(errors);
            }

#if !DEBUG
            logger.LogMessage(ServerLogger.msgType.infoSimple, "SurveyCreateEndpoint.Sync", 2472, "{0} contributions found".With(contributions.Count));
#endif

            Collection<FeatureAction> buildings;
            try
            {
                buildings = CommandExecutor.ExecuteCommand(
                    new GetRecordsCommand(buildingsFeatureClass.FeatureClass, propertyIdWhereClause));
            }
            catch (ArgumentException ex)
            {
                errors.Message = ex.Message;
#if !DEBUG
                logger.LogMessage(ServerLogger.msgType.infoSimple, "SurveyCreateEndpoint.Sync", 2472, errors.Message);
#endif
                return Json(errors);
            }

#if !DEBUG
            logger.LogMessage(ServerLogger.msgType.infoSimple, "SurveyCreateEndpoint.Sync", 2472, "{0} buildings found".With(buildings.Count));
#endif

            EditContainer container;
            try
            {
                container = CommandExecutor.ExecuteCommand(new CreateApplyEditActionsCommand(contributions, contributionFeatureClass.Index, buildings, buildingsFeatureClass.Index));
            }
            catch (ConstraintException ex)
            {
                errors.Message = ex.Message;
#if !DEBUG
                logger.LogMessage(ServerLogger.msgType.infoSimple, "SurveyCreateEndpoint.Sync", 2472, errors.Message);
#endif
                return Json(errors);
            }
            catch (InvalidOperationException ex)
            {
                errors.Message = ex.Message;
#if !DEBUG
                logger.LogMessage(ServerLogger.msgType.infoSimple, "SurveyCreateEndpoint.Sync", 2472, errors.Message);
#endif
                return Json(errors);
            }

            if (container == null || container.Total == 0)
            {
                return Json(new ResponseContainer(HttpStatusCode.OK, "No actions taken."));
            }

#if !DEBUG
            logger.LogMessage(ServerLogger.msgType.infoSimple, "SurveyCreateEndpoint.Sync", 2472, "sending {0} edits".With(container.Total));
#endif

            var result = CommandExecutor.ExecuteCommand(new SendEditsToFeatureServiceCommand(container, ApplicationCache.Settings.FeatureServiceEditingUrl));

            var response = new SoeResponse(result);

            var code = response.Successful ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
            var message = response.Message;

            return Json(new ResponseContainer<SoeResponse>(response, code, message));
        }
    }

}