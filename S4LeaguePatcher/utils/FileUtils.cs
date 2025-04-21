using System.IO;
using System.Security.Cryptography;
using Microsoft.Win32;
using S4LeaguePatcher.constants;

namespace S4LeaguePatcher.utils;

/// <summary>
///     Provides utility methods for file operations.
/// </summary>
public static class FileUtils
{
    /// <summary>
    ///     Validates if a file matches the expected MD5 checksum.
    /// </summary>
    /// <param name="filePath">The path to the file to validate.</param>
    /// <param name="expectedMd5">The expected MD5 checksum as a hexadecimal string.</param>
    /// <returns>
    ///     <c>true</c> if the computed MD5 checksum of the file matches the expected checksum;
    ///     otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    ///     The comparison is case-insensitive, meaning that checksums like "1A2B3C" and "1a2b3c"
    ///     are considered equal.
    /// </remarks>
    /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
    /// <exception cref="IOException">Thrown when an I/O error occurs while opening the file.</exception>
    /// <exception cref="UnauthorizedAccessException">
    ///     Thrown when the caller does not have the required permission to access
    ///     the file.
    /// </exception>
    public static bool ValidateChecksum(string filePath, string expectedMd5)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);
        var hashBytes = md5.ComputeHash(stream);
        var actual = Convert.ToHexString(hashBytes).ToUpperInvariant();
        return actual.Equals(expectedMd5, StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    ///     Prompts the user to select the S4League game executable.
    /// </summary>
    /// <param name="installPath">Returns the selected path if successful.</param>
    /// <returns><c>true</c> if a path was selected; otherwise <c>false</c>.</returns>
    public static bool TryAskUserForGamePath(out string installPath)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select S4League Game Executable",
            Filter = $"S4Client Executable | {PathAndNames.ClientExecutableName}",
            CheckFileExists = true
        };

        // Prefill with default path if valid
        if (Directory.Exists(PathAndNames.DefaultGameInstallPath))
        {
            var defaultExePath = Path.Combine(PathAndNames.DefaultGameInstallPath, PathAndNames.ClientExecutableName);
            if (File.Exists(defaultExePath))
            {
                dialog.InitialDirectory = PathAndNames.DefaultGameInstallPath;
                dialog.FileName = PathAndNames.ClientExecutableName;
            }
        }

        if (dialog.ShowDialog() == true)
        {
            installPath = Path.GetDirectoryName(dialog.FileName)!;
            return true;
        }

        installPath = string.Empty;
        return false;
    }
}