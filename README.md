# Azure Mobile Apps: .NET Client SDK

With Azure Mobile Apps you can add a scalable backend to your connected client applications in minutes. To learn more, visit our [Developer Center](http://azure.microsoft.com/en-us/develop/mobile) and the [App Service Mobile learning path](https://azure.microsoft.com/en-us/documentation/learning-paths/appservice-mobileapps/).

## Getting Started

If you are new to Azure Mobile Apps, you can get started by following our tutorials for connecting to your hosted cloud backend with a [Xamarin.Forms client](https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-xamarin-forms-get-started/) or [Windows client](https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-windows-store-dotnet-get-started/).

## Download Source Code

To get the source code of our SDKs and samples via **git** just type:

    git clone https://github.com/Azure/azure-mobile-apps-net-client.git
    cd ./azure-mobile-apps-net-client/

## Reference Documentation

## Change log
- [Managed SDK](CHANGELOG.md)

## Managed Windows Client SDK

Our managed portable library for Window and Xamarin makes it easy to use Azure Mobile Apps from your managed client applications. The [Azure Mobile Client SDK](https://www.nuget.org/packages/Microsoft.Azure.Mobile.Client/) is available
as a NuGet package or you can download the source using the instructions above. The managed portable library also supports the full .NET 4.5 platform.

To learn more about the client library, see [How to use the managed client for Azure Mobile Apps](https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-dotnet-how-to-use-client-library/).

### Prerequisites

The SDK requires Visual Studio 2015.

### Building and Referencing the SDK

The managed portable library solution includes a core portable assembly and platform-specific assemblies for each of the supported platforms: Xamarin.iOS, Xamarin.Android, Windows 8.1, Windows Phone 8.1 and .NET 4.5. The core portable platform project is ```Microsoft.WindowsAzure.Mobile```. The platform-specific assembly projects are
named using a ```Microsoft.WindowsAzure.Mobile.Ext.<Platform>``` convention. The Windows Phone 8 platform also
include a ```Microsoft.WindowsAzure.Mobile.UI.<Platform>``` project that contain UI components. To build the Managed Portable Libray:

1. Open the ```Microsoft.WindowsAzure.Mobile.Managed_Windows.sln``` solution file in Visual Studio 2015.
2. Press F6 to build the solution.

### Running the Tests

The managed portable library ```Microsoft.WindowsAzure.Mobile.Managed_Windows.sln``` has a test application for each of the supported platforms: Windows 8,
Windows Phone 8 and .NET 4.5.

1. Open the ```Microsoft.WindowsAzure.Mobile.Managed_Windows.sln``` solution file in Visual Studio 2013.
2. Right-click on the test project for a given platform in the Solution Explorer and select ```Set as StartUp Project```.
3. Press F5 to run the application in debug mode.
4. An application will appear with a prompt for a runtime Uri and Tags. You can safely ignore this prompt and just click the Start button.
5. The test suite will run and display the results.

### Running the Xamarin.iOS E2E test using Xamarin Studio
1. Open the client folder in Console or Terminal and execute the following:  
   
   ```git submodule init```  
   ```git submodule update```  
2. Open the ```e2etest/iOS.E2ETest/iOS.E2ETest.csproj``` file in Xamarin Studio, build and run.

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
