using AIS_APP.Helpers;
using AIS_APP.Models;
using AIS_APP.Services;

namespace AIS_APP.Pages;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public LoginPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async void BtnLogin_Clicked(object sender, EventArgs e)
    {
        LoadingSpinner.IsRunning = true;
        BtnLogin.IsEnabled = false;

        if (string.IsNullOrEmpty(EntEmail.Text))
        {
            LoadingSpinner.IsRunning = false;
            BtnLogin.IsEnabled = true;
            await DisplayAlert("Error", "Enter your Email", "Cancel");
            return;
        }

        if (string.IsNullOrEmpty(EntPassword.Text))
        {
            LoadingSpinner.IsRunning = false;
            BtnLogin.IsEnabled = true;
            await DisplayAlert("Error", "Enter your Password", "Cancel");
            return;
        }

        LoginModel login = new()
        {
            Email = EntEmail.Text,
            Password = EntPassword.Text,
        };

        var response = await _apiService.Login(login);

        if (!response.HasError)
        {
            LoadingSpinner.IsRunning = false;
            Application.Current!.MainPage = new AppShell(_apiService, _validator);
        }
        else
        {
            LoadingSpinner.IsRunning = false;
            BtnLogin.IsEnabled = true;

            await DisplayAlert("Error", "Something went wrong", "Cancel");
        }
    }

    private async void TapRegister_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(_apiService, _validator));
    }

    private async void TapForgotPassword_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ForgotPasswordPage(_apiService, _validator));
    }
}