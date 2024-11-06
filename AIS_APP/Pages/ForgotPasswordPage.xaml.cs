using AIS_APP.Helpers;
using AIS_APP.Models;
using AIS_APP.Services;

namespace AIS_APP.Pages;

public partial class ForgotPasswordPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public ForgotPasswordPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async void BtnRecoverAccount_Clicked(object sender, EventArgs e)
    {
        BtnRecoverAccount.IsEnabled = false;
        LoadingSpinner.IsRunning = true;

        if (string.IsNullOrEmpty(EntEmail.Text))
        {
            BtnRecoverAccount.IsEnabled = true;
            LoadingSpinner.IsRunning = false;
            await DisplayAlert("Error", "Enter your Email", "Cancel");
            return;
        }

        ForgotPasswordModel forgot = new()
        {
            Email = EntEmail.Text,
        };

        var response = await _apiService.ForgotPassword(forgot);

        if (!response.HasError)
        {
            BtnRecoverAccount.IsEnabled = true;
            LoadingSpinner.IsRunning = false;

            await DisplayAlert("Info", "Get the token sent to your email to set a new password", "Ok");

            await Navigation.PushAsync(new SetPasswordPage(_apiService, _validator));

            //Application.Current!.MainPage = new AppShell(_apiService, _validator);
        }
        else
        {
            BtnRecoverAccount.IsEnabled = true;
            LoadingSpinner.IsRunning = false;

            await DisplayAlert("Error", "Something went wrong", "Cancel");
        }
    }
}