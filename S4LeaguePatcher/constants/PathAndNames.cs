namespace S4LeaguePatcher.constants;

/// <summary>
///     Provides constant values for file paths and names used throughout the application.
/// </summary>
/// <remarks>
///     This class centralizes all filesystem-related constants to ensure consistency
///     and to simplify future updates to file paths or names. If the game structure
///     changes, only the constants in this class need to be modified.
/// </remarks>
public static class PathAndNames
{
    /// <summary>
    ///     The filename of the main game client executable.
    /// </summary>
    /// <remarks>
    ///     This is the primary executable that is launched when starting the game.
    ///     The patcher uses this to locate the game installation and to launch the game
    ///     after updates are complete.
    /// </remarks>
    public const string ClientExecutableName = "Client_Release.exe";

    /// <summary>
    ///     The filename of the version information file.
    /// </summary>
    /// <remarks>
    ///     This INI file contains version information about the installed game.
    ///     The patcher compares this version against the server version to determine
    ///     if an update is needed.
    /// </remarks>
    public const string VersionFileName = "s4league.ini";

    /// <summary>
    ///     The default installation path for the game.
    /// </summary>
    /// <remarks>
    ///     This path is used as the default location for new installations.
    ///     Users can override this path during the installation process.
    /// </remarks>
    public const string DefaultGameInstallPath = @"C:\VFUN\s4league";
}