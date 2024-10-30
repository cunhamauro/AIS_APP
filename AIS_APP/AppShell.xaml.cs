using AIS_APP.Helpers;
using AIS_APP.Services;

namespace AIS_APP
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;

        public AppShell(ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService;
            _validator = validator;
        }

        private void ConfigureShell()
        {
            var homePage = new HomePage(_apiService, _validator);
            var flightHistory = new FlightHistoryPage(_apiService, _validator);
            var scheduledFlights = new ScheduledFlightsPage(_apiService, _validator);
            var profilePage = new ProfilePage(_apiService, _validator);

            Items.Add(new TabBar
            {
                Items =
                {
                    new ShellContent {Title = "Home", Icon = "home", Content = homePage},
                    new ShellContent {Title = "Flight History", Icon = "history", Content = flightHistory},
                    new ShellContent {Title = "Scheduled Flights", Icon = "plane", Content = scheduledFlights},
                    new ShellContent {Title = "Profile", Icon = "profile", Content = profilePage},
                }
            });
        }
    }
}
