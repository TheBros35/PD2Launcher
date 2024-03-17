
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PD2Launcherv2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
            Debug.WriteLine("OptionsButton_Click end");
        }

        private void LootButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("LootButton_Click start");
            Debug.WriteLine("LootButton_Click end");
        }

        private void DonateButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("DonateButton_Click start");
            Debug.WriteLine("DonateButton_Click end");
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("AboutButton_Click start");
            Debug.WriteLine("AboutButton_Click end");
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