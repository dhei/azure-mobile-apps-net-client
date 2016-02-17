// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.MobileServices.TestFramework;
using Newtonsoft.Json.Linq;

namespace Microsoft.WindowsAzure.MobileServices.Test
{
    class PushTestUtility : IPushTestUtility
    {
        private const string DefaultChannelUri = "17BA0791499DB908433B80F37C5FBC89B870084B";
        const string DefaultToastTemplateName = "templateForToastGcm";
        public string GetPushHandle()
        {
            return DefaultChannelUri;
        }

        public string GetUpdatedPushHandle()
        {
            return DefaultChannelUri.Replace('A', 'B');
        }

        public JObject GetInstallation(string installationId, bool includeTemplates = false, bool templateBodyJson = false, string defaultChannelUri = null)
        {
            JObject installation = new JObject();
            installation[PushInstallationProperties.PUSHCHANNEL] = defaultChannelUri ?? DefaultChannelUri;
            installation[PushInstallationProperties.PLATFORM] = Platform.Instance.PushUtility.GetPlatform();
            if (includeTemplates)
            {
                installation[PushInstallationProperties.TEMPLATES] = GetTemplates(templateBodyJson);
            }
            return installation;
        }

        public JObject GetTemplates(bool templateBodyJson = false)
        {
            JObject msg = new JObject();
            msg["msg"] = "$(message)";
            JObject data = new JObject();
            data["data"] = msg;
            JObject templateBody = new JObject();
            templateBody["body"] = data.ToString();
            if (templateBodyJson)
            {
                templateBody["body"] = data;
            }
            JObject templates = new JObject();
            templates[DefaultToastTemplateName] = templateBody;
            return templates;
        }
    }
}