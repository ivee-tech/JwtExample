using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.JwtExample
{
    public class AzAuth
    {
        public async static Task<AuthenticationResult> GetAccessToken(string resourceUri)
        {
            //Get access token:   
            // To call a Data Catalog REST operation, create an instance of AuthenticationContext and call AcquireToken  
            // AuthenticationContext is part of the Active Directory Authentication Library NuGet package  
            // To install the Active Directory Authentication Library NuGet package in Visual Studio,   
            //  run "Install-Package Microsoft.IdentityModel.Clients.ActiveDirectory Version 2.19.208020213" from the nuget Package Manager Console.  

            //To learn how to register a client app and get a Client ID, see https://msdn.microsoft.com/en-us/library/azure/mt403303.aspx#clientID     
            string clientId = ConfigurationManager.AppSettings["ida:ClientId"];

            //A redirect uri gives AAD more details about the specific application that it will authenticate.  
            //Since a client app does not have an external service to redirect to, this Uri is the standard placeholder for a client app.  
            string redirectUri = "https://login.live.com/oauth20_desktop.srf";

            // Create an instance of AuthenticationContext to acquire an Azure access token  
            // OAuth2 authority Uri  
            //string authorityUri = "https://login.windows.net/common/oauth2/authorize";
            //string authorityUri = "https://login.microsoftonline.com/common/";
            string authorityUri = "https://login.microsoftonline.com/daradu/";
            AuthenticationContext authContext = new AuthenticationContext(authorityUri);

            // Call AcquireToken to get an Azure token from Azure Active Directory token issuance endpoint  
            //  AcquireToken takes a Client Id that Azure AD creates when you register your client app.  
            return await authContext.AcquireTokenAsync(resourceUri, clientId, new Uri(redirectUri), new PlatformParameters(PromptBehavior.Auto, null)); //, PromptBehavior.RefreshSession);
        }

        public async static Task<string> GetAccessToken()
        {
            var tokenProvider = new AzureServiceTokenProvider();
            var result = await tokenProvider.GetAccessTokenAsync("https://management.core.windows.net/", string.Empty);

            return result;
        }

        public async static Task<AuthenticationResult> GetAccessToken2(string resource)
        {
            var tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
            var parameters = new PlatformParameters(PromptBehavior.Auto);
            //string authorityUri = "https://login.microsoftonline.com/b29343ba-***/oauth2/authorize"; // Authorization endpoint
            //string authorityUri = string.Format("https://login.windows.net/{0}/oauth2/token", tenantId); //token endpoint
            string authorityUri = string.Format("https://login.microsoftonline.com/{0}", tenantId);
            string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
            string redirectUri = "https://login.live.com/oauth20_desktop.srf";
            AuthenticationContext context = new AuthenticationContext(authorityUri);
            var token = await context.AcquireTokenAsync(resource, clientId, new Uri(redirectUri), parameters);
            return token;
        }
        public static async Task<string> GetKeyVaultAccessToken(string authority, string resource, string scope)
        {
            var tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
            string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
            string authorityUri = string.Format("https://login.microsoftonline.com/{0}", tenantId);
            string redirectUri = "https://login.live.com/oauth20_desktop.srf";
            var context = new AuthenticationContext(authorityUri);
            var keyVaultUri = ConfigurationManager.AppSettings["KeyVaultUri"];
            var tokenResult = await context.AcquireTokenAsync(resource, clientId, new Uri(redirectUri), new PlatformParameters(PromptBehavior.Auto));
            return tokenResult.AccessToken;
        }
    }
}

