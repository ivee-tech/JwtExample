# JwtExample
Demo for JWT generation, parsing and validation using Microsoft.IdentityModel.JsonWebTokens package and using keys from Azure Key Vault.

The application must be registered in Azure AD to get access to Key Vault.
A script is provided to register the application and assign permissions in Key Vault

```
./scripts/create-AAD-app.ps1
```
You will need to provide the application name, and optionally App Uri, Home page Url, and reply Urls.
Additionally, yopu will need to specify the resource group name and the key vault name.
To execute the script, you will need to connect to Azure using

``` PowerShell
Connect-AzureRmAccount
```

and to Azure AD using

``` PowerShell
Connect-AzureAD
```

For Azure AD operations, make sure the Azure AD module is installed. If not, install it using

``` PowerShell
Install-Module AzureAD
```

Another script is used to set the JwtExample-SigningKey secret in Key Vault

```
./scripts/set=keyvault-secret.ps1
```
You will need to provide the resource group name and key vault name.
To execute the script, connect to Azure first

``` PowerShell
Connect-AzureRmAccount
```

The solution contains the following projects:

 - **Microsoft.CommonLib** - a simple library containing <code>IConfigReader</code> interface for unifying the configuration.

 -  **Microsoft.ConfigReaders** - contains implementations for <code>IConfigReader</code> interface:
   1. <code>AppSettingsConfigReader</code> - uses <code>ConfigurationManager.AppSettings</code> to retrieve the configuration settings;
   2. <code>CredentialsKeyVaultConfigReader</code> - uses credentials (application ID and secret) to get access to Azure Key Vault;
   3. Uses <code>AzureServiceTokenProvider</code> to get access to Azure Key Vault;
 
 - **Microsoft.Tokens** - contains the <code>JwtGen</code> class responsible for generating, parsing, and validating JWTs;

 - **JwtExample** - console app to test the JWTs generation;

The <code>JwtGen</code> class has the following methods:

 - <code>GenerateToken</code> - generates a token based on signing key, name, audience and additional claims
``` C#
var nameId = "myemail@provider.com";
var issuer = "http://my.tokenissuer.com";
var audience = "http://my.website.com";
var role = "User";
var subscription = Guid.NewGuid().ToString();

var appConfig = new AppSettingsConfigReader();
var kvUri = appConfig["KeyVaultUri"];

IConfigReader kvConfig = new CredentialsKeyVaultConfigReader(appConfig);
var signingKey = kvConfig["JwtExample-SigningKey"];
Console.WriteLine("Signing key: {0}", signingKey);

var tokenData = JwtGen.GenerateToken(signingKey, nameId, audience, issuer, role, new Dictionary<string, string> { { nameof(subscription), subscription } });
Console.WriteLine("Token data: {0}", tokenData);
```

 - <code>ParseToken</code> - returns a <code>JwtSecurityToken</code> object based on token data string
``` C#
var token = JwtGen.ParseToken(tokenData);
Console.WriteLine("Audience: {0}", token.Audiences.First());
Console.WriteLine("Subscription: {0}", token.Claims.SingleOrDefault(c => c.Type == nameof(subscription)).Value);
```

 - <code>ValidateToken</code> - validates a token and returns a <code>ClaimsPrincipal</code> based on token data string and signing key
``` C#
var principal = JwtGen.ValidateToken(tokenData, signingKey, new string[] { audience }, new string[] { issuer });
Console.WriteLine("Principal: {0}", principal.Identity.Name);
Console.WriteLine("Subscription: {0}", principal.Claims.SingleOrDefault(c => c.Type == nameof(subscription)).Value);
```

