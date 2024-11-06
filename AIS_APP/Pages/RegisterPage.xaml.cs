using AIS_APP.Helpers;
using AIS_APP.Models;
using AIS_APP.Services;

namespace AIS_APP.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public RegisterPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async void BtnRegister_Clicked(object sender, EventArgs e)
    {
        BtnRegister.IsEnabled = false;
        LoadingSpinner.IsRunning = true;

        bool valid = await _validator.Validate(EntFirstName.Text, EntLastName.Text, EntEmail.Text, EntPhone.Text, EntPassword.Text, EntConfirmPassword.Text);

        if (valid)
        {
            RegisterModel register = new()
            {
                FirstName = EntFirstName.Text,
                LastName = EntLastName.Text,
                Email = EntEmail.Text,
                Password = EntPassword.Text,
                ConfirmPassword = EntConfirmPassword.Text,
                PhoneNumber = EntPhone.Text,
            };

            var response = await _apiService.Register(register);

            if (!response.HasError)
            {
                BtnRegister.IsEnabled = true;
                LoadingSpinner.IsRunning = false;
                await DisplayAlert("Info", "Your account was created successfully! Check your email to activate it", "OK");
                await Navigation.PushAsync(new LoginPage(_apiService, _validator));
            }
            else
            {
                BtnRegister.IsEnabled = true;
                LoadingSpinner.IsRunning = false;
                await DisplayAlert("Error", "Something went wrong registering your account", "Cancel");
            }
        }
        else
        {
            string errorMessage = "";

            errorMessage += _validator.FirstNameError != null ? $"\n- {_validator.FirstNameError}" : "";
            errorMessage += _validator.LastNameError != null ? $"\n- {_validator.LastNameError}" : "";
            errorMessage += _validator.EmailError != null ? $"\n- {_validator.EmailError}" : "";
            errorMessage += _validator.PhoneNumberError != null ? $"\n- {_validator.PhoneNumberError}" : "";
            errorMessage += _validator.PasswordError != null ? $"\n- {_validator.PasswordError}" : "";

            BtnRegister.IsEnabled = true;
            LoadingSpinner.IsRunning = false;
            await DisplayAlert("Error", errorMessage, "OK");
        }
    }

    private async void TapLogin_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}