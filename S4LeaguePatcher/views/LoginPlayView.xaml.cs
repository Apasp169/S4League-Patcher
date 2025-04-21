using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using S4LeaguePatcher.constants;
using S4LeaguePatcher.services;
using S4LeaguePatcher.settings;

namespace S4LeaguePatcher.views;

public partial class LoginPlayView : UserControl
{
    private readonly GameService _gameService = new();
    private readonly Settings _settings = Settings.Instance;
    private readonly UserService _userService = new();

    public LoginPlayView()
    {
        InitializeComponent();

        PlayButton.IsEnabled = false;

        UsernameTextBox.TextChanged += LoginFields_Changed;
        PasswordBox.PasswordChanged += LoginFields_Changed;

        // Map then enter key on the password box to the play button
        PasswordBox.KeyDown += (s, e) =>
        {
            if (e.Key == Key.Enter) PlayButton_Click(s, e);
        };


        LoadSavedCredentials();
    }


    private void LoginFields_Changed(object sender, RoutedEventArgs e)
    {
        // Enable play button only if both username and password have values
        PlayButton.IsEnabled = !string.IsNullOrWhiteSpace(UsernameTextBox.Text) &&
                               !string.IsNullOrWhiteSpace(PasswordBox.Password);
    }

    private void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        UsernameTextBox.IsEnabled = false;
        PasswordBox.IsEnabled = false;
        PlayButton.IsEnabled = false;
        AttemptLogin();
    }

    private void LoadSavedCredentials()
    {
        if (!string.IsNullOrEmpty(_settings.Username))
            UsernameTextBox.Text = _settings.Username;

        if (!string.IsNullOrEmpty(_settings.Password))
            PasswordBox.Password = "********"; // Mask password for security
    }

    private void LoginFailed()
    {
        UsernameTextBox.IsEnabled = true;
        PasswordBox.IsEnabled = true;
        PlayButton.IsEnabled = true;
    }

    private void AttemptLogin()
    {
        var username = UsernameTextBox.Text;
        var password = PasswordBox.Password;

        if (password == "********" && !string.IsNullOrEmpty(_settings.Password))
            password = _settings.Password;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MessageBox.Show("Please enter both username and password.", "Login Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        _userService.LoginAsync(username, password).ContinueWith(task =>
        {
            if (task.Result)
            {
                Dispatcher.Invoke(() =>
                {
                    StatusText.Text = "Login successful! Starting game...";
                    StartGame();
                });
                return;
            }

            // Login failed
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show("Login failed. Please check your credentials.", "Login Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                LoginFailed();
            });
        });
    }

    private async void StartGame()
    {
        try
        {
            if (await _gameService.IsInMaintenanceModeAsync())
                MessageBox.Show("The game is currently in maintenance mode. Please try again later.",
                    "Maintenance Mode", MessageBoxButton.OK, MessageBoxImage.Warning);

            var authCode = await _userService.GetAuthCodeAsync();
            if (string.IsNullOrEmpty(authCode))
            {
                StatusText.Text = "Failed to retrieve authentication code.";
                LoginFailed();
            }

            // TODO: Start the game with the auth code and we need to pass the language code for now we just force it to english
            var languageCode = "eng";
            var gameParams = $"fromVLauncher::VALOFE:{authCode}:{languageCode}";

            // Start standalone process
            var gamePath = _settings.GameInstallPath;
            var gameProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = $"{gamePath}\\{PathAndNames.ClientExecutableName}",
                    Arguments = gameParams,
                    UseShellExecute = true,
                    CreateNoWindow = false
                }
            };

            gameProcess.Start();
            Application.Current.Shutdown();
        }
        catch (Exception e)
        {
            MessageBox.Show($"Failed to start the game:\n{e.Message}", "Game Start Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}