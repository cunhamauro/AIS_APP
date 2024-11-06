using AIS_APP.Helpers;
using AIS_APP.Models;
using AIS_APP.Services;
using System.Text.RegularExpressions;

namespace AIS_APP.Pages;

public partial class SetPasswordPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public SetPasswordPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async void BtnSetPassword_Clicked(object sender, EventArgs e)
    {
        BtnSetPassword.IsEnabled = false;
        LoadingSpinner.IsRunning = true;

        if (string.IsNullOrEmpty(EntEmail.Text))
        {
            LoadingSpinner.IsRunning = false;
            BtnSetPassword.IsEnabled = true;
            await DisplayAlert("Error", "Enter your Email", "Ok");
            return;
        }

        if (string.IsNullOrEmpty(EntToken.Text))
        {
            LoadingSpinner.IsRunning = false;
            BtnSetPassword.IsEnabled = true;
            await DisplayAlert("Error", "Enter your Token", "Ok");
            return;
        }

        if (string.IsNullOrEmpty(EntPassword.Text))
        {
            LoadingSpinner.IsRunning = false;
            BtnSetPassword.IsEnabled = true;
            await DisplayAlert("Error", "Enter your Password", "Ok");
            return;
        }

        if (EntPassword.Text.Length < 8 || !Regex.IsMatch(EntPassword.Text, @"[a-zA-Z]") || !Regex.IsMatch(EntPassword.Text, @"\d"))
        {
            LoadingSpinner.IsRunning = false;
            BtnSetPassword.IsEnabled = true;
            await DisplayAlert("Error", "Enter a valid Password", "Ok");
            return;
        }

        if (string.IsNullOrEmpty(EntConfirmPassword.Text))
        {
            LoadingSpinner.IsRunning = false;
            BtnSetPassword.IsEnabled = true;
            await DisplayAlert("Error", "Enter your Password confirmation", "Cancel");
            return;
        }

        if (EntPassword.Text != EntConfirmPassword.Text)
        {
            LoadingSpinner.IsRunning = false;
            BtnSetPassword.IsEnabled = true;
            await DisplayAlert("Error", "The Passwords do not match", "Cancel");
            return;
        }

        var reset = new ResetPasswordModel()
        {
            Email = EntEmail.Text,
            Token = EntToken.Text,
            NewPassword = EntPassword.Text,
            ConfirmPassword = EntConfirmPassword.Text,
        };

        var response = await _apiService.ResetPassword(reset);

        if (!response.HasError)
        {
            BtnSetPassword.IsEnabled = true;
            LoadingSpinner.IsRunning = false;
            await DisplayAlert("Info", "Your new password has been successfully set", "Ok");
            await Navigation.PushAsync(new LoginPage(_apiService, _validator));
        }
        else
        {
            BtnSetPassword.IsEnabled = true;
            LoadingSpinner.IsRunning = false;
            await DisplayAlert("Error", "Something went wrong", "Cancel");
        }
    }
}