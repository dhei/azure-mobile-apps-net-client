// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace Microsoft.WindowsAzure.MobileServices.Test
{
    /// <summary>
    /// Platform-specific Login Tests for the Windows Store platform.
    /// </summary>
    class LoginTests
    {
        private static MobileServiceClient client;

        /// <summary>
        /// Tests the <see cref="MobileServiceClient.LoginAsync"/> functionality on the platform.
        /// </summary>
        /// <param name="provider">The provider with which to login.
        /// </param>
        /// <param name="useSingleSignOn">Indicates if single sign-on should be used when logging in.
        /// </param>
        /// <param name="useProviderStringOverload">Indicates if the call to <see cref="MobileServiceClient.LoginAsync"/>
        /// should be made with the overload where the provider is passed as a string.
        /// </param>
        /// <returns>The UserId and MobileServiceAuthentication token obtained from logging in.</returns>
        public static async Task<string> TestLoginAsync(MobileServiceAuthenticationProvider provider, bool useSingleSignOn, bool useProviderStringOverload)
        {
            MobileServiceUser user;
            await TestCRUDAsync("Public", false, false);
            await TestCRUDAsync("Authorized", true, false);
            if (useProviderStringOverload)
            {
                user = await client.LoginAsync(provider.ToString(), useSingleSignOn);
            }
            else
            {
                user = await client.LoginAsync(provider, useSingleSignOn);
            }
            await TestCRUDAsync("Public", false, true);
            await TestCRUDAsync("Authorized", true, true);
            await TestLogoutAsync();
            await TestCRUDAsync("Authorized", true, false);
            
            return string.Format("UserId: {0} Token: {1}", user.UserId, user.MobileServiceAuthenticationToken);
        }

        public static async Task<string> TestLogoutAsync()
        {
            await client.LogoutAsync();
            return string.Format("Logged out. Current logged-in client: {0}", client.CurrentUser == null ? "<<NULL>>" : client.CurrentUser.UserId);
        }

        public static async Task<string> TestCRUDAsync(string tableName, bool tableRequiresAuthentication, bool userIsAuthenticated)
        {
            bool crudShouldWork = (tableRequiresAuthentication && userIsAuthenticated) || !tableRequiresAuthentication;
            var user = client.CurrentUser;
            var table = client.GetTable(tableName);
            var item = new JObject();
            item.Add("name", "John Doe");
            string id;
            Dictionary<string, string> queryParameters = new Dictionary<string, string>
            {
                {"userIsAuthenticated", userIsAuthenticated.ToString().ToLowerInvariant() },
            };

            // Attempt to insert 
            try
            {
                JToken inserted = await table.InsertAsync(item, queryParameters);
                item = (JObject)inserted;
                Debug.WriteLine(string.Format("Inserted item: {0}", item));
                id = item["id"].Value<string>();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                // Store the ID for other tests.
                id = "1";
                item["id"] = 1;
                ValidateExpectedFailure("Insert", tableName, crudShouldWork, tableRequiresAuthentication, e);
            }

            // Attempt to update 
            try
            {
                item["name"] = "Jane Roe";
                JToken updated = await table.UpdateAsync(item, queryParameters);
                Debug.WriteLine(string.Format("Updated item: {0}", item));
            }
            catch (MobileServiceInvalidOperationException e)
            {
                ValidateExpectedFailure("Update", tableName, crudShouldWork, tableRequiresAuthentication, e);
            }

            // Attempt to read data
            try
            {
                var item2 = await table.LookupAsync(id, queryParameters);
                Debug.WriteLine(string.Format("Retrieved item via lookup: {0}", item2));
            }
            catch (MobileServiceInvalidOperationException e)
            {
                ValidateExpectedFailure("Lookup", tableName, crudShouldWork, tableRequiresAuthentication, e);
            }

            // Attempt to read data with filter
            try
            {
                JToken items = null;
                items = await table.ReadAsync(string.Format("$filter=id eq '{0}'", id), queryParameters);
                Debug.WriteLine(string.Format("Retrieved items via Read: {0}", items));
                if (((JArray)items).Count != 1)
                {
                    string errorMessage = "Error, query should have returned exactly one item";
                    throw new Exception(errorMessage);
                }
            }
            catch (MobileServiceInvalidOperationException e)
            {
                ValidateExpectedFailure("Read", tableName, crudShouldWork, tableRequiresAuthentication, e);
            }

            // Attempt to delete the data
            try
            {
                JToken deletedItem = await table.DeleteAsync(item, queryParameters);
                Debug.WriteLine(string.Format("Deleted item: {0}", item));
            }
            catch (MobileServiceInvalidOperationException e)
            {
                ValidateExpectedFailure("Delete", tableName, crudShouldWork, tableRequiresAuthentication, e);
            }

            return "Test complete";
        }

        private static void ValidateExpectedFailure(string attemptedAction, string tableName, bool crudShouldWork, bool tableRequiresAuth, MobileServiceInvalidOperationException innerException)
        {
            if (crudShouldWork)
            {
                string errorMessage;
                if (!tableRequiresAuth)
                {
                    errorMessage = string.Format("Anonymous {0} to unprotected table {1} should have succeeded. Instead, found Status Code: {2}. Reason: {3}.", attemptedAction, tableName, innerException.Response.StatusCode, innerException.Response.ReasonPhrase);
                }
                else
                {
                    errorMessage = string.Format("Authenticated {0} to protected table {1} should have succeeded. Instead, found Status Code: {2}. Reason: {3}.", attemptedAction, tableName, innerException.Response.StatusCode, innerException.Response.ReasonPhrase);
                }
                throw new Exception(errorMessage, innerException);
            }
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

                App.LoginMobileService = client;
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
