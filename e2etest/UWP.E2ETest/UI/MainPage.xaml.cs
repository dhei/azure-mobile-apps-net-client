// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Microsoft.WindowsAzure.MobileServices.Test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            string mobileServiceRuntimeUrl = ApplicationData.Current.LocalSettings.Values["MobileServiceRuntimeUrl"] as string;
            string tags = ApplicationData.Current.LocalSettings.Values["MobileServiceTags"] as string;

            _RuntimeUriTextbox.Text = mobileServiceRuntimeUrl ?? "";
            _TagsTextBox.Text = tags ?? "";

            this.Loaded += (s, e) => _UnitTestsButton.Focus(FocusState.Keyboard);
        }

        /// <summary>
        /// Execute the login tests on button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void LoginTestsButton_Click(object sender, RoutedEventArgs e)
        {
            App.Harness.Settings.Custom["MobileServiceRuntimeUrl"] = _RuntimeUriTextbox.Text;
            ApplicationData.Current.LocalSettings.Values["MobileServiceRuntimeUrl"] = _RuntimeUriTextbox.Text;

            Frame.Navigate(typeof(LoginPage));
        }

        /// <summary>
        /// Execute unit tests on button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnitTestsButton_Click(object sender, RoutedEventArgs e)
        {
            App.Harness.Settings.Custom["MobileServiceRuntimeUrl"] = _RuntimeUriTextbox.Text;
            App.Harness.Settings.TagExpression = _TagsTextBox.Text;

            ApplicationData.Current.LocalSettings.Values["MobileServiceRuntimeUrl"] = _RuntimeUriTextbox.Text;
            ApplicationData.Current.LocalSettings.Values["MobileServiceTags"] = _TagsTextBox.Text;

            Frame.Navigate(typeof(TestPage));
        }
    }
}
