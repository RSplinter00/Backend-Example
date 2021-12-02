using Microsoft.Azure.Cosmos.Table;
using System;
using Newtonsoft.Json;
namespace Splinter.BackendExample.Models
{
    /// <summary>
    /// Represents the informatino of a comment belonging to a <seealso cref="Post"/>.
    /// </summary>
    public class Comment : TableEntity
    {
        [JsonProperty("_user")]
        public string User { get; set; }
        // Post = Post.RowKey
        [JsonProperty("_post")]
        public string Post { get; set; }
        [JsonProperty("_content")]
        public string Content { get; set; }
        [JsonProperty("_accountId")]
        public string AccountId { get; set; }
        [JsonProperty("_dateCreated")]
        public DateTime DateCreated { get; set; }
    }
}