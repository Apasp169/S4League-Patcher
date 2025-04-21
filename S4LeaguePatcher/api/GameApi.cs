using System.Net.Http;
using System.Windows;
using Newtonsoft.Json;
using S4LeaguePatcher.constants;
using S4LeaguePatcher.models;

namespace S4LeaguePatcher.api;

/// <summary>
///     Provides methods for interacting with the S4 League game API(VFUN).
/// </summary>
/// <remarks>
///     This class handles all HTTP communication with the game server's API endpoints,
///     including fetching installation manifests and other game-related data.
/// </remarks>
public class GameApi
{
    /// <summary>
    ///     HttpClient instance used for making API requests.
    /// </summary>
    /// <remarks>
    ///     A single HttpClient instance is used for all requests to optimize performance
    ///     and resource usage as recommended by Microsoft.
    /// </remarks>
    private readonly HttpClient _httpClient = new();

    /// <summary>
    ///     Fetches the installer manifest from the game server.
    /// </summary>
    /// <returns>
    ///     An <see cref="InstallerManifest" /> object containing information about game files
    ///     that need to be downloaded and installed, or null if the operation failed.
    /// </returns>
    /// <remarks>
    ///     The installer manifest contains information about all files required for installation,
    ///     including their URLs, checksums, and installation paths.
    ///     If an error occurs during the request, a message box is displayed to the user.
    /// </remarks>
    public async Task<InstallerManifest?> GetInstallerManifestAsync()
    {
        try
        {
            // Fetch the manifest JSON from the server and deserialize it into an InstallerManifest object
            var json = await _httpClient.GetStringAsync(Endpoints.GameDownloadManifest);
            return JsonConvert.DeserializeObject<InstallerManifest>(json);
        }
        catch (Exception ex)
        {
            // Display an error message to the user if the request fails
            MessageBox.Show(
                $"Failed to fetch game manifest:\n{ex.Message}",
                "Download Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            return null;
        }
    }

    public async Task<bool> GetMaintenanceCheckResponseAsync()
    {
        try
        {
            var json = await _httpClient.GetStringAsync(Endpoints.GameDownloadManifest);
            var responsePayload = JsonConvert.DeserializeObject<MaintenanceCheckResponse>(json);

            return responsePayload?.CanPlay ?? false;
        }
        catch (Exception e)
        {
            MessageBox.Show($"Failed to check maintenance:\n{e.Message}", "Maintenance Check Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }
}