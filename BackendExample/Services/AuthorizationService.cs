using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;

namespace Splinter.BackendExample.Services
{
    /// <summary>
    /// The <see cref="AuthorizationService"/> handles authorization checks on users to prevent unauthorized access.
    /// </summary>
    public class AuthorizationService
    {
        private AuthorizationService()
        { }

        /// <summary>
        /// Retrieves the access token from a header dictionary.
        /// </summary>
        /// <param name="headers">The headers from an HttpRequest.</param>
        /// <returns>The access token if it can be found; otherwise an empty string.</returns>
        private static string GetAccessToken(IHeaderDictionary headers)
        {
            try
            {
                var accessToken = "";

                if (headers.ContainsKey("Authorization"))
                {
                    // If an authorization header exists, extract the access token.
                    headers.TryGetValue("Authorization", out var bearerToken);
                    accessToken = bearerToken[0].Replace("Bearer ", "").Trim();
                }

                return accessToken;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return "";
            }
        }

        /// <summary>
        /// Checks if a user is authorized to perform an action.
        /// </summary>
        /// <param name="accountId">Account id belonging to the actual access token.</param>
        /// <param name="headers">The header dictionary retrieved from the request.</param>
        /// <returns>true if the access token belongs to the given account id; otherwise, false.</returns>
        public static bool IsAuthorized(string accountId, IHeaderDictionary headers)
        {
            try
            {
                var accessToken = GetAccessToken(headers);

                // Check if an accountId and accessToken are given.
                if (string.IsNullOrWhiteSpace(accountId) || string.IsNullOrWhiteSpace(accessToken)) return false;

                // Retrieve the access token belonging to the account id.
                var postAccessToken = KeyVaultService.GetAccessToken(accountId);

                // Check if an access token has been found
                if (string.IsNullOrWhiteSpace(postAccessToken?.Value)) return false;

                // Check if the given access token is the same as that belonging to the given account id.
                var result = accessToken.Equals(postAccessToken.Value);
                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
    }
}
