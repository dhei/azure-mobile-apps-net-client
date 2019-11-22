// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System.Linq;

using Microsoft.WindowsAzure.MobileServices.TestFramework;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using Foundation;
using System.Globalization;
using Newtonsoft.Json;

namespace Microsoft.WindowsAzure.MobileServices.Test
{
    [Tag("push")]
    public class PushFunctional : FunctionalTestBase
    {
        readonly IPushTestUtility pushTestUtility;

        public PushFunctional()
        {
            this.pushTestUtility = TestPlatform.Instance.PushTestUtility;
        }

        [AsyncTestMethod]
        public async Task InitialDeleteRegistrationsAsync()
        {
            NSData channelUri = NSDataFromDescription(this.pushTestUtility.GetPushHandle());
            Dictionary<string, string> channelUriParam = new Dictionary<string, string>()
            {
                {"channelUri", TrimDeviceToken(channelUri)}
            };
            await this.GetClient().InvokeApiAsync("deleteRegistrationsForChannel", HttpMethod.Delete, channelUriParam);
        }

        [AsyncTestMethod]
        public async Task RegisterAsync()
        {
            NSData channelUri = NSDataFromDescription(this.pushTestUtility.GetPushHandle());
            var push = this.GetClient().GetPush();
            await push.RegisterAsync(channelUri);
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                {"channelUri", TrimDeviceToken(channelUri)}
            };
            await VerifyRegistration(parameters, push);
        }

        [AsyncTestMethod]
        public async Task LoginRegisterAsync()
        {
            MobileServiceUser user = await Utilities.GetDummyUser(this.GetClient());
            this.GetClient().CurrentUser = user;
            NSData channelUri = NSDataFromDescription(this.pushTestUtility.GetPushHandle());
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                {"channelUri", TrimDeviceToken(channelUri)}
            };
            var push = this.GetClient().GetPush();
            await push.RegisterAsync(channelUri);
            await VerifyRegistration(parameters, push);
        }

        [AsyncTestMethod]
        public async Task UnregisterAsync()
        {
            var channelUri = this.pushTestUtility.GetPushHandle();
            var push = this.GetClient().GetPush();
            await push.UnregisterAsync();
            await this.GetClient().InvokeApiAsync("verifyUnregisterInstallationResult", HttpMethod.Get, null);
        }

        [AsyncTestMethod]
        public async Task RegisterAsyncTemplatesAndOverride()
        {
            NSData channelUri = NSDataFromDescription(this.pushTestUtility.GetPushHandle());
            JObject templates = GetTemplates("foo");
            JObject expectedTemplates = GetTemplates("testApnsTemplate");
            var push = this.GetClient().GetPush();
            try
            {
                await push.RegisterAsync(channelUri);
                Dictionary<string, string> parameters = new Dictionary<string, string>()
                {
                    {"channelUri", TrimDeviceToken(channelUri)},
                };
                await this.GetClient().InvokeApiAsync("verifyRegisterInstallationResult", HttpMethod.Get, parameters);

                await push.RegisterAsync(channelUri, templates);
                parameters = new Dictionary<string, string>()
                {
                    {"channelUri", TrimDeviceToken(channelUri)},
                    {"templates", JsonConvert.SerializeObject(expectedTemplates)}
                };
                await this.GetClient().InvokeApiAsync("verifyRegisterInstallationResult", HttpMethod.Get, parameters);

                await push.RegisterAsync(channelUri);
                parameters = new Dictionary<string, string>()
                {
                    {"channelUri", TrimDeviceToken(channelUri)},
                };
                await this.GetClient().InvokeApiAsync("verifyRegisterInstallationResult", HttpMethod.Get, parameters);
            }
            finally
            {
                push.UnregisterAsync().Wait();
                this.GetClient().CurrentUser = null;
            }
        }

        [AsyncTestMethod]
        public async Task RegisterAsyncTemplatesWithJsonBody()
        {
            NSData channelUri = NSDataFromDescription(this.pushTestUtility.GetPushHandle());
            JObject templates = GetTemplates("foo", true);
            JObject expectedTemplates = GetTemplates("testApnsTemplate");
            var push = this.GetClient().GetPush();
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>()
                {
                     {"channelUri", TrimDeviceToken(channelUri)},
                     {"templates", JsonConvert.SerializeObject(expectedTemplates)}
                };
                await push.RegisterAsync(channelUri, templates);
                await this.GetClient().InvokeApiAsync("verifyRegisterInstallationResult", HttpMethod.Get, parameters);
            }
            finally
            {
                push.UnregisterAsync().Wait();
            }
        }

        private async Task VerifyRegistration(Dictionary<string, string> parameters, Push push)
        {
            try
            {
                await this.GetClient().InvokeApiAsync("verifyRegisterInstallationResult", HttpMethod.Get, parameters);
            }
            finally
            {
                push.UnregisterAsync().Wait();
                this.GetClient().CurrentUser = null;
            }
        }

        private static JObject GetTemplates(string tag, bool templateBodyJson = false)
        {
            JObject alert = new JObject();
            alert["alert"] = "$(message)";
            JObject aps = new JObject();
            aps["aps"] = alert;
            JObject templateBody = new JObject();
            templateBody["body"] = aps.ToString();
            if (templateBodyJson)
            {
                templateBody["body"] = aps;
            }
            if (tag != null)
            {
                JArray tags = new JArray { tag };
                templateBody["tags"] = tags;
            }

            JObject templates = new JObject();
            templates["testApnsTemplate"] = templateBody;
            return templates;
        }

        internal static string TrimDeviceToken(NSData deviceToken)
        {
            if (deviceToken == null)
            {
                throw new ArgumentNullException("deviceToken");
            }
            byte[] byteArray = deviceToken.ToArray();
            if (byteArray.Length == 0)
            {
                throw new ArgumentException("deviceToken bytes is empty array.");
            }
            StringBuilder hex = new StringBuilder(byteArray.Length * 2);
            foreach (byte b in byteArray)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        internal static NSData NSDataFromDescription(string hexString)
        {
            NSMutableData data = new NSMutableData();
            byte[] hexAsBytes = new byte[hexString.Length / 2];
            for (int index = 0; index < hexAsBytes.Length; index++)
            {
                hexAsBytes[index] = Convert.ToByte(hexString.Substring(index * 2, 2), 16);
            }

            data.AppendBytes(hexAsBytes);
            return data;
        }
    }
}