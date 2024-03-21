
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PD2Launcherv2.Enums;
using PD2Launcherv2.Helpers;
using PD2Launcherv2.Views;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace PD2Launcherv2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ICommand OpenOptionsCommand { get; private set; }
        public ICommand OpenLootCommand { get; private set; }
        public ICommand OpenDonateCommand { get; private set; }
        public ICommand OpenAboutCommand { get; private set; }
        public ICommand OpenHomeCommand { get; set; }
        public ICommand OpenTradeCommand { get; set; }
        public ICommand OpenRedditCommand { get; set; }
        public ICommand OpenTwitterCommand { get; set; }
        public ICommand OpenDiscordCommand { get; set; }
        public ICommand OpenWikiCommand { get; set; }


        public MainWindow()
        {
            InitializeComponent();

            OpenOptionsCommand = new RelayCommand(ShowOptionsView);
            OpenLootCommand = new RelayCommand(ShowLootView);
            OpenAboutCommand = new RelayCommand(ShowAboutView);
            //OpenHomeCommand = new RelayCommand(() => OpenUrlInBrowser("https://www.projectdiablo2.com"));
            //OpenTradeCommand = new RelayCommand(() => OpenUrlInBrowser("https://www.projectdiablo2.com/market"));
            //OpenRedditCommand = new RelayCommand(() => OpenUrlInBrowser("https://www.reddit.com/r/ProjectDiablo2/"));
            //OpenTwitterCommand = new RelayCommand(() => OpenUrlInBrowser("https://twitter.com/projectdiablo2"));
            //OpenDiscordCommand = new RelayCommand(() => OpenUrlInBrowser("https://discord.gg/RgX4MWu"));
            //OpenWikiCommand = new RelayCommand(() => OpenUrlInBrowser("https://projectdiablo2.miraheze.org/wiki/Main_Page"));


            // Registering to receive NavigationMessage
            Messenger.Default.Register<NavigationMessage>(this, OnNavigationMessageReceived);


            DataContext = this;
        }
        private void OnNavigationMessageReceived(NavigationMessage message)
        {
            Overlay.Visibility = Visibility.Collapsed;
            // Handle the message
            if (message.Action == NavigationAction.GoBack)
            {
                // Assuming MainFrame is your Frame control
                if (MainFrame.CanGoBack)
                {
                    MainFrame.GoBack();
                }
                else
                {
                    // If no navigation history, just clear the content of the frame.
                    MainFrame.Content = null;
                }
            }
        }

        private void ClearNavigationStack()
        {
            while (MainFrame.CanGoBack)
            {
                MainFrame.RemoveBackEntry();
            }
        }

        private void BackgroundImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("PlayButton_Click start");
            Debug.WriteLine("PlayButton_Click end");
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OptionsButton_Click start");
            ShowOptionsView();
            Debug.WriteLine("OptionsButton_Click end");
        }

        private void ShowOptionsView()
        {
            ClearNavigationStack();
            Overlay.Visibility = Visibility.Visible;
            MainFrame.Navigate(new OptionsView());
        }

        private void ShowLootView()
        {
            ClearNavigationStack();
            Overlay.Visibility = Visibility.Visible;
            MainFrame.Navigate(new FiltersView());
        }

        private void ShowAboutView()
        {
            ClearNavigationStack();
            Overlay.Visibility = Visibility.Visible;
            MainFrame.Navigate(new AboutView());
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // Use the ProcessStartInfo class to open the link in the default browser
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });

            // Prevent the default behavior of opening the link
            e.Handled = true;
        }



        private void DonateButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("DonateButton_Click start");
            Debug.WriteLine("DonateButton_Click end");
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("CloseButton_Click start");
            this.Close();
            Debug.WriteLine("CloseButton_Click end");
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("MinimizeButton_Click start");
            this.WindowState = WindowState.Minimized;
            Debug.WriteLine("MinimizeButton_Click end");
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.Tag is string url)
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }
    }
}