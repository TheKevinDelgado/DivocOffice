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
    /// 
    /// UPDATE: all that seems to matter is the msalcache.bin3 file, so moved it to appdata
    /// and everything is awesome/cool when you're part of a team.
    /// </summary>
    public class AuthenticationManager
    {
        /// <summary>
        /// Application configuration information. Should be moved out of the scope of authentication
        /// to more generically available/useful level, but for now only authentication is leveraging
        /// this stuff so it is fine here.
        /// </summary>
        private static class ConfigurationInfo
        {
            public static string ClientId = Environment.GetEnvironmentVariable("DIVOC_CLIENTID", EnvironmentVariableTarget.User);
            public static string Tenant = Environment.GetEnvironmentVariable("DIVOC_TENANT", EnvironmentVariableTarget.User);
            public static string Instance = Environment.GetEnvironmentVariable("DIVOC_INSTANCE", EnvironmentVariableTarget.User);

            public static string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Divoc");
        }

        static class TokenCacheHelper
        {
            /// <summary>
            /// Path to the token cache
            /// </summary>
            public static readonly string CacheFilePath = Path.Combine(ConfigurationInfo.AppDataPath, "DivocCommon.dll.msalcache.bin3");

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

        string[] scopes = new string[] { "user.read", "files.readwrite.all" };

        private static IPublicClientApplication _clientApp;
        public static IPublicClientApplication PublicClientApp { get { return _clientApp; } }

        public static string AccessToken { get; private set; }

        public AuthenticationManager()
        {
            // This app path stuff could/should go elsewhere, but since authentication is the only
            // thing using it for now, and all add-ins use authentication, just leave it here.
            // If the add-ins require their own subdirectories or just need to store other information,
            // break app dir stuff out into something more generically usable across the solution.
            Directory.CreateDirectory(ConfigurationInfo.AppDataPath);

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
                AccessToken = authResult.AccessToken;
                success = true;
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
