using Windows.Security.Credentials;
using Windows.Security.Credentials.UI;
using Windows.UI.Xaml.Controls;
using System;

namespace HelloUnlocker.Controls
{
    public sealed partial class AdviceDialog : ContentDialog
    {
        public bool Success { get; set; }

        public AdviceDialog(string message)
        {
            this.InitializeComponent();
            this.Message.Text = message;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            var ucvAvailability = await UserConsentVerifier.CheckAvailabilityAsync();

            if (ucvAvailability == UserConsentVerifierAvailability.Available)
            {
                var consentResult = await UserConsentVerifier.RequestVerificationAsync(this.Title.ToString());

                this.Success = (consentResult == UserConsentVerificationResult.Verified);
                this.Hide();
                return;
            }
            //else
            //{
            //    bool keyCredentialAvailable = await KeyCredentialManager.IsSupportedAsync();
            //    KeyCredentialRetrievalResult keyCreationResult = await KeyCredentialManager.RequestCreateAsync("igor.damiani@hotmail.com",
            //        KeyCredentialCreationOption.ReplaceExisting);

            //    this.Success = (keyCreationResult.Status == KeyCredentialStatus.Success);
            //    this.Hide();
            //    return;
            //}
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Success = false;
        }
    }
}