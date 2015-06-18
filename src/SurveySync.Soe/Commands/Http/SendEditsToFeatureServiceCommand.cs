using System;
using System.Collections.Generic;
using System.Net.Http;
using CommandPattern;
using Containers;
using ESRI.ArcGIS.SOESupport;
using Newtonsoft.Json;
using SurveySync.Soe.Extensions;
using SurveySync.Soe.Models.FeatureService;

namespace SurveySync.Soe.Commands.Http {

    public class SendEditsToFeatureServiceCommand : Command<IList<FeatureServiceEditResponse>>
    {
        private readonly IEnumerable<KeyValuePair<string,string>> _data;
        private readonly string _url;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "SendEditsToFeatureServiceCommand";
        }

        public SendEditsToFeatureServiceCommand(EditContainer container, string url)
        {
            _url = url;
            _data = container.AsFormData;
        }

        /// <summary>
        ///     code to execute when command is run.
        /// </summary>
        public override void Execute()
        {
           using (var client = new HttpClient())
           {
#if !DEBUG
               Logger.LogMessage(ServerLogger.msgType.infoSimple, "SendEditsToFeatureServiceCommand", 2472, "_url: {0}".With(_url));
#endif
               HttpContent formContent = null;
               try
               {
                   formContent = new FormUrlEncodedContent(_data);
               }
               catch (FormatException fex)
               {
#if !DEBUG
                   Logger.LogMessage(ServerLogger.msgType.infoSimple, "SendEditsToFeatureServiceCommand", 2472, "POST format exception (exception swallowed)");
#endif

                   var tempContent = new MultipartFormDataContent();
                   foreach (var keyValuePair in _data)
                   {
                       tempContent.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                   }
                   formContent = tempContent;
               }
#if !DEBUG
               Logger.LogMessage(ServerLogger.msgType.infoSimple, "SendEditsToFeatureServiceCommand", 2472, "form encoded");
#endif

               var response = client.PostAsync(_url, formContent);

               var content = response.Result.Content.ReadAsStringAsync().Result;

               try
               {
                   Result = JsonConvert.DeserializeObject<IList<FeatureServiceEditResponse>>(content);
                   return;
               }
               catch (JsonSerializationException)
               {
                   //swallow 
               }

               try
               {
                   var error = JsonConvert.DeserializeObject<ResponseContainer>(content);
                   Result = new List<FeatureServiceEditResponse>
                       {
                           new FeatureServiceEditResponse
                               {
                                   Id = -1,
                                   AddResults = new List<FeatureServiceActionResponse>
                                       {
                                           new FeatureServiceActionResponse
                                               {
                                                   Error = new FeatureServiceError
                                                       {
                                                           Code = error.Error.Code,
                                                           Description = error.Error.Message
                                                       }
                                               }
                                       }
                               }
                       };
                   return;
               }
               catch (JsonSerializationException)
               {
                   // what just happened
               }

               Result = null;
           }
        }
    }

}