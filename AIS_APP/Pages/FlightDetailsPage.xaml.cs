using AIS_APP.Helpers;
using AIS_APP.Models;
using AIS_APP.Services;
using System.Diagnostics;

namespace AIS_APP.Pages;

public partial class FlightDetailsPage : ContentPage
{
    private bool _loginPageDisplayed = false;
    private readonly int _flightId;
    private readonly string _flightNumber;
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private FlightModel FlightModel = new FlightModel();

    public FlightDetailsPage(int flightId, string flightNumber, ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _flightId = flightId;
        _flightNumber = flightNumber;
        _apiService = apiService;
        _validator = validator;

        Title = "Flight " + flightNumber ?? "Flight";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await GetFlightById(_flightId);
    }

    private async Task GetFlightById(int flightId)
    {
        try
        {
            var (flight, errorMessage) = await _apiService.GetFlightById(flightId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }

            if (flight is null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not fetch the selected flight", "OK");
                return;
            }
            else
            {
                FlightModel = flight;
                BindingContext = flight;
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Error fetching flight. Try again later", "OK");
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private async void PurchaseTicket_Clicked(object sender, EventArgs e)
    {
        PurchaseTicket.IsEnabled = false;

        var accessToken = Preferences.Get("accesstoken", string.Empty);

        if (string.IsNullOrEmpty(accessToken))
        {
            //    // When going to the home page through the login page go through routing to not add a page to navigation
            //    // So the Home tab is selected correctly and doesnt stay in Home with Login selected

            //Preferences.Set("PreviousPage", "flightdetails");
            //Preferences.Set("FD_FlightId", $"{_flightId}");
            //Preferences.Set("FD_FlightNumber", $"{_flightNumber}");

            await Shell.Current.GoToAsync("//login");

            //await Navigation.PushAsync(new LoginPage(_apiService, _validator));

        }
        else
        {
            await Navigation.PushAsync(new PurchaseTicketPage(FlightModel, _apiService, _validator));
        }
        PurchaseTicket.IsEnabled = true;
    }
}