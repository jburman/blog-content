using Microsoft.Identity.Client;
using System;
using System.Linq;

namespace MSALFromConsoleApp
{
    // Some example code for authenticating for Graph API access via MSAL
    // Uses local cache so that multiple runs do not need to pop authentication dialog.
    // Uses MSAL nuget package: install-package Microsoft.Identity.Client -IncludePrerelease
    class Program
    {
        static void Main(string[] args)
        {
            string clientId = "CLIENT ID HERE";

            // Example scopes
            string[] scopes = new[] { "https://graph.microsoft.com/user.read https://graph.microsoft.com/files.read" };

            PublicClientApplication myApp = new PublicClientApplication(clientId, "https://login.microsoftonline.com/common", TokenCacheHelper.GetUserCache());
            var user = myApp.Users.FirstOrDefault();
            AuthenticationResult auth = null;
            string token = null;

            if (user != null)
            {
                Console.WriteLine("Getting token from cache: " + user.Name);
                auth = myApp.AcquireTokenSilentAsync(scopes, user).GetAwaiter().GetResult();
                if (auth != null && !string.IsNullOrEmpty(auth.IdToken))
                    token = auth.IdToken;
            }
            else
                Console.WriteLine("No token found in cache");

            if (token == null)
            {
                auth = myApp.AcquireTokenAsync(scopes).GetAwaiter().GetResult();
                token = auth.IdToken;
            }

            Console.WriteLine("Authenticated: " + auth.User?.Name);
        }
    }
}
