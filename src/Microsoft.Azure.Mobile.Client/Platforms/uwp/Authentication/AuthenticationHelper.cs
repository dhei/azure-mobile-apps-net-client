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
                var fragments = decodeResponse(uri.Fragment);
                Completed?.Invoke(this, new AuthenticatorCompletedEventArgs(fragments["authorization_code"]));
            }
        }

        private Dictionary<string, string> decodeResponse(string encodedString)
        {
            var inputs = new Dictionary<string, string>();

            if (encodedString.Length > 0)
            {
                char firstChar = encodedString[0];
                if (firstChar == '?' || firstChar == '#')
                {
                    encodedString = encodedString.Substring(1);
                }

                if (encodedString.Length > 0)
                {
                    var parts = encodedString.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var part in parts)
                    {
                        var equalsIndex = part.IndexOf('=');

                        string key;
                        string value;
                        if (equalsIndex >= 0)
                        {
                            key = Uri.UnescapeDataString(part.Substring(0, equalsIndex));
                            value = Uri.UnescapeDataString(part.Substring(equalsIndex + 1));
                        }
                        else
                        {
                            key = Uri.UnescapeDataString(part);
                            value = string.Empty;
                        }

                        inputs[key] = value;
                    }
                }
            }
            return inputs;
        }
    }
}
