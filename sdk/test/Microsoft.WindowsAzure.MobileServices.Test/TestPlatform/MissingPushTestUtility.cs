using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Microsoft.WindowsAzure.MobileServices.Test
{
    class MissingPushTestUtility : IPushTestUtility
    {
        public string GetPushHandle()
        {
            throw new NotImplementedException();
        }

        public string GetUpdatedPushHandle()
        {
            throw new NotImplementedException();
        }

        public JObject GetInstallation(string installationId, bool includeTemplates = false, bool templateBodyJson = false, string defaultChannelUri = null)
        {
            throw new NotImplementedException();
        }

        public JObject GetTemplates(bool templateBodyJson)
        {
            throw new NotImplementedException();
        }
    }
}
