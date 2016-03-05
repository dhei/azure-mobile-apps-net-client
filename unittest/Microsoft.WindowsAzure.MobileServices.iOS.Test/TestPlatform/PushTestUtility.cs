// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.MobileServices.TestFramework;
using Newtonsoft.Json.Linq;
using Foundation;

namespace Microsoft.WindowsAzure.MobileServices.Test
{
    class PushTestUtility : IPushTestUtility
    {
        private const string DefaultDeviceToken =
            "<f6e7cd2 80fc5b5 d488f8394baf216506bc1bba 864d5b483d>";
        const string DefaultToastTemplateName = "templateForToastiOS";

        public string GetPushHandle()
        {
            return DefaultDeviceToken;
        }

        public string GetUpdatedPushHandle()
        {
            return DefaultDeviceToken.Replace('b', 'a').Replace('B', 'a');
        }

        public JObject GetInstallation(string installationId, bool includeTemplates = false, bool templateBodyJson = false, string defaultChannelUri = null)
        {
            JObject installation = new JObject();
            installation[PushInstallationProperties.PUSHCHANNEL] = defaultChannelUri ?? TrimDeviceToken(DefaultDeviceToken);
            installation[PushInstallationProperties.PLATFORM] = Platform.Instance.PushUtility.GetPlatform();
            if (includeTemplates)
            {
                installation[PushInstallationProperties.TEMPLATES] = GetTemplates(templateBodyJson);
            }
            return installation;
        }

        public JObject GetTemplates(bool templateBodyJson = false)
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
            JObject templates = new JObject();
            templates[DefaultToastTemplateName] = templateBody;
            return templates;
        }

        internal static string TrimDeviceToken(string deviceToken)
        {
            if (deviceToken == null)
            {
                throw new ArgumentNullException("deviceToken");
            }

            return deviceToken.Trim('<', '>').Replace(" ", string.Empty).ToUpperInvariant();
        }
    }
}