using System.Net.Http;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using S4LeaguePatcher.constants;
using S4LeaguePatcher.models;
using S4LeaguePatcher.settings;

namespace S4LeaguePatcher.api;

public class UserApi
{
    private readonly HttpClient _httpClient = new();

    public async Task<LoginResponse?> LoginAsync(string userId, string password)
    {
        var loginModel = new LoginRequest
        {
            ServiceCode = "vfun",
            InputUserId = userId,
            InputUserPassword = password
        };

        var jsonContent = JsonConvert.SerializeObject(loginModel);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(Endpoints.UserLogin, content);
            var responseString = await response.Content.ReadAsStringAsync();

            var rawResponse = JsonConvert.DeserializeObject<dynamic>(responseString);

            if ((int)(rawResponse?.result ?? -1) == 1)
                return JsonConvert.DeserializeObject<LoginResponse>(responseString);

            MessageBox.Show("Login failed. Please check your credentials.", "Login Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Login failed:\n{ex.Message}", "Login Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }
    }

    public async Task<MemberInfo?> GetMemberInfoAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Endpoints.UserInfo);
            request.Headers.Add("Authorization", $"Bearer {Settings.Instance.AuthToken}");

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            var rawResponse = JsonConvert.DeserializeObject<dynamic>(responseString);
            if ((int)(rawResponse?.result ?? -1) != 1)
                return null;

            var dataJson = JsonConvert.SerializeObject(rawResponse!.data);
            return JsonConvert.DeserializeObject<MemberInfo>(dataJson);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to fetch member info:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }
    }

    public async Task<AuthCodeResponse?> GetAuthCodeAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Endpoints.MakeAuthCode);
            request.Headers.Add("Authorization", $"Bearer {Settings.Instance.AuthToken}");

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            var rawResponse = JsonConvert.DeserializeObject<dynamic>(responseString);
            if ((int)(rawResponse?.result ?? -1) != 1)
                return null;

            var dataJson = JsonConvert.SerializeObject(rawResponse!.data);
            return JsonConvert.DeserializeObject<AuthCodeResponse>(dataJson);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to fetch member info:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }
    }
}