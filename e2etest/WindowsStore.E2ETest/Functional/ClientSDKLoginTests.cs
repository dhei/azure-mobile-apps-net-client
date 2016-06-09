// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.OneDrive.Sdk;
using Microsoft.WindowsAzure.MobileServices.TestFramework;
using Newtonsoft.Json.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Microsoft.WindowsAzure.MobileServices.Test
{
    [Tag("ClientSDKLoginTests")]
    public class ClientSDKLoginTests : FunctionalTestBase
    {
        [AsyncTestMethod]
        /// <summary>
        /// Tests logging into MobileService with Microsoft Account token (via OneDrive SDK). 
        /// App needs to be assosciated with a WindowsStoreApp
        /// </summary>
        private async Task TestClientDirectedMicrosoftAccountLogin()
        {
            try
            {
                Task<AccountSession> sessionTask = null;

                // Force AuthenticateAsync() to run on the UI thread.
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    IOneDriveClient oneDriveClient = OneDriveClientExtensions.GetUniversalClient(new string[] { "wl.signin" });
                    sessionTask = oneDriveClient.AuthenticateAsync();
                });
                AccountSession session = await sessionTask;

                if (session != null && session.AccessToken != null)
                {
                    JObject accessToken = new JObject();
                    accessToken["access_token"] = session.AccessToken;

                    MobileServiceUser user = await this.GetClient().LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount, accessToken);

                    Log(string.Format("Log in with Microsoft Account succeeded - userId {0}", user.UserId));
                }
                else
                {
                    Assert.Fail("Log in with Live SDK failed");
                }
            }
            catch (Exception exception)
            {
                Log(string.Format("ExceptionType: {0} Message: {1} StackTrace: {2}",
                                                exception.GetType().ToString(),
                                                exception.Message,
                                                exception.StackTrace));
                Assert.Fail("Log in with Live SDK failed");
            }
        }

        [AsyncTestMethod]
        /// <summary>
        /// Tests login endpoint when alternate login host is set.
        /// </summary>
        private async Task AlternateHostLoginTest()
        {
            MobileServiceClient mobileServiceClient = GetClient();
            string expectedRequestUri = "";
            string alternateLoginHost = "https://login.live.com";
            string defaultLoginPrefix = "/.auth/login";

            try
            {
                mobileServiceClient.AlternateLoginHost = new Uri(alternateLoginHost);
                expectedRequestUri = alternateLoginHost + defaultLoginPrefix + "/microsoftaccount";
                var result = await mobileServiceClient.LoginWithMicrosoftAccountAsync(null);
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                if (!VerifyRequestUri(ex, expectedRequestUri))
                {
                    throw;
                }
            }
            finally
            {
                mobileServiceClient.AlternateLoginHost = null;
            }
        }

        [AsyncTestMethod]
        /// <summary>
        /// Tests login endpoint when loginPrefix is set.
        /// </summary>
        private async Task LoginPrefixTest()
        {
            MobileServiceClient mobileServiceClient = GetClient();
            string expectedRequestUri = "";
            string loginPrefix = "foo/bar";
            mobileServiceClient.LoginUriPrefix = loginPrefix;

            try
            {
                expectedRequestUri = mobileServiceClient.MobileAppUri.ToString() + loginPrefix + "/microsoftaccount";
                var result = await mobileServiceClient.LoginWithMicrosoftAccountAsync(null);
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                if (!VerifyRequestUri(ex, expectedRequestUri))
                {
                    throw;
                }
            }
            finally
            {
                mobileServiceClient.LoginUriPrefix = null;
            }
        }

        private bool VerifyRequestUri(MobileServiceInvalidOperationException ex, string expectedRequestUri)
        {
            string requestUri = ex.Response.RequestMessage.RequestUri.ToString();
            if (ex.Response.StatusCode == HttpStatusCode.NotFound && requestUri == expectedRequestUri)
            {
                Log("Login request routed expected endpoint");
                return true;
            }
            Log("Expected request Uri: {0} Acutual request Uri: {1}",
                expectedRequestUri,
                requestUri);

            return false;
        }
    }
}