using System;
using System.Collections.Generic;
using System.Net.Http;
using CommandPattern;
using Containers;
using Newtonsoft.Json;
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
            return string.Format("{0}, Url: {1}", "SendEditsToFeatureServiceCommand", _url);
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
               var response = client.PostAsync(_url, new FormUrlEncodedContent(_data));

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