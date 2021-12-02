using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Splinter.BackendExample.Models;
using System.Net.Http;
using System.Net;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System.Linq;

namespace Splinter.BackendExample.Functions
{
    public static class SearchPostHttpTrigger
    {
        /// <summary>
        /// Retrieves <see cref="Post"/>s for a given list of parameters.
        /// </summary>
        /// <param name="req">
        /// Http request message which calls the function.
        /// Must contain a parameter 'searchTerms', containing a string of search terms, separated by a ','.
        /// May contain paramaters 'count' and 'offset' to set the number of returned posts and how many posts to skip, respectively.
        /// </param>
        /// <param name="table">The storage table where the posts are stored.</param>
        /// <param name="log">The logger used to write logs to Azure.</param>
        /// <returns>An Http response message with the posts.</returns>
        [FunctionName(nameof(SearchPostHttpTrigger))]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [Table("Posts")] CloudTable table,
            ILogger log)
        {
            try
            {
                // Read the request parameters.
                string searchTermsParameters = req.Query["searchTerms"];
                int.TryParse(req.Query["count"], out int count);
                int.TryParse(req.Query["offset"], out int offset);

                // If count was not set, use the default value.
                if (count <= 0) count = Constants.ReturnEntitiesUpperBound;

                if (string.IsNullOrWhiteSpace(searchTermsParameters))
                {
                    // Check for valid search terms.
                    log.LogInformation($"{nameof(SearchPostHttpTrigger)} called with missing parameter.");
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                // Split the search terms on ','.
                var searchTerms = searchTermsParameters.ToLower().Split(',');

                // Retrieve all posts from the storage.
                var query = new TableQuery<Post>();
                var posts = new List<Post>(table.ExecuteQuery(query));

                // Search for all posts which contains a search term in its title or highlights.
                posts = posts.FindAll(post =>
                    searchTerms.Any(searchTerm => 
                        post.Title.ToLower().Contains(searchTerm) ||
                        post.SerializedHighlights.ToLower().Contains(searchTerm)));

                // Create the search results.
                var searchResults = new SearchResults<Post>(posts);

                // Partition the search results and deserialize the properties.
                searchResults.PartitionResults(offset, count);
                foreach (Post post in searchResults.Results) post.DeserializeHightlights();

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(searchResults))
                };
            }
            catch (Exception e)
            {
                log.LogError(e, "Failed to search post.");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}
