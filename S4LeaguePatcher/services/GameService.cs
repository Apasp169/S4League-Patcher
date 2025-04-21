using System.Diagnostics;
using System.IO;
using System.Windows;
using S4LeaguePatcher.api;
using S4LeaguePatcher.models;
using S4LeaguePatcher.settings;
using S4LeaguePatcher.utils;

namespace S4LeaguePatcher.services;

/// <summary>
///     Provides high-level game installation and management functionality.
/// </summary>
/// <remarks>
///     This service coordinates the game installation process by fetching the manifest,
///     downloading required files, and handling the installation process.
///     It acts as a facade for the <see cref="DownloadService" /> and <see cref="GameApi" />
///     to provide a simplified interface for game installation.
/// </remarks>
public class GameService
{
    /// <summary>
    ///     Service responsible for downloading files.
    /// </summary>
    private readonly DownloadService _downloadService = new();

    /// <summary>
    ///     API client for interacting with the game server.
    /// </summary>
    private readonly GameApi _gameApi = new();

    /// <summary>
    ///     Event that is triggered when download progress changes.
    /// </summary>
    /// <remarks>
    ///     This event forwards progress updates from the underlying <see cref="DownloadService" />.
    ///     Subscribers receive updates about the current download progress, including the current file,
    ///     download percentage, and overall progress information.
    /// </remarks>
    public event Action<DownloadProgressInfo>? DownloadProgressChanged
    {
        add => _downloadService.DownloadProgressChanged += value;
        remove => _downloadService.DownloadProgressChanged -= value;
    }

    /// <summary>
    ///     Downloads and installs the game by fetching the manifest, downloading required files,
    ///     and initiating the installation process.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous installation operation.</returns>
    /// <remarks>
    ///     This method performs the following steps:
    ///     1. Fetches the game manifest from the server
    ///     2. Creates a temporary directory for downloaded files
    ///     3. Downloads all required game files to the temporary directory
    ///     4. Launches the installer executable after all files are downloaded
    ///     5. Prompts the user to select the game installation directory
    ///     6. Saves the selected directory to application settings
    ///     If the user cancels the path selection, the application will exit.
    /// </remarks>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
    public async Task DownloadAndInstallGameAsync(CancellationToken cancellationToken = default)
    {
        // Get the manifest from the server
        var manifest = await _gameApi.GetInstallerManifestAsync();
        if (manifest == null)
            return;

        // Prepare temporary folder for downloads
        var tempFolder = Path.Combine(Path.GetTempPath(), "S4LeagueInstaller");
        Directory.CreateDirectory(tempFolder);

        // Download all files in the manifest
        for (var i = 0; i < manifest.FileList.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var file = manifest.FileList[i];
            var url = manifest.Url + file.Name;
            var destPath = Path.Combine(tempFolder, file.Name);

            // Check if file exists and matches checksum
            if (File.Exists(destPath) && !string.IsNullOrWhiteSpace(file.CheckSum) &&
                FileUtils.ValidateChecksum(destPath, file.CheckSum))
                // Simulate download complete to update UI
                _downloadService.ReportProgress(new DownloadProgressInfo
                {
                    CurrentFileIndex = i + 1,
                    TotalFiles = manifest.FileList.Count,
                    CurrentFileName = file.Name,
                    Percentage = 100.0
                });
            else
                await _downloadService.DownloadFileAsync(
                    url, destPath, i + 1, manifest.FileList.Count, file.Name, file.CheckSum, cancellationToken);
        }

        // Find the installer executable
        var exeFile = manifest.FileList
            .FirstOrDefault(f => f.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase));

        if (exeFile != null)
        {
            var exePath = Path.Combine(tempFolder, exeFile.Name);

            // Launch the installer
            var installerProcess = Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = true
            });

            if (installerProcess != null)
                await installerProcess.WaitForExitAsync(cancellationToken);
            else
                // fallback delay
                await Task.Delay(5000, cancellationToken);

            // Show message box to inform user that he should select the game path
            MessageBox.Show("Please select the game install path.",
                "Notice", MessageBoxButton.OK, MessageBoxImage.Information);

            // Ask user for install path
            if (FileUtils.TryAskUserForGamePath(out var selectedPath))
            {
                Settings.Instance.GameInstallPath = selectedPath;
                await Settings.Instance.SaveAsync();
            }
            else
            {
                MessageBox.Show("Game path was not selected.",
                    "Notice", MessageBoxButton.OK, MessageBoxImage.Information);

                // Close the application if no path was selected, the user will be prompted again with the next start
                Application.Current.Shutdown();
            }
        }
    }

    public async Task<bool> IsInMaintenanceModeAsync()
    {
        return await _gameApi.GetMaintenanceCheckResponseAsync();
    }
}