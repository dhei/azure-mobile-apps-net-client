using System;
using System.Net.Http;
using System.Reflection;
using Microsoft.WindowsAzure.MobileServices.TestFramework;
// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227
using Newtonsoft.Json.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Microsoft.WindowsAzure.MobileServices.Test
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static MobileServiceClient LoginMobileService;

        /// <summary>
        /// Gets the test harness used by the application.
        /// </summary>
        public static TestHarness Harness { get; private set; }

        private const string E2E_TEST_BLOB_STORAGE_CONTAINER = @"TestInput\e2e_test_storage_url.txt";
        private const string E2E_TEST_BLOB_STORAGE_CONTAINER_SAS_TOKEN = @"TestInput\e2e_test_storage_sas_token.txt";
        private const string INPUT_PARAM_FILE = "windows_client_input.json";

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            Harness = new TestHarness();
            Harness.Platform = TestPlatform.WindowsPhone81;
            Harness.LoadTestAssembly(typeof(FunctionalTestBase).GetTypeInfo().Assembly);
            Harness.LoadTestAssembly(typeof(PushFunctional).GetTypeInfo().Assembly);
        }

        /// <summary>
        /// Handles back button press.  If app is at the root page of app, don't go back and the
        /// system will suspend the app.
        /// </summary>
        /// <param name="sender">The source of the BackPressed event.</param>
        /// <param name="e">Details for the BackPressed event.</param>
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            if (frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();

            string storageBlobContainerUrl = ReadFile(E2E_TEST_BLOB_STORAGE_CONTAINER);
            string storageSasTokenBase64Encoded = ReadFile(E2E_TEST_BLOB_STORAGE_CONTAINER);

            if (!String.IsNullOrEmpty(storageSasTokenBase64Encoded))
            {
                // Automated testing
                string storageSasToken = TestHarness.DecodeBase64String(storageSasTokenBase64Encoded);

                var testConfig = DownloadInputFromStorage(storageBlobContainerUrl, storageSasToken, INPUT_PARAM_FILE);
                App.Harness.SetAutoConfig(testConfig);
                rootFrame.Navigate(typeof(TestPage), e.Arguments);
            }
        }

        private static TestConfig DownloadInputFromStorage(string storageUrl, string storageSasToken, string inputFilePath)
        {
            string storageSasUrl = TestHarness.GetBlobStorageSasUrl(storageUrl, inputFilePath, storageSasToken);

            HttpClient client = new HttpClient();
            var response = client.GetAsync(storageSasUrl).Result;
            var inputFileContent = response.Content.ReadAsStringAsync().Result;

            return TestHarness.GenerateTestConfigFromInputFile(storageSasToken, inputFileContent);
        }

        public string ReadFile(string filePath)
        {
            var file = Package.Current.InstalledLocation.GetFileAsync(filePath).GetAwaiter().GetResult();
            return FileIO.ReadTextAsync(file).GetAwaiter().GetResult();
        }

        protected override void OnActivated(IActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            //Check if this is a continuation
            var continuationEventArgs = e as IContinuationActivatedEventArgs;
            if (continuationEventArgs != null)
            {
                if (continuationEventArgs.Kind == ActivationKind.WebAuthenticationBrokerContinuation)
                {
                    LoginMobileService.LoginComplete(continuationEventArgs as WebAuthenticationBrokerContinuationEventArgs);
                }
            }

            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails.
        /// </summary>
        /// <param name="sender">The Frame which failed navigation.</param>
        /// <param name="e">Details about the navigation failure.</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
