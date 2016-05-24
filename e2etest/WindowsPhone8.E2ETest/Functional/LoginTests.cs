// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.TestFramework;
using Newtonsoft.Json.Linq;

namespace Microsoft.WindowsAzure.MobileServices.Test
{
    /// <summary>
    /// Platform-specific Login Tests for the Windows Phone Silverlight 8.1 platform.
    /// </summary>
    public class LoginTests
    {
        private const string RefreshUser400ErrorMessage = "Refresh failed with a 400 Bad Request error. The identity provider does not support refresh, or the user is not logged in with sufficient permission.";

        private const string GoogleTokenRevocationURI = @"https://accounts.google.com/o/oauth2/revoke?token={0}";

        private static MobileServiceClient client;

        /// <summary>
        /// Tests the <see cref="MobileServiceClient.RefreshUserAsync"/> functionality on the platform.
        /// </summary>
        /// <param name="provider">
        /// The provider with which to login.
        /// </param>
        /// <returns>
        /// The UserId and MobileServiceAuthentication token obtained from logging in.
        /// </returns>
        public static async Task<string> TestRefreshUserAsync(MobileServiceAuthenticationProvider provider)
        {
            string result;
            switch (provider)
            {
                case MobileServiceAuthenticationProvider.MicrosoftAccount:
                    result = await TestMicrosoftAccountRefreshUserAsync();
                    break;
                case MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory:
                    result = await TestAADRefreshUserAsync();
                    break;
                case MobileServiceAuthenticationProvider.Google:
                    result = await TestGoogleRefreshUserAsync();
                    break;
                case MobileServiceAuthenticationProvider.Facebook:
                    result = await TestFacebookRefreshUserAsync();
                    break;
                case MobileServiceAuthenticationProvider.Twitter:
                    result = await TestTwitterRefreshUserAsync();
                    break;
                default:
                    result = "MobileServiceAuthenticationProvider is not recognized.";
                    break;
            }
            return result;
        }

        /// <summary>
        /// Tests the <see cref="MobileServiceClient.RefreshUserAsync"/> functionality of Facebook
        /// </summary>
        /// <returns>
        /// The UserId and MobileServiceAuthentication token obtained after refresh
        /// </returns>
        private static async Task<string> TestFacebookRefreshUserAsync()
        {
            MobileServiceUser user = await client.LoginAsync(MobileServiceAuthenticationProvider.Facebook);

            try
            {
                await client.RefreshUserAsync();
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.AreEqual(HttpStatusCode.BadRequest, ex.Response.StatusCode);
                Assert.AreEqual(RefreshUser400ErrorMessage, ex.Message);
                return "User Login succeeded. User Refresh is not supported by Facebook as expected.";
            }

            Assert.Fail("RefreshAsync() should throw 400 error on Facebook account.");
            return string.Empty;
        }

        /// <summary>
        /// Tests the <see cref="MobileServiceClient.RefreshUserAsync"/> functionality of Twitter
        /// </summary>
        /// <returns>
        /// The UserId and MobileServiceAuthentication token obtained after refresh
        /// </returns>
        private static async Task<string> TestTwitterRefreshUserAsync()
        {
            MobileServiceUser user = await client.LoginAsync(MobileServiceAuthenticationProvider.Twitter);

            try
            {
                await client.RefreshUserAsync();
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.AreEqual(HttpStatusCode.BadRequest, ex.Response.StatusCode);
                Assert.AreEqual(RefreshUser400ErrorMessage, ex.Message);
                return "User Login succeeded. User Refresh is not supported by Twitter as expected.";
            }

            Assert.Fail("RefreshAsync() should throw 400 error on Twitter account.");
            return string.Empty;
        }

        /// <summary>
        /// Tests the <see cref="MobileServiceClient.RefreshUserAsync"/> functionality of MicrosoftAccount
        /// </summary>
        /// <returns>
        /// The UserId and MobileServiceAuthentication token obtained after refresh
        /// </returns>
        private static async Task<string> TestMicrosoftAccountRefreshUserAsync()
        {
            MobileServiceUser user = await client.LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);
            string authToken = user.MobileServiceAuthenticationToken;

            MobileServiceUser refreshedUser = await client.RefreshUserAsync();
            string refreshedAuthToken = refreshedUser.MobileServiceAuthenticationToken;

            Assert.AreEqual(user.UserId, refreshedUser.UserId);
            Assert.AreNotEqual(authToken, refreshedAuthToken);

            return string.Format("User Login and Refresh succeeded. UserId: {0} Token: {1}", refreshedUser.UserId, refreshedUser.MobileServiceAuthenticationToken);
        }

        /// <summary>
        /// Tests the <see cref="MobileServiceClient.RefreshUserAsync"/> functionality of WindowsAzureActiveDirectory
        /// </summary>
        /// <returns>
        /// The UserId and MobileServiceAuthentication token obtained after refresh
        /// </returns>
        private static async Task<string> TestAADRefreshUserAsync()
        {
            MobileServiceUser user = await client.LoginAsync(MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory, new Dictionary<string, string>()
            {
                { "response_type", "code id_token" }
            });
            string authToken = user.MobileServiceAuthenticationToken;

            MobileServiceUser refreshedUser = await client.RefreshUserAsync();
            string refreshedAuthToken = refreshedUser.MobileServiceAuthenticationToken;

            Assert.AreEqual(user.UserId, refreshedUser.UserId);
            Assert.AreNotEqual(authToken, refreshedAuthToken);

            return string.Format("User Login and Refresh succeeded. UserId: {0} Token: {1}", refreshedUser.UserId, refreshedUser.MobileServiceAuthenticationToken);
        }

        /// <summary>
        /// Tests the <see cref="MobileServiceClient.RefreshUserAsync"/> functionality of Google
        /// </summary>
        /// <returns>
        /// The UserId and MobileServiceAuthentication token obtained after refresh
        /// </returns>
        private static async Task<string> TestGoogleRefreshUserAsync()
        {
            // Firstly, login user with Google account with offline permission
            MobileServiceUser user = await client.LoginAsync(MobileServiceAuthenticationProvider.Google, new Dictionary<string, string>()
            {
                { "access_type", "offline" },
                { "prompt", "consent" } // Force prompt window of Google offline scope in login
            });

            // Secondly, refresh user using refresh token
            MobileServiceUser refreshedUser = await client.RefreshUserAsync();
            Assert.AreEqual(user.UserId, refreshedUser.UserId);
            Assert.AreNotEqual(user.MobileServiceAuthenticationToken, refreshedUser.MobileServiceAuthenticationToken);

            // Then, get refresh token from /.auth/me endpoint of the backend
            string googleAccessToken = await GetGoogleAccessToken(client.MobileAppUri.AbsoluteUri, refreshedUser.MobileServiceAuthenticationToken);

            // Next, revoke Google access token, which would subsequently revoke Google refresh token
            bool accessTokenRevoked = await RevokeGoogleAccessToken(googleAccessToken);

            // Next, refresh user with the refresh token which just got revoked.
            // Because of the revocation, refresh will throw 403 Forbidden exception
            MobileServiceInvalidOperationException exception = null;
            try
            {
                await client.RefreshUserAsync();
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                exception = ex;
            }
            if (exception == null)
            {
                throw new Exception("RefreshUserAsync API should throw 403 exception because of access token revocation.");
            }
            else if (exception.Response.StatusCode != System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("RefreshUserAsync API throws " + exception.Response.StatusCode + " exception instead of 403 exception.");
            }

            // Last, login user with Google offline scope again
            MobileServiceUser user2 = await client.LoginAsync(MobileServiceAuthenticationProvider.Google, new Dictionary<string, string>()
            {
                { "access_type", "offline" },
                { "prompt", "consent" } // Force prompt window of Google offline scope in login
            });

            // Refresh user should work this time
            MobileServiceUser refreshedUser2 = await client.RefreshUserAsync();
            Assert.AreEqual(refreshedUser2.UserId, user2.UserId);
            Assert.AreNotEqual(refreshedUser2.MobileServiceAuthenticationToken, user2.MobileServiceAuthenticationToken);

            return string.Format("User Login and Refresh succeeded. UserId: {0} Token: {1}", refreshedUser2.UserId, refreshedUser2.MobileServiceAuthenticationToken);
        }

        private static async Task<string> GetGoogleAccessToken(string mobileServiceUri, string mobileServiceAuthenticationToken)
        {
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(mobileServiceUri + ".auth/me")
            };
            request.Headers.Add("zumo-api-version", "2.0.0");
            request.Headers.Add("x-zumo-auth", mobileServiceAuthenticationToken);

            HttpResponseMessage response = await httpClient.SendAsync(request);

            string responseString = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(responseString))
            {
                JToken jtoken = JToken.Parse(responseString);
                return (string)jtoken[0]["access_token"];
            }
            else
            {
                throw new Exception("Failed to fetch refresh token");
            }
        }

        private static async Task<bool> RevokeGoogleAccessToken(string googleAccessToken)
        {
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(string.Format(GoogleTokenRevocationURI, googleAccessToken))
            };
            HttpResponseMessage response = await httpClient.SendAsync(request);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        /// <summary>
        /// Utility method that can be used to execute a test.  It will capture any exceptions throw
        /// during the execution of the test and return a message with details of the exception thrown.
        /// </summary>
        /// <param name="testName">The name of the test being executed.
        /// </param>
        /// <param name="test">A test to execute.
        /// </param>
        /// <returns>
        /// Either the result of the test if the test passed, or a message with the exception
        /// that was thrown.
        /// </returns>
        public static async Task<string> ExecuteTest(string testName, Func<Task<string>> test)
        {
            string resultText = null;
            bool didPass = false;

            if (client == null)
            {
                string appUrl = null;
                App.Harness.Settings.Custom.TryGetValue("MobileServiceRuntimeUrl", out appUrl);

                client = new MobileServiceClient(appUrl);
            }

            try
            {
                resultText = await test();
                didPass = true;
            }
            catch (Exception exception)
            {
                resultText = string.Format("ExceptionType: {0} Message: {1} StackTrace: {2}",
                                               exception.GetType().ToString(),
                                               exception.Message,
                                               exception.StackTrace);
            }

            return string.Format("Test '{0}' {1}.\n{2}",
                                 testName,
                                 didPass ? "PASSED" : "FAILED",
                                 resultText);
        }
    }
}
