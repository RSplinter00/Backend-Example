using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System;
using Splinter.BackendExample.Services;
using System.Net.Http;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Splinter.BackendExample.Functions.Authorization
{
    public static class GetAccessTokenHttpTrigger
    {
        /// <summary>
        /// Returns the access token of a user.
        /// If a user does not have an access token, a new token is created.
        /// </summary>
        /// <param name="req">
        /// Http request message which calls the function.
        /// The request must contain a parameter 'AccountId' with the user's LocalAccountId.
        /// </param>
        /// <param name="log">The logger used to write logs to Azure.</param>
        /// <returns>An Http response message with the user's access token.</returns>
        [FunctionName(nameof(GetAccessTokenHttpTrigger))]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                var accountId = req.Query["accountId"];

                // Check if a valid parameter is given.
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    log.LogInformation("Request sent with an invalid account id.");
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                // Request the access token.
                var accessToken = KeyVaultService.GetAccessToken(accountId);

                // If no access token has been found, create a new token.
                if (string.IsNullOrWhiteSpace(accessToken?.Value))
                {
                    log.LogInformation($"New Access Token created for user {accountId}.");
                    accessToken = KeyVaultService.SetAccessToken(accountId);
                }
                
                // Return the access token if the user has one.
                if (accessToken?.Value != null) 
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(accessToken.Value)
                    };
            }
            catch (Exception e)
            {
                log.LogError(e, "Unable to get an access token.");
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }
}
