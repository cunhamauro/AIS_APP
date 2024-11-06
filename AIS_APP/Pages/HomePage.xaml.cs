using AIS_APP.Helpers;
using AIS_APP.Models;
using AIS_APP.Services;

namespace AIS_APP.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    List<FlightModel> AvailableFlights = new List<FlightModel>();

    public HomePage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();

        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        LoadingSpinner.IsRunning = true;

        try
        {
            var flights = await GetAvailableFlights();
            AvailableFlights = flights.ToList();

            FillFilterLists(flights);
        }
        catch (Exception ex)
        {
            LoadingSpinner.IsRunning = false;
            Console.WriteLine($"Error fetching flights: {ex.Message}");
        }
        finally
        {
            LoadingSpinner.IsRunning = false;

            if (AvailableFlights.Count == 0)
            {
                NoItems.IsVisible = true;
            }
        }
    }

    private void FillFilterLists(IEnumerable<FlightModel> flights)
    {
        List<string> OriginsList = new();
        List<string> DestinationsList = new();


        foreach (var flight in flights)
        {
            OriginsList.Add($"{flight.OriginCity}, {flight.OriginCountry}");
            DestinationsList.Add($"{flight.DestinationCity}, {flight.DestinationCountry}");
        }

        OriginsList = OriginsList.Distinct().ToList();
        DestinationsList = DestinationsList.Distinct().ToList();

        OriginPicker.ItemsSource = OriginsList;
        DestinationPicker.ItemsSource = DestinationsList;
    }

    private async Task<IEnumerable<FlightModel>> GetAvailableFlights()
    {
        try
        {
            var (flights, errorMessage) = await _apiService.GetAvailableFlights();

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<FlightModel>();
            }

            if (flights == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not get the available flights", "OK");
                return Enumerable.Empty<FlightModel>();
            }

            Flights.ItemsSource = flights;
            return flights;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
            return Enumerable.Empty<FlightModel>();
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private async void Filter_Clicked(object sender, EventArgs e)
    {
        LoadingSpinner.IsRunning = true;
        Filter.IsEnabled = false;

        bool filterByOrigin = FilterByOriginCheckBox.IsChecked;
        bool filterByDestination = FilterByDestinationCheckBox.IsChecked;
        bool filterByDeparture = FilterByDepartureCheckBox.IsChecked;
        bool filterByArrival = FilterByArrivalCheckBox.IsChecked;

        string filterOrigin = OriginPicker.SelectedItem as string;
        string filterDestination = DestinationPicker.SelectedItem as string;

        DateTime filterDeparture = DepartureDatePicker.Date;
        DateTime filterArrival = ArrivalDatePicker.Date;

        List<FlightModel> flightsFiltered = new(AvailableFlights);

        if (AvailableFlights != null && AvailableFlights.Any())
        {
            // Apply filters if selected
            if (filterByOrigin && filterOrigin != null)
            {
                flightsFiltered = flightsFiltered.Where(f => $"{f.OriginCity}, {f.OriginCountry}" == filterOrigin).ToList();
            }
            else if (filterByOrigin && (string.IsNullOrEmpty(filterOrigin) || filterOrigin == "Select Origin"))
            {
                await DisplayAlert("Error", "Select a valid Origin to filter", "Ok");
            }

            if (filterByDestination && filterDestination != null)
            {
                flightsFiltered = flightsFiltered.Where(f => $"{f.DestinationCity}, {f.DestinationCountry}" == filterDestination).ToList();
            }
            else if (filterByDestination && (string.IsNullOrEmpty(filterDestination) || filterDestination == "Select Destination"))
            {
                await DisplayAlert("Error", "Select a valid Destination to filter", "Ok");
            }

            if (filterByDeparture && filterDeparture != null)
            {
                flightsFiltered = flightsFiltered.Where(f => f.Arrival >= filterDeparture).ToList();
            }
            else if (filterByDeparture && filterDeparture == null)
            {
                await DisplayAlert("Error", "Select a valid Departure date to filter", "Ok");
            }

            if (filterByArrival && filterArrival != null)
            {
                flightsFiltered = flightsFiltered.Where(f => f.Arrival < filterArrival).ToList();
            }
            else if (filterByArrival && filterArrival == null)
            {
                await DisplayAlert("Error", "Select a valid Arrival date to filter", "Ok");
            }
        }

        Flights.ItemsSource = flightsFiltered;
        Filter.IsEnabled = true;
        LoadingSpinner.IsRunning = false;
    }

    private async void FlightTapped_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is FlightModel selectedFlight)
        {
            await Navigation.PushAsync(new FlightDetailsPage(selectedFlight.Id, selectedFlight.FlightNumber, _apiService, _validator));
        }
    }
}