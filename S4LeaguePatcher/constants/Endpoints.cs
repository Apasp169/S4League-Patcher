namespace S4LeaguePatcher.constants;

/// <summary>
///     Provides constant URL endpoints for API and resource access.
/// </summary>
/// <remarks>
///     This class contains all external API endpoints used by the application to communicate
///     with the game servers and content delivery networks (CDNs). If the server infrastructure
///     changes, only the constants in this class need to be updated.
/// </remarks>
public static class Endpoints
{
    /// <summary>
    ///     The URL for retrieving game information from the Valofe API.
    /// </summary>
    /// <remarks>
    ///     This endpoint returns general information about the game
    /// </remarks>
    public const string GameInfo = "https://external-api.valofe.com/api/library/gameinfo/s4league?lang=en";

    /// <summary>
    ///     The URL for retrieving the game download manifest.
    /// </summary>
    /// <remarks>
    ///     This endpoint returns a JSON file containing the list of all game files that need to be
    ///     downloaded during installation, including their paths, sizes, and checksums.
    ///     This manifest is used by the downloader to verify file integrity and determine which
    ///     files need to be downloaded.
    /// </remarks>
    public const string GameDownloadManifest =
        "http://s4-cdn.valofe.com/s4league_on_valofe/live/fullclient/game_downloader/file.json";

    public const string UserLogin = "https://external-api.valofe.com/api/vfun/login";

    public const string UserInfo = "https://external-api.valofe.com/api/vfun/member_info";

    public const string MakeAuthCode = "https://external-api.valofe.com/api/vfun/make_auth_code";

    public const string CheckMaintenance =
        "https://api.valofe.com/v1/vlauncher/check_maintenance?service_code=s4league";
}