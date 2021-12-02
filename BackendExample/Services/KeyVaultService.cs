using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Diagnostics;

namespace Splinter.BackendExample.Services
{
    /// <summary>
    /// The <see cref="KeyVaultService"/> handles operations with the key vault.
    /// It retrieves and sets secrets.
    /// </summary>
    public class KeyVaultService
    {
        private static readonly SecretClient Client = new SecretClient(vaultUri: new Uri(Environment.GetEnvironmentVariable("VaultUri")), credential: new DefaultAzureCredential());

        private KeyVaultService()
        { }

        /// <summary>
        /// Returns an access token from the Key Vault for a user.
        /// </summary>
        /// <param name="accountId">The accountId of the user.</param>
        /// <returns>The access token belonging to the user.</returns>
        public static KeyVaultSecret GetAccessToken(string accountId)
        {
            try
            {
                return Client.GetSecret(accountId).Value;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Sets a new access token for the given user.
        /// </summary>
        /// <param name="accountId">The accountId of the user.</param>
        /// <returns>The newly created access token belonging to the user.</returns>
        public static KeyVaultSecret SetAccessToken(string accountId)
        {
            try
            {
                // Generate an access token
                var accessToken = Guid.NewGuid();

                // Save the access token to the Key Vault and return the access token.
                return Client.SetSecret(accountId, accessToken.ToString());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
    }
}
