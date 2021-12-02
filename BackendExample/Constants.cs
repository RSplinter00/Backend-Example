using System;

namespace Splinter.BackendExample
{
    public class Constants
    {
        public static readonly string GraphApiTokenUrl = $"https://login.microsoftonline.com/{Environment.GetEnvironmentVariable("FreeboardTenantId")}/oauth2/v2.0/token";
        public static readonly string GraphApiScope = "https://graph.microsoft.com/.default";
        public static readonly string GraphApiGrantType = "client_credentials";
        public static readonly int ReturnEntitiesUpperBound = 20;
    }
}
