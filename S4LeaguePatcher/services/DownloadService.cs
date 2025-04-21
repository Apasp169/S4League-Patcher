using System.IO;
using System.Net.Http;
using S4LeaguePatcher.models;
using S4LeaguePatcher.utils;

namespace S4LeaguePatcher.services;

/// <summary>
///     Provides functionality for downloading game files with progress tracking and checksum validation.
/// </summary>
/// <remarks>
///     This service handles the downloading of game files from remote servers,
///     reports download progress, and validates file integrity using MD5 checksums.
/// </remarks>
public class DownloadService
{
    /// <summary>
    ///     HttpClient instance used for all download operations.
    /// </summary>
    private readonly HttpClient _httpClient = new();

    /// <summary>
    ///     Event that is triggered when download progress changes.
    /// </summary>
    /// <remarks>
    ///     Subscribers to this event receive updates about the current download progress,
    ///     including the current file, download percentage, and overall progress information.
    /// </remarks>
    public event Action<DownloadProgressInfo>? DownloadProgressChanged;

    /// <summary>
    ///     Downloads a file from the specified URL to the destination path with progress tracking and optional checksum
    ///     validation.
    /// </summary>
    /// <param name="url">The URL of the file to download.</param>
    /// <param name="destinationPath">The local file path where the downloaded file will be saved.</param>
    /// <param name="fileIndex">The index of the current file in the overall download queue (0-based).</param>
    /// <param name="totalFiles">The total number of files to be downloaded.</param>
    /// <param name="fileName">The name of the file being downloaded (for display purposes).</param>
    /// <param name="expectedMd5">Optional MD5 checksum for file validation after download.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous download operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    /// <exception cref="IOException">Thrown when an I/O error occurs during file writing.</exception>
    /// <exception cref="InvalidDataException">Thrown when the downloaded file fails checksum validation.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
    public async Task DownloadFileAsync(
        string url,
        string destinationPath,
        int fileIndex,
        int totalFiles,
        string fileName,
        string? expectedMd5 = null,
        CancellationToken cancellationToken = default)
    {
        long downloadedBytes = 0;

        using (var response =
               await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var totalBytes = response.Content.Headers.ContentLength ?? 1;

            await using (var fs = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
            await using (var stream = await response.Content.ReadAsStreamAsync(cancellationToken))
            {
                var buffer = new byte[8192];
                int read;
                while ((read = await stream.ReadAsync(buffer, cancellationToken)) > 0)
                {
                    await fs.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
                    downloadedBytes += read;

                    DownloadProgressChanged?.Invoke(new DownloadProgressInfo
                    {
                        CurrentFileIndex = fileIndex,
                        TotalFiles = totalFiles,
                        CurrentFileName = fileName,
                        Percentage = downloadedBytes * 100.0 / totalBytes
                    });

                    // Check for cancellation inside loop
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(expectedMd5) &&
            !FileUtils.ValidateChecksum(destinationPath, expectedMd5))
            throw new InvalidDataException($"Checksum mismatch for {fileName}");
    }

    /// <summary>
    ///     Manually reports download progress by invoking the DownloadProgressChanged event.
    /// </summary>
    /// <param name="info">The progress information to report.</param>
    /// <remarks>
    ///     This method can be used to report progress for operations that don't directly
    ///     involve downloading files but are part of the overall installation process.
    /// </remarks>
    public void ReportProgress(DownloadProgressInfo info)
    {
        DownloadProgressChanged?.Invoke(info);
    }
}