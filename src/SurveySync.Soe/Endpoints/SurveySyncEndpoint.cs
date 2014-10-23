using System;
using System.Collections.Generic;
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
                                     Handler);
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
            responseProperties = null;
            var errors = new ResponseContainer(HttpStatusCode.BadRequest, "");

            double surveyId;
            try
            {
                surveyId = operationInput.GetNumberValue("surveyId");
            }
            catch (ArgumentException)
            {
                errors.Message = "Must contain 'surveyId' to find properties";

                return Json(errors);
            }

            var propertyIds =
                CommandExecutor.ExecuteCommand(new GetPropertyIdsFromSurveyCommand(ApplicationCache.Settings, surveyId));

            if (propertyIds == null)
            {
                errors.Message = "No properties found for survey {0}".With(surveyId);

                return Json(errors);
            }

            var propertyIdWhereClause =
                CommandExecutor.ExecuteCommand(
                    new ComposeInSetQueryCommand(ApplicationCache.Settings.ContributionPropertyPointFields.PropertyId,
                                                 propertyIds.Cast<object>()));

            if (propertyIdWhereClause == null)
            {
                errors.Message = "No properties found for survey {0}. {1}".With(surveyId, propertyIds);

                return Json(errors);
            }

            var contributionFeatureClass =
                ApplicationCache.FeatureClassIndexMap.Single(x => x.Name == "cpp");
            var buildingsFeatureClass =
                ApplicationCache.FeatureClassIndexMap.Single(x => x.Name == "Buildings");

            Collection<FeatureAction> contributions;
            try
            {
                contributions = CommandExecutor.ExecuteCommand(
                    new GetRecordsCommand(contributionFeatureClass.FeatureClass, propertyIdWhereClause));
            }
            catch (ArgumentException ex)
            {
                errors.Message = ex.Message;
                return Json(errors);
            }

            Collection<FeatureAction> buildings;
            try
            {
                buildings = CommandExecutor.ExecuteCommand(
                    new GetRecordsCommand(buildingsFeatureClass.FeatureClass, propertyIdWhereClause));
            }
            catch (ArgumentException ex)
            {
                errors.Message = ex.Message;
                return Json(errors);
            }

            EditContainer container;
            try
            {
                container = CommandExecutor.ExecuteCommand(new CreateApplyEditActionsCommand(contributions, contributionFeatureClass.Index, buildings, buildingsFeatureClass.Index));
            }
            catch (ConstraintException ex)
            {
                errors.Message = ex.Message;
                return Json(errors);
            }
            catch (InvalidOperationException ex)
            {
                errors.Message = ex.Message;
                return Json(errors);
            }

            if (container == null || container.Total == 0)
            {
                return Json(new ResponseContainer(HttpStatusCode.OK, "No actions taken."));
            }

            var result = CommandExecutor.ExecuteCommand(new SendEditsToFeatureServiceCommand(container, ApplicationCache.Settings.FeatureServiceEditingUrl));

            var response = new SoeResponse(result);

            return Json(new ResponseContainer<SoeResponse>(response));
        }
    }

}