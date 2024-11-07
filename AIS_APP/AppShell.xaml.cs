using AIS_APP.Helpers;
using AIS_APP.Pages;
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
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _validator = validator;

            ConfigureShell();
        }

        private void ConfigureShell()
        {
            var accessToken = Preferences.Get("accesstoken", string.Empty);

            var homePage = new HomePage(_apiService, _validator);
            var flightHistory = new FlightHistoryPage(_apiService, _validator);
            var scheduledFlights = new ScheduledFlightsPage(_apiService, _validator);
            var profilePage = new ProfilePage(_apiService, _validator);
            var info = new InfoPage(_apiService, _validator);
            var login = new LoginPage(_apiService, _validator);

            // Register routes for each page to allow direct page navigation without adding to stack
            //Routing.RegisterRoute("home", typeof(HomePage));
            Routing.RegisterRoute("login", typeof(LoginPage));

            if (string.IsNullOrEmpty(accessToken))
            {
                // Adding the Login and Home tabs for unauthenticated users
                Items.Add(new TabBar
                {
                    Items =
                    {
                        new ShellContent {Title = "Home", Icon = "home", Content = homePage/*, Route = "home"*/},
                        new ShellContent {Title = "Login", Icon = "logout", Content = login, Route = "login"},
                        new ShellContent {Title = "About", Icon = "info", Content = info},
                    }
                });
            }
            else
            {
                // Adding the authenticated user tabs
                Items.Add(new TabBar
                {
                    Items =
                    {
                        new ShellContent {Title = "Home", Icon = "home", Content = homePage/*, Route = "home"*/},
                        new ShellContent {Title = "History", Icon = "history", Content = flightHistory},
                        new ShellContent {Title = "Flights", Icon = "scheduled", Content = scheduledFlights},
                        new ShellContent {Title = "Profile", Icon = "profile", Content = profilePage},
                        new ShellContent {Title = "About", Icon = "info", Content = info},
                    }
                });
            }
        }
    }
}
