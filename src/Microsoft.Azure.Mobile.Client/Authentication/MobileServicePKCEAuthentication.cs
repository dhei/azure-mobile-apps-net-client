using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using PCLCrypto;

namespace Microsoft.WindowsAzure.MobileServices
{
    internal abstract class MobileServicePKCEAuthentication : MobileServiceAuthentication
    {
        /// <summary>
        /// The <see cref="MobileServiceClient"/> used by this authentication session.
        /// </summary>
        private readonly MobileServiceClient client;

        protected Uri LoginUri { get; private set; }

        protected Uri CallbackUri { get; private set; }
        
        protected string CodeVerifier { get; private set; }

        public MobileServicePKCEAuthentication(MobileServiceClient client, string provider, string uriScheme, IDictionary<string, string> parameters)
            : base(client, provider, parameters)
        {
            Debug.Assert(client != null, "client should not be null.");
            Debug.Assert(uriScheme != null, "uriScheme should not be null.");

            this.client = client;

            this.CodeVerifier = GetCodeVerifier();

            var path = MobileServiceUrlBuilder.CombinePaths(LoginAsyncUriFragment, this.ProviderName);
            if (!string.IsNullOrEmpty(this.Client.LoginUriPrefix))
            {
                path = MobileServiceUrlBuilder.CombinePaths(this.Client.LoginUriPrefix, this.ProviderName);
            }

            var loginParameters = parameters != null ? new Dictionary<string, string>(parameters) : new Dictionary<string, string>();
            loginParameters.Add("code_challenge", GetHash(this.CodeVerifier));
            loginParameters.Add("code_challenge_method", "S256");
            var loginQueryString = MobileServiceUrlBuilder.GetQueryString(loginParameters, useTableAPIRules: false);
            var loginPathAndQuery = MobileServiceUrlBuilder.CombinePathAndQuery(path, loginQueryString);
            
            this.LoginUri = new Uri(this.Client.MobileAppUri, loginPathAndQuery);
            if (this.Client.AlternateLoginHost != null)
            {
                this.LoginUri = new Uri(this.Client.AlternateLoginHost, loginPathAndQuery);
            }
            this.CallbackUri = new Uri(MobileServiceUrlBuilder.CombileSchemeAndPath(uriScheme, "easyauth.callback"));
        }
        
        protected async sealed override Task<string> LoginAsyncOverride()
        {
            string codeVerifier = GetCodeVerifier();
            string authorizationCode = await this.GetAuthorizationCodeAsync();
            string path = MobileServiceUrlBuilder.CombinePaths(LoginAsyncUriFragment, ProviderName);
            if (!string.IsNullOrEmpty(client.LoginUriPrefix))
            {
                path = MobileServiceUrlBuilder.CombinePaths(client.LoginUriPrefix, ProviderName);
            }
            path = MobileServiceUrlBuilder.CombinePaths(path, "token");
            var tokenParameters = Parameters != null ? new Dictionary<string, string>(Parameters) : new Dictionary<string, string>();
            tokenParameters.Add("authorization_code", authorizationCode);
            tokenParameters.Add("code_verifier", codeVerifier);
            string queryString = MobileServiceUrlBuilder.GetQueryString(tokenParameters);
            string pathAndQuery = MobileServiceUrlBuilder.CombinePathAndQuery(path, queryString);
            var httpClient = client.AlternateLoginHost == null ? client.HttpClient : client.AlternateAuthHttpClient;
            return await client.HttpClient.RequestWithoutHandlersAsync(HttpMethod.Get, pathAndQuery, null);
        }

        protected abstract Task<string> GetAuthorizationCodeAsync();

        private static string GetCodeVerifier()
        {
            var randomBytes = WinRTCrypto.CryptographicBuffer.GenerateRandom(32);
            return Convert.ToBase64String(randomBytes);
        }

        private static string GetHash(string data)
        {
            var sha = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
            return Encoding.UTF8.GetString(sha.HashData(Encoding.UTF8.GetBytes(data)));
        }
    }
}
