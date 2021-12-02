using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace Splinter.BackendExample.Functions.Users
{
    public static class GetGraphAPITokenHttpTrigger
    {
        /// <summary>
        /// Returns the access token required to call Graph API.
        /// </summary>
        /// <param name="req">Http request message which calls the function.</param>
        /// <param name="log">The logger used to write logs to Azure.</param>
        /// <returns>An Http response message with the access token for Graph API.</returns>
        [FunctionName("GetGraphAPITokenHttpTrigger")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                var client = new HttpClient();
                // Create the body, consisting of Key-Value pairs for the request.
                var body = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("client_id", Environment.GetEnvironmentVariable("FreeboardClientId")),
                    new KeyValuePair<string, string>("scope", Constants.GraphApiScope),
                    new KeyValuePair<string, string>("client_secret", Environment.GetEnvironmentVariable("FreeboardClientSecret")),
                    new KeyValuePair<string, string>("grant_type", Constants.GraphApiGrantType),
                };

                // Request the access token.
                var response = await client.PostAsync(Constants.GraphApiTokenUrl, new FormUrlEncodedContent(body));
                var content = await response.Content.ReadAsAsync<JObject>();

                if (content.HasValues && content.ContainsKey("access_token"))
                {
                    // If the request's content contains the key 'access_token',
                    // read the value and return it.
                    var token = content.GetValue("access_token");

                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(JsonConvert.SerializeObject(token))
                    };
                }

                log.LogInformation("Response did not contain key 'access_token'.");
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                log.LogError(e, "Unable to get Graph Api access token.");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}
