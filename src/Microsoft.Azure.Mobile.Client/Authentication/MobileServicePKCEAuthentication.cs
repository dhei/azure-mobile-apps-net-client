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
        
        internal Uri CallbackUri { get; private set; }
        
        public MobileServicePKCEAuthentication(MobileServiceClient client, string provider, string uriScheme, IDictionary<string, string> parameters)
            : base(client, provider, parameters)
        {
            Debug.Assert(client != null, "client should not be null.");
            Debug.Assert(uriScheme != null, "uriScheme should not be null.");

            this.client = client;

            this.CallbackUri = new Uri(MobileServiceUrlBuilder.CombileSchemeAndPath(uriScheme, "easyauth.callback"));
        }
        
        protected async sealed override Task<string> LoginAsyncOverride()
        {
            string codeVerifier = GetCodeVerifier();
            string authorizationCode = await this.LoginAsyncOverride(GetHash(codeVerifier), "S256");
            string path = MobileServiceUrlBuilder.CombinePaths(LoginAsyncUriFragment, ProviderName);
            if (!string.IsNullOrEmpty(client.LoginUriPrefix))
            {
                path = MobileServiceUrlBuilder.CombinePaths(client.LoginUriPrefix, ProviderName);
            }
            path = MobileServiceUrlBuilder.CombinePaths(path, "token");
            string queryString = MobileServiceUrlBuilder.GetQueryString(new Dictionary<string, string>
            {
                { "authorization_code", authorizationCode },
                { "code_verifier", codeVerifier }
            });
            string pathAndQuery = MobileServiceUrlBuilder.CombinePathAndQuery(path, queryString);
            var httpClient = client.AlternateLoginHost == null ? client.HttpClient : client.AlternateAuthHttpClient;
            return await client.HttpClient.RequestWithoutHandlersAsync(HttpMethod.Get, pathAndQuery, null);
        }

        protected abstract Task<string> LoginAsyncOverride(string codeChallenge, string codeChallengeMethod);

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
