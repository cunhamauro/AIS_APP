using AIS_APP.Helpers;
using AIS_APP.Models;
using AIS_APP.Services;
using Microsoft.Maui.Animations;

namespace AIS_APP.Pages;

public partial class FlightHistoryPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public FlightHistoryPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();

        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        LoadingSpinner.IsRunning = true;

        var ticketsList = new List<TicketRecord>();

        try
        {
            var tickets = await GetTicketRecords();
            ticketsList = tickets.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching flight history: {ex.Message}");
        }
        finally
        {
            LoadingSpinner.IsRunning = false;

            if (ticketsList.Count == 0)
            {
                NoItems.IsVisible = true;
            }
        }
    }

    private async Task<IEnumerable<TicketRecord>> GetTicketRecords()
    {
        try
        {
            var (tickets, errorMessage) = await _apiService.GetUserFlightHistory();

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<TicketRecord>();
            }

            if (tickets == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not get the flight history", "OK");
                return Enumerable.Empty<TicketRecord>();
            }

            FlightHistory.ItemsSource = tickets;
            return tickets;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
            return Enumerable.Empty<TicketRecord>();
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}