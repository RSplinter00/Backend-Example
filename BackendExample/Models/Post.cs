using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Splinter.BackendExample.Models
{
    /// <summary>
    /// Represents a job posting by a company to hire a freelancer for a vacancy.
    /// </summary>
    public class Post : TableEntity
    {
        [JsonProperty("_user")]
        public string User { get; set; }
        [JsonProperty("_title")]
        public string Title { get; set; }
        [JsonProperty("_content")]
        public string Content { get; set; }
        [JsonProperty("_image")]
        public string Image { get; set; }
        [JsonProperty("_highlights")]
        public string[] Highlights { get; set; }
        [JsonProperty("_visits")]
        public int Visits { get; set; }
        [JsonProperty("_dateCreated")]
        public DateTime DateCreated { get; set; }
        public string SerializedHighlights { get; set; }

        public Post()
        {
            DateCreated = DateTime.Now;
        }

        /// <summary>
        /// Serializes the Higlights property to a Json string value.
        /// </summary>
        public void SerializeHighlights()
        {
            try
            {
                if (Highlights != null) SerializedHighlights = JsonConvert.SerializeObject(Highlights);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Deserializes a Json string value to the Highlights property.
        /// </summary>
        public void DeserializeHightlights()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(SerializedHighlights))
                {
                    Highlights = JsonConvert.DeserializeObject<string[]>(SerializedHighlights);
                    SerializedHighlights = null;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}