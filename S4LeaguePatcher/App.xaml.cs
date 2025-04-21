using System.Windows;
using S4LeaguePatcher.settings;
using S4LeaguePatcher.utils;

namespace S4LeaguePatcher;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
/// <remarks>
///     The main application class that handles startup logic, including:
///     - Loading application settings
///     - Checking for a valid game installation path
///     - Prompting the user to download the game or select an existing installation
///     - Launching the appropriate window based on the user's choice
/// </remarks>
public partial class App : Application
{
    /// <summary>
    ///     Handles application startup logic.
    /// </summary>
    /// <param name="e">Startup event arguments.</param>
    /// <remarks>
    ///     This method performs the following operations:
    ///     1. Loads user settings from persistent storage
    ///     2. Checks if a valid game installation path exists
    ///     3. If no path is set, prompts the user with options:
    ///     - Download the game (opens MainWindow in download mode)
    ///     - Select an existing installation folder
    ///     - Cancel (exits the application)
    ///     4. If a valid path exists or is selected, opens the MainWindow normally
    ///     The method is marked as async because it loads settings asynchronously
    ///     and may need to save them after user interaction.
    /// </remarks>
    protected override async void OnStartup(StartupEventArgs e)
    {
        try
        {
            base.OnStartup(e);

            // Load settings
            await Settings.Instance.LoadAsync();

            if (string.IsNullOrWhiteSpace(Settings.Instance.GameInstallPath))
            {
                var result = MessageBox.Show(
                    "Game install path is not set.\n\nDo you want to download the game or select an existing folder?",
                    "Game Not Found",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question,
                    MessageBoxResult.Cancel);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        // Show download view in main window
                        var downloadWindow = new MainWindow(true);
                        downloadWindow.Show();
                        return;

                    case MessageBoxResult.No:
                        if (FileUtils.TryAskUserForGamePath(out var selectedPath))
                        {
                            Settings.Instance.GameInstallPath = selectedPath;
                            await Settings.Instance.SaveAsync();
                        }
                        else
                        {
                            MessageBox.Show("No install directory selected. The application will now close.");
                            Shutdown();
                            return;
                        }

                        break;
                    case MessageBoxResult.None:
                    case MessageBoxResult.OK:
                    case MessageBoxResult.Cancel:
                    default:
                        Shutdown();
                        return;
                }
            }

            // Show main window normally
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
        catch (Exception error)
        {
            // Show error message and close the application
            MessageBox.Show(
                $"An error occurred during startup:\n{error.Message}",
                "Startup Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }
}