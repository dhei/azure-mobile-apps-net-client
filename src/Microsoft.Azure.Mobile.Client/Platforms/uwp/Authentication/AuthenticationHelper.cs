using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.WindowsAzure.MobileServices.Platforms.uwp.Authentication
{
    class AuthenticationHelper
    {
        /// <summary>
        /// Occurs when authentication has been successfully or unsuccessfully completed.
        /// </summary>
        public event EventHandler<AuthenticatorCompletedEventArgs> Completed;

        /// <summary>
        /// Occurs when there an error is encountered when authenticating.
        /// </summary>
        public event EventHandler<AuthenticatorErrorEventArgs> Error;

        internal void OnResponseReceived(Uri uri)
        {
            if (!uri.Fragment.Contains("authorization_code"))
            {
                Error?.Invoke(this, new AuthenticatorErrorEventArgs("Authorization code not found in server response"));
            }
            else
            {
                string authorizationCode = uri.Fragment.Substring(uri.Fragment.IndexOf('=') + 1);
                Completed?.Invoke(this, new AuthenticatorCompletedEventArgs(authorizationCode));
            }
        }
    }
}
