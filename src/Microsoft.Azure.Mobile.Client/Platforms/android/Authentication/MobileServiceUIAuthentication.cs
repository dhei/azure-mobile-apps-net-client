using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Android.Content;
using Xamarin.Auth._MobileServices;

namespace Microsoft.WindowsAzure.MobileServices
{
    internal class MobileServiceUIAuthentication : MobileServicePKCEAuthentication
    {
        public MobileServiceUIAuthentication (Context context, MobileServiceClient client, string providerName, string uriScheme, IDictionary<string, string> parameters)
            : base (client, providerName, uriScheme, parameters)
        {
            this.context = context;
        }

        private Context context;

        protected override Task<string> LoginAsyncOverride(string codeChallenge, string codeChallengeMethod)
        {
            var tcs = new TaskCompletionSource<string>();

            var authenticator = new WebRedirectAuthenticator (StartUri, CallbackUri)
            {
                IsUsingNativeUI = true,
                //ShowUIErrors = false,
                ClearCookiesBeforeLogin = false
            };

            Intent intent = authenticator.GetUI (this.context);

            authenticator.Error += (sender, e) =>
            {
                string message = String.Format (CultureInfo.InvariantCulture, "Authentication failed with HTTP response code {0}.", e.Message);
                InvalidOperationException ex = (e.Exception == null)
                    ? new InvalidOperationException (message)
                    : new InvalidOperationException (message, e.Exception);

                tcs.TrySetException (ex);
                authenticator = null;
            };

            authenticator.Completed += (sender, e) =>
            {
                if (!e.IsAuthenticated)
                    tcs.TrySetException (new InvalidOperationException ("Authentication was cancelled by the user."));
                else
                    tcs.TrySetResult(e.Account.Properties["token"]);
                authenticator = null;
            };

            context.StartActivity (intent);
            
            return tcs.Task;
        }
    }
}