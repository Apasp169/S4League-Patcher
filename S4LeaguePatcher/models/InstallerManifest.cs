using Newtonsoft.Json;

namespace S4LeaguePatcher.models;

/// <summary>
///     Represents the manifest file for the S4League installer.
///     Contains information about available files, versions, and download locations.
/// </summary>
/// <remarks>
///     This class maps to a JSON structure that defines the installer configuration,
///     including file list, download URL, and version information.
/// </remarks>
public class InstallerManifest
{
    /// <summary>
    ///     Gets or sets the version of the game client.
    /// </summary>
    /// <remarks>
    ///     Typically a numeric string (e.g., "10000") representing the client version.
    ///     Used to determine if updates are available.
    /// </remarks>
    [JsonProperty(nameof(Version))]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the base URL for downloading the installer files.
    /// </summary>
    /// <remarks>
    ///     The complete download URL for a file would be this URL plus the file name.
    ///     Property is serialized as "URL" in JSON.
    /// </remarks>
    [JsonProperty("URL")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the total number of files in the installer package.
    /// </summary>
    /// <remarks>
    ///     This should match the count of items in the FileList property.
    ///     Used for progress tracking during installation.
    /// </remarks>
    [JsonProperty(nameof(TotalCount))]
    public int TotalCount { get; set; }

    /// <summary>
    ///     Gets or sets the total size of all files in bytes.
    /// </summary>
    /// <remarks>
    ///     Represents the cumulative size of all files in the FileList.
    ///     Used for progress tracking and disk space verification.
    /// </remarks>
    [JsonProperty(nameof(TotalSize))]
    public long TotalSize { get; set; }

    /// <summary>
    ///     Gets or sets the list of files to be downloaded and installed.
    /// </summary>
    /// <remarks>
    ///     Contains details about each file, including name, size, and checksum.
    /// </remarks>
    [JsonProperty(nameof(FileList))]
    public List<InstallerFile> FileList { get; set; } = [];
}

/// <summary>
///     Represents an individual file entry in the installer manifest.
/// </summary>
/// <remarks>
///     Contains information needed to download, verify, and install a single file
///     as part of the S4League installation process.
/// </remarks>
public class InstallerFile
{
    /// <summary>
    ///     Gets or sets the name of the file.
    /// </summary>
    /// <remarks>
    ///     The filename is used both for download path construction and for
    ///     saving the file to the local file system.
    /// </remarks>
    [JsonProperty(nameof(Name))]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the size of the file in bytes.
    /// </summary>
    /// <remarks>
    ///     Used for download progress tracking and to verify the completeness
    ///     of the downloaded file.
    /// </remarks>
    [JsonProperty(nameof(Size))]
    public long Size { get; set; }

    /// <summary>
    ///     Gets or sets the MD5 checksum of the file.
    /// </summary>
    /// <remarks>
    ///     Used to verify file integrity after download.
    ///     The checksum is typically represented as a 32-character hexadecimal string.
    /// </remarks>
    [JsonProperty(nameof(CheckSum))]
    public string CheckSum { get; set; } = string.Empty;
}