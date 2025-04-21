using System.Windows;
using S4LeaguePatcher.views;

namespace S4LeaguePatcher;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(bool showDownloadView = false)
    {
        InitializeComponent();

        if (showDownloadView)
            MainContent.Content = new DownloadView();
        else
            MainContent.Content = new LoginPlayView();
    }
}