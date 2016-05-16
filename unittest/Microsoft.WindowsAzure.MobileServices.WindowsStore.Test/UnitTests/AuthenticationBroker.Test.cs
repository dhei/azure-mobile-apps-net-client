// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using Microsoft.WindowsAzure.MobileServices.TestFramework;

namespace Microsoft.WindowsAzure.MobileServices.Test
{
    [Tag("authentication")]
    [Tag("unit")]
    public class AuthenticationBrokerTest : TestBase
    {
        [TestMethod]
        public void GetUrlWithQueryStringParameter_StartUrlContainsNoQueryParameters()
        {
            GetUrlWithQueryStringParameter_Setup(
                startUrl: "https://foobar.azurewebsites.net/.auth/login/aad",
                param: "param",
                paramValue: "value",
                expectedResult: "https://foobar.azurewebsites.net/.auth/login/aad?param=value");
        }

        [TestMethod]
        public void GetUrlWithQueryStringParameter_StartUrlContainsOneQueryParameter()
        {
            GetUrlWithQueryStringParameter_Setup(
                startUrl: "https://foobar.azurewebsites.net/.auth/login/aad?param1=value1",
                param: "param2",
                paramValue: "value2",
                expectedResult: "https://foobar.azurewebsites.net/.auth/login/aad?param1=value1&param2=value2");
        }

        [TestMethod]
        public void GetUrlWithQueryStringParameter_StartUrlContainsTwoQueryParameters()
        {
            GetUrlWithQueryStringParameter_Setup(
                startUrl: "https://foobar.azurewebsites.net/.auth/login/aad?param1=value1&param2=value2",
                param: "param3",
                paramValue: "value3",
                expectedResult: "https://foobar.azurewebsites.net/.auth/login/aad?param1=value1&param2=value2&param3=value3");
        }

        private static void GetUrlWithQueryStringParameter_Setup(string startUrl, string param, string paramValue, string expectedResult)
        {
            Uri result = new AuthenticationBroker().GetUrlWithQueryStringParameter(new Uri(startUrl), param, paramValue);

            Assert.AreEqual(expectedResult, result.AbsoluteUri);
        }
    }
}
