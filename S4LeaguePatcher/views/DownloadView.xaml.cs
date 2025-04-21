using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using S4LeaguePatcher.models;
using S4LeaguePatcher.services;

namespace S4LeaguePatcher.views;

public partial class DownloadView : UserControl
{
    private readonly GameService _gameService = new();
    private readonly BackgroundWorker _worker = new();
    private int _completedFiles;
    private CancellationTokenSource _cts = new();
    private int _totalFiles;

    public DownloadView()
    {
        InitializeComponent();
        _gameService.DownloadProgressChanged += OnDownloadProgressChanged;

        _worker.WorkerReportsProgress = true;
        _worker.WorkerSupportsCancellation = true;
        _worker.DoWork += Worker_DoWork!;
        _worker.RunWorkerCompleted += Worker_RunWorkerCompleted!;

        StartDownload();
    }

    private void StartDownload()
    {
        if (_worker.IsBusy) return;
        StatusText.Text = "Starting download...";
        _worker.RunWorkerAsync();
    }

    private void Worker_DoWork(object sender, DoWorkEventArgs e)
    {
        try
        {
            _gameService.DownloadAndInstallGameAsync(_cts.Token).GetAwaiter().GetResult();
        }
        catch (OperationCanceledException)
        {
            e.Cancel = true;
        }
        catch (Exception ex)
        {
            e.Result = ex;
        }
    }

    private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        _cts.Dispose();
        _cts = new CancellationTokenSource();

        CancelButton.IsEnabled = true;

        if (e.Error != null)
        {
            StatusText.Text = $"Error: {e.Error.Message}";
            return;
        }

        if (e.Cancelled)
        {
            StatusText.Text = "Download cancelled";
            return;
        }

        if (e.Result is Exception exception)
        {
            StatusText.Text = $"Error: {exception.Message}";
            return;
        }

        StatusText.Text = "Download completed successfully!";
        CurrentFileProgressBar.Value = 100;
        TotalProgressBar.Value = 100;
        CurrentFilePercentageText.Text = "100%";
        TotalProgressText.Text = "100%";
        CancelButton.Content = "Close";
    }

    private void OnDownloadProgressChanged(DownloadProgressInfo progressInfo)
    {
        Dispatcher.Invoke(() =>
        {
            if (_totalFiles != progressInfo.TotalFiles)
            {
                _totalFiles = progressInfo.TotalFiles;
                TotalFilesText.Text = _totalFiles.ToString();
            }

            CurrentFileIndexText.Text = $"File {progressInfo.CurrentFileIndex}";
            CurrentFileNameText.Text = progressInfo.CurrentFileName;

            var percentage = progressInfo.Percentage;
            CurrentFileProgressBar.Value = percentage;
            CurrentFilePercentageText.Text = $"{percentage:0}%";

            if (percentage >= 99.9 && progressInfo.CurrentFileIndex > _completedFiles)
                _completedFiles = progressInfo.CurrentFileIndex;

            var totalProgress = (_completedFiles * 100.0 + percentage) / _totalFiles;
            TotalProgressBar.Value = totalProgress;
            TotalProgressText.Text = $"{totalProgress:0}%";

            StatusText.Text = $"Downloading {progressInfo.CurrentFileName}...";
        });
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (_worker is { IsBusy: true, CancellationPending: false })
        {
            StatusText.Text = "Cancelling download...";
            _cts.Cancel();
            _worker.CancelAsync();
            CancelButton.IsEnabled = false;
        }
        else
        {
            Window.GetWindow(this)?.Close();
        }
    }
}