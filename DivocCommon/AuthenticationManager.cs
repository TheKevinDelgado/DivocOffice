using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Net;
using System.Net.Http;
using Microsoft.Identity.Client;
using System.Security.Cryptography;
using System.IO;
using System.Windows.Forms;

namespace DivocCommon
{
    /// <summary>
    /// First pass MSAL user authentication. Currently, each Office Add-in creates its
    /// own AuthenticationManager, which means each is authenticating separately, even though
    /// they are all using the same client id/tenant, etc. Future revision could have a 
    /// singleton out of proc server proxying authentication which would be called by all
    /// of the add-ins. This stuff appears to want to run in the main app UI thread, so 
    /// there may be limitations to this approad if using the .Net APIs. May be better to
    /// use javascript.
    /// </summary>
    public class AuthenticationManager
    {
        private static class ConfigurationInfo
        {
            public static string ClientId = Environment.GetEnvironmentVariable("DIVOC_CLIENTID", EnvironmentVariableTarget.User);
            public static string Tenant = Environment.GetEnvironmentVariable("DIVOC_TENANT", EnvironmentVariableTarget.User);
            public static string Instance = Environment.GetEnvironmentVariable("DIVOC_INSTANCE", EnvironmentVariableTarget.User);
        }

        static class TokenCacheHelper
        {
            /// <summary>
            /// Path to the token cache
            /// </summary>
            public static readonly string CacheFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location + ".msalcache.bin3";

            private static readonly object FileLock = new object();

            public static void BeforeAccessNotification(TokenCacheNotificationArgs args)
            {
                lock (FileLock)
                {
                    args.TokenCache.DeserializeMsalV3(File.Exists(CacheFilePath)
                            ? ProtectedData.Unprotect(File.ReadAllBytes(CacheFilePath),
                                                     null,
                                                     DataProtectionScope.CurrentUser)
                            : null);
                }
            }

            public static void AfterAccessNotification(TokenCacheNotificationArgs args)
            {
                // if the access operation resulted in a cache update
                if (args.HasStateChanged)
                {
                    lock (FileLock)
                    {
                        // reflect changesgs in the persistent store
                        File.WriteAllBytes(CacheFilePath,
                                           ProtectedData.Protect(args.TokenCache.SerializeMsalV3(),
                                                                 null,
                                                                 DataProtectionScope.CurrentUser)
                                          );
                    }
                }
            }

            internal static void EnableSerialization(ITokenCache tokenCache)
            {
                tokenCache.SetBeforeAccess(BeforeAccessNotification);
                tokenCache.SetAfterAccess(AfterAccessNotification);
            }
        }

        string graphAPIEndpoint = "https://graph.microsoft.com/v1.0/me";
        string[] scopes = new string[] { "user.read" };

        private static IPublicClientApplication _clientApp;
        public static IPublicClientApplication PublicClientApp { get { return _clientApp; } }

        public AuthenticationManager()
        {
            CreateApplication(false);
        }

        public async Task<bool> Authenticate(IntPtr wnd)
        {
            bool success = false;

            AuthenticationResult authResult = null;
            IAccount firstAccount;
            var accounts = await PublicClientApp.GetAccountsAsync();
            firstAccount = accounts.FirstOrDefault();
            try
            {
                authResult = await PublicClientApp.AcquireTokenSilent(scopes, firstAccount)
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilent. 
                // This indicates you need to call AcquireTokenInteractive to acquire a token
                System.Diagnostics.Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                try
                {
                    authResult = await PublicClientApp.AcquireTokenInteractive(scopes)
                        .WithAccount(firstAccount)
                        //.WithParentActivityOrWindow(wnd) // optional, used to center the browser on the window
                        .WithPrompt(Prompt.SelectAccount)
                        .ExecuteAsync();
                }
                catch (MsalException msalex)
                {
                    string err = $"Error Acquiring Token:{System.Environment.NewLine}{msalex}";
                }
            }

            if (authResult != null)
            {
                success = true;
                string content = await GetHttpContentWithToken(graphAPIEndpoint, authResult.AccessToken);
                LogManager.LogInformation(content);
            }

            return success;
        }

        public async void SignOut()
        {
            var accounts = await PublicClientApp.GetAccountsAsync();
            if (accounts.Any())
            {
                try
                {
                    await PublicClientApp.RemoveAsync(accounts.FirstOrDefault());
                }
                catch (MsalException ex)
                {
                    string err = $"Error signing-out user: {ex.Message}";
                }
            }
        }

        public static void CreateApplication(bool useWam)
        {
            var builder = PublicClientApplicationBuilder.Create(ConfigurationInfo.ClientId)
                .WithAuthority($"{ConfigurationInfo.Instance}{ConfigurationInfo.Tenant}")
                .WithDefaultRedirectUri();

            if (useWam)
            {
                builder.WithExperimentalFeatures();
                builder.WithBroker(true);  // Requires redirect URI "ms-appx-web://microsoft.aad.brokerplugin/{client_id}" in app registration
            }
            _clientApp = builder.Build();
            TokenCacheHelper.EnableSerialization(_clientApp.UserTokenCache);
        }

        public async Task<string> GetHttpContentWithToken(string url, string token)
        {
            var httpClient = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage response;
            try
            {
                var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, url);
                //Add the token in Authorization header
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
