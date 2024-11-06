using AIS_APP.Helpers;
using AIS_APP.Models;
using AIS_APP.Services;

namespace AIS_APP.Pages;

public partial class PurchaseTicketPage : ContentPage
{
    private readonly FlightModel _flight;
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public PurchaseTicketPage(FlightModel flight, ApiService apiService, IValidator validator)
    {
        InitializeComponent();

        _flight = flight;
        _apiService = apiService;
        _validator = validator;

        DateOfBirth.Date = DateTime.Now.AddYears(-18); // Set date to 18 years ago

        var ticketPrice = TicketPriceGenerator.GetTicketPrice(_flight.Duration, _flight.AvailableSeatsCount, _flight.FlightCapacity);

        _flight.CurrentTicketPrice = ticketPrice;

        BindingContext = _flight;
    }

    private async void PurchaseTicket_Clicked(object sender, EventArgs e)
    {
        PurchaseTicket.IsEnabled = false;
        LoadingSpinner.IsRunning = true;

        string seat = SelectSeat.SelectedItem as string;
        string title = SelectTitle.SelectedItem as string;
        string fullName = EntFullName.Text;
        string email = EntEmail.Text;
        string phone = EntPhoneNumber.Text;
        string idNum = EntIdNumber.Text;
        DateTime birth = DateOfBirth.Date;

        if (!_flight.AvailableSeats.Contains(seat))
        {
            LoadingSpinner.IsRunning = false;
            PurchaseTicket.IsEnabled = true;
            await DisplayAlert("Error", "Please select a valid seat", "OK");
            return;
        }

        if (string.IsNullOrEmpty(title) || title == "Title")
        {
            LoadingSpinner.IsRunning = false;
            PurchaseTicket.IsEnabled = true;
            await DisplayAlert("Error", "Please select a valid title", "OK");
            return;
        }

        if (string.IsNullOrEmpty(fullName))
        {
            LoadingSpinner.IsRunning = false;
            PurchaseTicket.IsEnabled = true;
            await DisplayAlert("Error", "Please select a valid full name", "OK");
            return;
        }

        if (string.IsNullOrEmpty(idNum))
        {
            LoadingSpinner.IsRunning = false;
            PurchaseTicket.IsEnabled = true;
            await DisplayAlert("Error", "Please select a valid identification number", "OK");
            return;
        }

        var checkResponse = await _apiService.CheckIdNumberOnFlight(idNum, _flight.Id);

        if (checkResponse.IdNumberOnFlight)
        {
            LoadingSpinner.IsRunning = false;
            PurchaseTicket.IsEnabled = true;
            await DisplayAlert("Error", "This identification number is already on this flight", "OK");
            return;
        }

        if (birth == null || birth > DateTime.UtcNow)
        {
            LoadingSpinner.IsRunning = false;
            PurchaseTicket.IsEnabled = true;
            await DisplayAlert("Error", "Please select a valid date of birth", "OK");
            return;
        }

        if (string.IsNullOrEmpty(phone))
        {
            LoadingSpinner.IsRunning = false;
            PurchaseTicket.IsEnabled = true;
            await DisplayAlert("Error", "Please select a valid phone number", "OK");
            return;
        }

        if (string.IsNullOrEmpty(email))
        {
            LoadingSpinner.IsRunning = false;
            PurchaseTicket.IsEnabled = true;
            await DisplayAlert("Error", "Please select a valid email", "OK");
            return;
        }

        PurchaseTicketModel purchase = new()
        {
            FlightId = _flight.Id,
            Seat = seat,
            Title = title,
            FullName = fullName,
            IdNumber = idNum,
            DateOfBirth = birth,
            PhoneNumber = phone,
            Email = email,
        };

        var purchaseResponse = await _apiService.PurchaseTicket(purchase);

        if (!purchaseResponse.HasError)
        {
            LoadingSpinner.IsRunning = false;
            await DisplayAlert("Info", "The ticket was successfully purchased", "OK");
            await Navigation.PushAsync(new HomePage(_apiService, _validator));

            // Remove the current page from the navigation stack so that, in the chance that it doesnt has tickets anymore, the user can't return to it
            var previousPage = Navigation.NavigationStack[Navigation.NavigationStack.Count - 2];
            Navigation.RemovePage(previousPage);
        }
        else
        {
            LoadingSpinner.IsRunning = false;
            PurchaseTicket.IsEnabled = true;
            await DisplayAlert("Error", "Something went wrong purchasing this ticket", "Cancel");
        }
    }
}