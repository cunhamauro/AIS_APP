using AIS_APP.Helpers;
using AIS_APP.Models;
using AIS_APP.Services;

namespace AIS_APP.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    private UserModel User = new UserModel();

    public ProfilePage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await GetUserInfo();
        LoadUserInfo();
    }

    private async Task GetUserInfo()
    {
        LoadingSpinner.IsRunning = true;

        try
        {
            var (user, errorMessage) = await _apiService.GetUserInfo();

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }

            if (user is null)
            {
                LoadingSpinner.IsRunning = false;
                await DisplayAlert("Error", errorMessage ?? "Could not fetch user info", "OK");
                return;
            }
            else
            {
                User = user;
            }
        }
        catch (Exception)
        {
            LoadingSpinner.IsRunning = false;
            await DisplayAlert("Error", "Error fetching user info. Try again later", "OK");
        }
    }

    private void LoadUserInfo()
    {
        LblEmail.Text = User.Email;
        EntFirstName.Text = User.FirstName;
        EntLastName.Text = User.LastName;
        EntPhoneNumber.Text = User.PhoneNumber;

        if (!string.IsNullOrEmpty(User.ImageUrl))
        {
            var imagePath = User.ImageUrl.Substring(2);
            var imageUrl = $"{AppConfig.Url}{imagePath}";

            ProfileImage.Source = imageUrl;
        }

        LoadingSpinner.IsRunning = false;
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private async void UpdateUser_Clicked(object sender, EventArgs e)
    {
        LoadingSpinner.IsRunning = true;

        UpdateUser.IsEnabled = false;

        string firstName = EntFirstName.Text;
        string lastName = EntLastName.Text;
        string phoneNumber = EntPhoneNumber.Text;

        if (string.IsNullOrEmpty(firstName))
        {
            LoadingSpinner.IsRunning = false;
            UpdateUser.IsEnabled = true;
            await DisplayAlert("Error", "Please enter a valid first name", "OK");
            return;
        }

        if (string.IsNullOrEmpty(lastName))
        {
            LoadingSpinner.IsRunning = false;
            UpdateUser.IsEnabled = true;
            await DisplayAlert("Error", "Please enter a valid last name", "OK");
            return;
        }

        if (string.IsNullOrEmpty(phoneNumber))
        {
            LoadingSpinner.IsRunning = false;
            UpdateUser.IsEnabled = true;
            await DisplayAlert("Error", "Please enter a valid phone number", "OK");
            return;
        }

        UpdateUserModel update = new()
        {
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
        };

        var updateResponse = await _apiService.UpdateUser(update);

        if (!updateResponse.HasError)
        {
            LoadingSpinner.IsRunning = false;
            await DisplayAlert("Info", "The user was successfully updated", "OK");

            await GetUserInfo();
            LoadUserInfo();
        }
        else
        {
            LoadingSpinner.IsRunning = false;
            await DisplayAlert("Error", "Something went wrong updating user", "Cancel");
        }

        UpdateUser.IsEnabled = true;
    }

    private async Task<byte[]> SelectImageAsync()
    {
        try
        {
            var archive = await MediaPicker.PickPhotoAsync();

            if (archive is null)
            {
                return null;
            }

            using (var stream = await archive.OpenReadAsync())
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Error", $"The functionality is not supported in this device", "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Error", $"Permission not granted to access camera or gallery", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"There was an error selecting the image: {ex.Message}", "OK");
        }
        return null;
    }

    private async void ProfileImage_Clicked(object sender, EventArgs e)
    {
        try
        {
            var imageArray = await SelectImageAsync();

            LoadingSpinner.IsRunning = true;
            ProfileImage.IsEnabled = false;

            if (imageArray == null)
            {
                LoadingSpinner.IsRunning = false;
                ProfileImage.IsEnabled = true;
                await DisplayAlert("Error", "Error loading the image", "OK");
                return;
            }

            ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(imageArray));

            var response = await _apiService.UploadUserImage(imageArray);

            if (response.Data)
            {
                await DisplayAlert("Info", "Image successfully uploaded", "OK");
            }
            else
            {
                await DisplayAlert("Error", response.ErrorMessage ?? "There was an unexpected error", "Cancel");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"There was an unexpected error: {ex.Message}", "OK");
        }

        LoadingSpinner.IsRunning = false;
        ProfileImage.IsEnabled = true;
    }

    private async void Logout_Clicked(object sender, EventArgs e)
    {
        // Clear the access token to log the user out
        Preferences.Set("accesstoken", string.Empty);

        // Set the MainPage back to the Shell with the necessary pages, such as Login
        Application.Current!.MainPage = new AppShell(_apiService, _validator);

        // After setting the MainPage to the Shell, navigate to the Login page
        await Shell.Current.GoToAsync("//login");
    }

    private async void UpdatePassword_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new UpdatePasswordPage(_apiService, _validator));
    }
}