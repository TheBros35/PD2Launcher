
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PD2Launcherv2.Enums;
using PD2Launcherv2.Helpers;
using PD2Launcherv2.Views;
using System.Diagnostics;
using System.Windows;
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

        public MainWindow()
        {
            InitializeComponent();

            OpenOptionsCommand = new RelayCommand(ShowOptionsView);
            OpenLootCommand = new RelayCommand(ShowLootView);
            OpenAboutCommand = new RelayCommand(ShowAboutView);

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
            Overlay.Visibility = Visibility.Visible;
            MainFrame.Content = new OptionsView();
        }

        private void ShowLootView()
        {
            Overlay.Visibility = Visibility.Visible;
            MainFrame.Content = new FiltersView();
        }

        private void ShowAboutView()
        {
            Overlay.Visibility = Visibility.Visible;
            MainFrame.Content = new AboutView();
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

    }
}