using AIS_APP.Helpers;
using AIS_APP.Models;
using AIS_APP.Services;
using System.Text.RegularExpressions;

namespace AIS_APP.Pages;

public partial class UpdatePasswordPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public UpdatePasswordPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async void UpdatePassword_Clicked(object sender, EventArgs e)
    {
        UpdatePassword.IsEnabled = false;
        LoadingSpinner.IsRunning = true;

        string currentPassword = EntCurrentPassword.Text;
        string newPassword = EntNewPassword.Text;
        string confirmNewPassword = EntConfirmNewPassword.Text;


        if (string.IsNullOrEmpty(currentPassword))
        {
            LoadingSpinner.IsRunning = false;
            UpdatePassword.IsEnabled = true;
            await DisplayAlert("Error", "Enter your current Password", "Ok");
            return;
        }

        if (newPassword.Length < 8 || !Regex.IsMatch(newPassword, @"[a-zA-Z]") || !Regex.IsMatch(newPassword, @"\d"))
        {
            LoadingSpinner.IsRunning = false;
            UpdatePassword.IsEnabled = true;

            await DisplayAlert("Error", "Enter a valid new Password", "Ok");
            return;
        }

        if (string.IsNullOrEmpty(confirmNewPassword))
        {
            LoadingSpinner.IsRunning = false;
            UpdatePassword.IsEnabled = true;

            await DisplayAlert("Error", "Enter your Password confirmation", "Cancel");
            return;
        }

        if (newPassword != confirmNewPassword)
        {
            LoadingSpinner.IsRunning = false;
            UpdatePassword.IsEnabled = true;

            await DisplayAlert("Error", "The Passwords do not match", "Cancel");
            return;
        }

        UpdatePasswordModel update = new()
        {
            CurrentPassword = currentPassword,
            NewPassword = newPassword,
            Confirm = confirmNewPassword,
        };

        var updateResponse = await _apiService.UpdatePassword(update);

        if (!updateResponse.HasError)
        {
            LoadingSpinner.IsRunning = false;

            await DisplayAlert("Info", "The password was successfully updated", "OK");
            await Navigation.PushAsync(new ProfilePage(_apiService, _validator));
        }
        else
        {
            UpdatePassword.IsEnabled = true;
            LoadingSpinner.IsRunning = false;
            await DisplayAlert("Error", "Something went wrong updating the password", "Cancel");
        }
    }
}