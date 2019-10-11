# Azure Mobile Apps: .NET Client SDK

With Azure Mobile Apps you can add a scalable backend to your connected client applications in minutes.

# Visual Studio App Center as modern and integrated solution for mobile development
 Visual Studio App Center supports end to end and integrated services central to mobile app development. Developers can use the **Build**, **Test** and **Distribute** services to set up Continuous Integration and Delivery pipelines. Once the app is deployed, developers can monitor the status and usage of their app using the **Analytics** and **Diagnostics** services, and engage with users using the **Push** service. Developers can also leverage **Auth** to authenticate their users and **Data** to persist and sync app data in the cloud.
 
If you are looking to integrate cloud services in your mobile application, sign up with [App Center](https://appcenter.ms/signup?utm_source=zumo&utm_medium=Azure&utm_campaign=GitHub) today.

## Getting Started

If you are new to Azure Mobile Apps, you can get started by following our tutorials for connecting to your hosted cloud backend with a [Xamarin.Forms client](https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-xamarin-forms-get-started/) or [Windows client](https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-windows-store-dotnet-get-started/).

## Download Source Code

To get the source code of our SDKs and samples via **git** just type:

    git clone https://github.com/Azure/azure-mobile-apps-net-client.git
    cd ./azure-mobile-apps-net-client/
    git submodule init
    git submodule update

Please note that this project uses git submodules which isn't included in the archive if you are using "Download ZIP" button on GitHub.

## Supported platforms

* .NET Standard 1.4
* Xamarin Android for API 19 through 24 (KitKat through Nougat)
* Xamarin iOS for iOS versions 8.0 through 10.0
* Xamarin.Forms (Android, iOS and UWP)
* Universal Windows Platform

Other versions may work.  We do not test them and thus cannot support them.

## Change log
- [Managed SDK](CHANGELOG.md)

## Managed Windows Client SDK

Our managed portable library for Window and Xamarin makes it easy to use Azure Mobile Apps from your managed client applications. The [Azure Mobile Client SDK](https://www.nuget.org/packages/Microsoft.Azure.Mobile.Client/) is available as a NuGet package or you can download the source using the instructions above.

To learn more about the client library, see [How to use the managed client for Azure Mobile Apps](https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-dotnet-how-to-use-client-library/).

### Prerequisites

The SDK requires Visual Studio 2017.

### Building and Referencing the SDK

1. Open the ```Microsoft.Azure.Mobile.Client.sln``` solution file in Visual Studio 2017.
2. Ensure you have connected an iOS Build Agent prior to building
2. Use Solution -> Restore NuGet Packages...
3. Press F6 to build the solution.

### Running the Unit Tests

The following test suites under the 'unittest' directory contain the unit tests:

* Microsoft.WindowsAzure.Mobile._platform_.Test
* Microsoft.WindowsAzure.Mobile.SQLiteStore._platform_.Test

Mark the appropriate project as the Startup project and run it.  The UI will open and then you can run the tests.  If in doubt,
run the Net 4.6.1 platform locally as a minimal unit test.

### Running the E2E Tests

You must have a working test endpoint to run the tests.  The test endpoint is not included and may take some time to set up.  If
you need a working test endpoint, please reach out to us on Twitter or via the GitHub Issues.  The Azure Mobile Apps team will run
the E2E tests prior to publication.

If you are an Azure Mobile Apps team member, you can set the appropriate e2etest project as active, build, and use a configured
endpoint.

## Future of Azure Mobile Apps
 
Microsoft is committed to fully supporting Azure Mobile Apps, including **support for the latest OS release, bug fixes, documentation improvements, and community PR reviews**. Please note that the product team is **not currently investing in any new feature work** for Azure Mobile Apps. We highly appreciate community contributions to all areas of Azure Mobile Apps. 

## Useful Resources

* [Quickstarts](https://github.com/Azure/azure-mobile-apps-quickstarts)
* [E2E Test Suite](e2etest)
* [Samples](https://azure.microsoft.com/en-us/documentation/samples/?service=app-service&term=mobile)
* Tutorials and product overview are available at [Azure Mobile Apps Developer Center](http://azure.microsoft.com/en-us/develop/mobile).
* Our product team actively monitors the [Mobile Apps Developer Forum](http://social.msdn.microsoft.com/Forums/en-US/azuremobile/) to assist you with any troubles and the StackOverflow tag [azure-mobile-services](http://stackoverflow.com/questions/tagged/azure-mobile-services).
* Our product team publishes symbols to SymbolSource for an improved debugging experience. Instructions on enabling VisualStudio to load symbols from SymbolSource [here](http://www.symbolsource.org/Public/Wiki/Using)

## Contribute Code or Provide Feedback

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

If you would like to become an active contributor to this project please follow the instructions provided in [Microsoft Azure Projects Contribution Guidelines](http://azure.github.com/guidelines.html).

If you encounter any bugs with the library please file an issue in the [Issues](https://github.com/Azure/azure-mobile-apps-net-client/issues) section of the project.
