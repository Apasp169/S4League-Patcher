namespace S4LeaguePatcher.models;

/// <summary>
///     Represents download progress information for tracking file downloads.
/// </summary>
/// <remarks>
///     This class is used to provide status updates during the game installation process.
///     It contains information about the overall download progress as well as details
///     about the current file being downloaded.
/// </remarks>
public class DownloadProgressInfo
{
    /// <summary>
    ///     Gets or sets the index of the current file being downloaded.
    /// </summary>
    /// <remarks>
    ///     This value is 0-based, meaning the first file has an index of 0.
    /// </remarks>
    public int CurrentFileIndex { get; set; }

    /// <summary>
    ///     Gets or sets the total number of files to be downloaded.
    /// </summary>
    public int TotalFiles { get; set; }

    /// <summary>
    ///     Gets or sets the name of the current file being downloaded.
    /// </summary>
    /// <remarks>
    ///     This is typically the filename without the path, used for display purposes.
    /// </remarks>
    public string CurrentFileName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the overall download progress as a percentage (0-100).
    /// </summary>
    /// <remarks>
    ///     This value represents the total progress across all files, not just the current file.
    /// </remarks>
    public double Percentage { get; set; }
}