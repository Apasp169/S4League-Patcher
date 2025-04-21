using System.Windows;
using S4LeaguePatcher.api;
using S4LeaguePatcher.models;
using S4LeaguePatcher.settings;

namespace S4LeaguePatcher.services;

public class UserService
{
    private readonly Settings _settings = Settings.Instance;
    private readonly UserApi _userApi = new();


    public async Task<bool> LoginAsync(string username, string password)
    {
        // Check if we are already logged in and have a valid token
        if (IsLoggedIn() && !string.IsNullOrEmpty(_settings.Username) && username == _settings.Username) return true;


        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MessageBox.Show("Username or password cannot be empty.", "Login Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        _settings.Username = username;
        _settings.Password = password;


        var loginResponse = await _userApi.LoginAsync(username, password);

        if (loginResponse == null)
        {
            await Logout();
            return false;
        }

        _settings.AuthToken = loginResponse.Data.ValofeWebToken;
        _settings.AuthTokenExpiry = loginResponse.Data.ExpiredAt;

        await _settings.SaveAsync();
        return true;
    }

    public bool IsLoggedIn()
    {
        return !string.IsNullOrEmpty(_settings.AuthToken) &&
               _settings.AuthTokenExpiry > DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public async Task<MemberInfo?> GetMemberInfoAsync()
    {
        if (!IsLoggedIn())
        {
            MessageBox.Show("You are not logged in.", "Login Required",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return null;
        }

        var memberInfo = await _userApi.GetMemberInfoAsync();
        if (memberInfo == null)
            MessageBox.Show("Failed to fetch member info. Please try again.", "Member Info Error",
                MessageBoxButton.OK, MessageBoxImage.Error);

        return memberInfo;
    }

    public async Task Logout()
    {
        try
        {
            _settings.Username = string.Empty;
            _settings.Password = string.Empty;
            _settings.AuthToken = string.Empty;
            _settings.AuthTokenExpiry = 0;
            await _settings.SaveAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show("Failed to logout. Please try again.", "Logout Error",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    public async Task<string?> GetAuthCodeAsync()
    {
        if (!IsLoggedIn())
        {
            MessageBox.Show("You are not logged in.", "Login Required",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return null;
        }

        var authCodeResponse = await _userApi.GetAuthCodeAsync();
        if (authCodeResponse == null)
            MessageBox.Show("Failed to fetch auth code. Please try again.", "Auth Code Error",
                MessageBoxButton.OK, MessageBoxImage.Error);

        return authCodeResponse!.AuthCode;
    }
}