using AIS_APP.Helpers;
using AIS_APP.Models;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AIS_APP.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = AppConfig.Url;
        private readonly ILogger<ApiService> _logger;
        JsonSerializerOptions _serializerOptions;

        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        // Ver os retornos da API do Marcorácio

        #region GET Methods

        public async Task<(bool IdNumberOnFlight, string? ErrorMessage)> CheckIdNumberOnFlight(string idNum, int flightId)
        {
            return await GetAsync<bool>($"Api/Tickets/CheckIdentificationNumberOnFlight?idNum={idNum}&flightId={flightId}");
        }

        public async Task<(List<FlightModel>? AvailableFlights, string? ErrorMessage)> GetAvailableFlights()
        {
            return await GetAsync<List<FlightModel>>("Api/Flights/GetAvailableFlights");
        }

        public async Task<(List<TicketRecord>? UserFlightHistory, string? ErrorMessage)> GetUserFlightHistory()
        {
            return await GetAsync<List<TicketRecord>>("Api/Flights/GetUserFlightHistory");
        }

        public async Task<(List<TicketRecord>? UserScheduledFlights, string? ErrorMessage)> GetUserScheduledFlights()
        {
            return await GetAsync<List<TicketRecord>>("Api/Flights/GetUserScheduledFlights");
        }

        public async Task<(FlightModel? FlightById, string? ErrorMessage)> GetFlightById(int id)
        {
            string endpoint = $"Api/Flights/GetFlightById?id={id}";

            return await GetAsync<FlightModel>(endpoint);
        }

        public async Task<(ProfileImage? ProfileImage, string? ErrorMessage)> GetUserImage()
        {
            string endpoint = $"Api/Users/GetUserImage";

            return await GetAsync<ProfileImage>(endpoint);
        }

        public async Task<(UserModel? ProfileImage, string? ErrorMessage)> GetUserInfo()
        {
            string endpoint = $"Api/Users/GetUserInfo";

            return await GetAsync<UserModel>(endpoint);
        }

        #endregion

        #region POST Methods

        public async Task<ApiResponse<bool>> PurchaseTicket(PurchaseTicketModel ticket)
        {
            try
            {
                var json = JsonSerializer.Serialize(ticket, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("Api/Tickets/PurchaseTicket", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = response.StatusCode == HttpStatusCode.Unauthorized
                        ? "Unauthorized"
                        : $"Error in HTTP request: {response.StatusCode}";

                    _logger.LogError($"Error in HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool> { ErrorMessage = errorMessage };
                }
                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing order: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> UploadUserImage(byte[] imageArray)
        {
            try
            {
                var content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(imageArray), "imageFile", "image.jpg");
                var response = await PostRequest("Api/Users/UploadUserImage", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = response.StatusCode == HttpStatusCode.Unauthorized
                      ? "Unauthorized"
                      : $"Error in HTTP request: {response.StatusCode}";

                    _logger.LogError($"Error in HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool> { ErrorMessage = errorMessage };
                }
                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading user image: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> Register(RegisterModel register)
        {
            try
            {
                var json = JsonSerializer.Serialize(register, _serializerOptions);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("Api/Users/Register", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error in HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Error in HTTP request: {response.StatusCode}"
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering user: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> Login(LoginModel login)
        {
            try
            {
                var json = JsonSerializer.Serialize(login, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("Api/Users/Login", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error in HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Error in HTTP request: {response.StatusCode}"
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);

                Preferences.Set("accesstoken", result!.AccessToken);
                Preferences.Set("userid", result.UserId!);
                Preferences.Set("email", result.UserEmail);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error logging user: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> UpdatePassword(UpdatePasswordModel model)
        {
            try
            {
                var json = JsonSerializer.Serialize(model, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("Api/Users/UpdatePassword", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = response.StatusCode == HttpStatusCode.Unauthorized
                        ? "Unauthorized"
                        : $"Error in HTTP request: {response.StatusCode}";

                    _logger.LogError($"Error in HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool> { ErrorMessage = errorMessage };
                }
                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating password: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> ResetPassword(ResetPasswordModel model)
        {
            try
            {
                var json = JsonSerializer.Serialize(model, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("Api/Users/ResetPassword", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = response.StatusCode == HttpStatusCode.Unauthorized
                        ? "Unauthorized"
                        : $"Error in HTTP request: {response.StatusCode}";

                    _logger.LogError($"Error in HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool> { ErrorMessage = errorMessage };
                }
                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error resetting password: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> ForgotPassword(ForgotPasswordModel model)
        {
            try
            {
                var json = JsonSerializer.Serialize(model, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("Api/Users/ForgotPassword", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = response.StatusCode == HttpStatusCode.Unauthorized
                        ? "Unauthorized"
                        : $"Error in HTTP request: {response.StatusCode}";

                    _logger.LogError($"Error in HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool> { ErrorMessage = errorMessage };
                }
                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in password recovery: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }


        #endregion

        #region PUT Methods

        public async Task<ApiResponse<bool>> UpdateUser(UpdateUserModel model)
        {
            try
            {
                var json = JsonSerializer.Serialize(model, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PutRequest("Api/Users/UpdateUser", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = response.StatusCode == HttpStatusCode.Unauthorized
                        ? "Unauthorized"
                        : $"Error in HTTP request: {response.StatusCode}";

                    _logger.LogError($"Error in HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool> { ErrorMessage = errorMessage };
                }
                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in user update: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        #endregion

        #region API Endpoint Communication Methods

        private void AddAuthorizationHeader()
        {
            var token = Preferences.Get("accesstoken", string.Empty);

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task<(T? Data, string? ErroMessage)> GetAsync<T>(string endpoint)
        {
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.GetAsync(AppConfig.Url + endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);
                    return (data ?? Activator.CreateInstance<T>(), null);
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (default, errorMessage);
                    }

                    string generalErrorMessage = $"Request error: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return (default, generalErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"Error in HTTP request: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (JsonException ex)
            {
                string errorMessage = $"Error in JSON deserialization: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Unexpected error: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
        }

        private async Task<HttpResponseMessage> PostRequest(string uri, HttpContent content)
        {
            var url = _baseUrl + uri;
            try
            {
                AddAuthorizationHeader();
                var result = await _httpClient.PostAsync(url, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in POST request to {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        private async Task<HttpResponseMessage> PutRequest(string uri, HttpContent content)
        {
            var url = AppConfig.Url + uri;
            try
            {
                AddAuthorizationHeader();
                var result = await _httpClient.PutAsync(url, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PUT request to {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        #endregion
    }
}
