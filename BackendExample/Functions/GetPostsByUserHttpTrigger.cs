using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Azure.Cosmos.Table;
using Splinter.BackendExample.Models;
using System.Net;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Splinter.BackendExample.Functions
{
    public static class GetPostsByUserHttpTrigger
    {
        /// <summary>
        /// Retrieves up to 20 <see cref="Post"/>s created by a specified user.
        /// </summary>
        /// <param name="req">
        /// Http request message which calls the function.
        /// The request must contain a parameter 'accountId' with the user's LocalAccountId.
        /// </param>
        /// <param name="table">The storage table where the post is stored.</param>
        /// <param name="log">The logger used to write logs to Azure.</param>
        /// <returns>An Http response message with the posts.</returns>
        [FunctionName(nameof(GetPostsByUserHttpTrigger))]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [Table("Posts")] CloudTable table,
            ILogger log)
        {
            try
            {
                // Get the user name.
                string user = req.Query["accountId"];

                // Check if a valid username is given.
                if (string.IsNullOrWhiteSpace(user))
                {
                    log.LogInformation("Request contains invalid value for the account id.");
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                // Generate a filter on the partition key to get the posts from the user.
                var filterConditionOnUser = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, user);

                // Create and execute the query to retrieve posts of the given user.
                var query = new TableQuery<Post>().Where(filterConditionOnUser).Take(Constants.ReturnEntitiesUpperBound);
                var results = new List<Post>(table.ExecuteQuery(query));

                // Deserialize the user and highlights for each post.
                foreach (Post post in results) post.DeserializeHightlights();

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(results))
                };
            }
            catch (Exception e)
            {
                log.LogError(e, $"Unable to retrieve posts for a user.");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}
