using AIS_APP.Helpers;
using AIS_APP.Services;

namespace AIS_APP.Pages;

public partial class InfoPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator validator;

    public InfoPage(ApiService apiService, IValidator _validator)
	{
		InitializeComponent();

        _apiService = apiService;
        validator = _validator;
    }
}