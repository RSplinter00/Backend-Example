using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using System.Net.Http;
using System.Net;
using Splinter.BackendExample.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Splinter.BackendExample.Functions
{
    public static class GetCommentsByPostHttpTrigger
    {
        /// <summary>
        /// Retrieves up to 20 <see cref="Comment"/>s for a given post.
        /// </summary>
        /// <param name="req">Http request message which calls the function.</param>
        /// <param name="table">The storage table where the <see cref="Post"/> is stored.</param>
        /// <param name="log">The logger used to write logs to Azure.</param>
        /// <returns>An Http response message with the comments.</returns>
        [FunctionName(nameof(GetCommentsByPostHttpTrigger))]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [Table("Comments")] CloudTable table,
            ILogger log)
        {
            try
            {
                // Get the user name.
                string postId = req.Query["postId"];

                // Check if a valid postId is given.
                if (!Guid.TryParse(postId, out _))
                {
                    log.LogInformation("Request did not contain a valid postId.");
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                // Generate a filter on the partition key to get the posts from the user.
                var filterConditionOnUser = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, postId);

                // Create and execute the query to retrieve posts of the given user.
                var query = new TableQuery<Comment>().Where(filterConditionOnUser).Take(Constants.ReturnEntitiesUpperBound);
                var results = new List<Comment>(table.ExecuteQuery(query));

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(results))
                };
            }
            catch (Exception e)
            {
                log.LogError(e, "Unable to get an access token.");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}
