using AIS_APP.Helpers;
using AIS_APP.Pages;
using AIS_APP.Services;

namespace AIS_APP
{
    public partial class App : Application
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;

        public App(ApiService apiService, IValidator validator)
        {
            InitializeComponent();

            // Force light theme
            UserAppTheme = AppTheme.Light;

            _apiService = apiService;
            _validator = validator;

            SetMainPage();
        }

        //protected override void OnAppLinkRequestReceived(Uri uri)
        //{
        //    base.OnAppLinkRequestReceived(uri);

        //    // Check the incoming URI and navigate accordingly
        //    if (uri.Scheme == "aisapp" && uri.Host == "login")
        //    {
        //        // Navigate to the appropriate page based on the link
        //        MainPage = new NavigationPage(new LoginPage(_apiService, _validator)); // Replace YourPage with the target page class
        //    }
        //}

        private void SetMainPage()
        {
            //var accessToken = Preferences.Get("accesstoken", string.Empty);

            //if (string.IsNullOrEmpty(accessToken))
            //{
            //    MainPage = new NavigationPage(new RegisterPage(_apiService, _validator));
            //    return;
            //}

            MainPage = new AppShell(_apiService, _validator);
        }
    }
}
