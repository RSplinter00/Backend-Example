using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Splinter.BackendExample.Models
{
    /// <summary>
    /// Represents the results of a search request.
    /// </summary>
    /// <typeparam name="T">The type of element of the results.</typeparam>
    public class SearchResults<T>
    {
        public int TotalSearchResults { get; set; }
        public int ShowingFrom { get; set; }
        public int ShowingUntil { get; set; }
        public List<T> Results { get; set; }

        public SearchResults(List<T> results)
        {
            Results = results;
            TotalSearchResults = Results.Count;
        }

        /// <summary>
        /// Overwrites the <see cref="SearchResults{T}.Results"/> with a partition and sets which indices are shown.
        /// </summary>
        /// <param name="offset">The index where the partition has to start.</param>
        /// <param name="count">The maximum number of items to return.</param>
        public void PartitionResults(int offset, int count)
        {
            try
            {
                if (offset >= Results.Count)
                {
                    // If the offset is greater then or equal to the number of results,
                    // No results are returned.
                    Results.Clear();
                    ShowingFrom = 0;
                    ShowingUntil = 0;
                    return;
                }
                else if (count + offset > Results.Count)
                {
                    // If the sum of count and offset is greater then the number of results,
                    // Calculate the number of items to return.
                    count = Results.Count - offset;
                }

                // Partition the results and set the indices.
                Results = Results.GetRange(offset, count);
                ShowingFrom = offset + 1;
                ShowingUntil = offset + count;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
