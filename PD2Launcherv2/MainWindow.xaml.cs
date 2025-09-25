﻿using CommunityToolkit.Mvvm.Input;
using GalaSoft.MvvmLight.Messaging;
using PD2Launcherv2.Enums;
using PD2Launcherv2.Helpers;
using PD2Shared.Helpers;
using PD2Shared.Interfaces;
using PD2Launcherv2.Messages;
using PD2Shared.Models;
using PD2Launcherv2.Views;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.IO;

namespace PD2Launcherv2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>.
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly ILocalStorage _localStorage;
        private readonly FileUpdateHelpers _fileUpdateHelpers;
        private readonly FilterHelpers _filterHelpers;
        private readonly LaunchGameHelpers _launchGameHelpers;
        private readonly NewsHelpers _newsHelpers;
        private readonly DDrawHelpers _dDrawHelpers;
        private readonly GameFileUpdateHelpers _gameFileUpdater;
        private bool _isBeta;
        public bool IsBeta
        {
            get => _isBeta;
            set
            {
                if (_isBeta != value)
                {
                    _isBeta = value;
                    Debug.WriteLine($"IsBeta changing to.: {value}");
                    BetaVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                    OnPropertyChanged(nameof(IsBeta));
                }
            }
        }
        private Visibility _betaVisibility = Visibility.Collapsed;
        public Visibility BetaVisibility
        {
            get => _betaVisibility;
            set
            {
                if (_betaVisibility != value)
                {
                    _betaVisibility = value;
                    OnPropertyChanged(nameof(BetaVisibility));
                }
            }
        }

        private bool _isCustom;
        public bool IsCustom
        {
            get => _isCustom;
            set
            {
                if (_isCustom != value)
                {
                    _isCustom = value;
                    Debug.WriteLine($"IsCustom changing to: {value}");
                    CustomVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                    OnPropertyChanged(nameof(IsCustom));
                }
            }
        }

        private Visibility _customVisibility = Visibility.Collapsed;
        public Visibility CustomVisibility
        {
            get => _customVisibility;
            set
            {
                if (_customVisibility != value)
                {
                    _customVisibility = value;
                    OnPropertyChanged(nameof(CustomVisibility));
                }
            }
        }

        private Visibility _updatesNotificationVisibility = Visibility.Collapsed;
        public Visibility UpdatesNotificationVisibility
        {
            get => _updatesNotificationVisibility;
            set
            {
                if (_updatesNotificationVisibility != value)
                {
                    _updatesNotificationVisibility = value;
                    OnPropertyChanged(nameof(UpdatesNotificationVisibility));
                }
            }
        }

        public List<NewsItem> NewsItems { get; set; }
        public bool IsDisableUpdates { get; private set; }
        public ICommand OpenOptionsCommand { get; private set; }
        public ICommand OpenLootCommand { get; private set; }
        public ICommand OpenAboutCommand { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            OpenOptionsCommand = new RelayCommand(ShowOptionsView);
            OpenLootCommand = new RelayCommand(ShowLootView);
            OpenAboutCommand = new RelayCommand(ShowAboutView);
            _dDrawHelpers = new DDrawHelpers();
            _localStorage = (ILocalStorage)App.ServiceProvider.GetService(typeof(ILocalStorage));
            InitializeDefaultSettings(_localStorage);
            _fileUpdateHelpers = (FileUpdateHelpers)App.ServiceProvider.GetService(typeof(FileUpdateHelpers));
            _filterHelpers = (FilterHelpers)App.ServiceProvider.GetService(typeof(FilterHelpers));
            _launchGameHelpers = (LaunchGameHelpers)App.ServiceProvider.GetService(typeof(LaunchGameHelpers));
            _newsHelpers = (NewsHelpers)App.ServiceProvider.GetService(typeof(NewsHelpers));
            _gameFileUpdater = (GameFileUpdateHelpers)App.ServiceProvider.GetService(typeof(GameFileUpdateHelpers));
            LoadAndUpdateDDrawOptions();
            InitWindow();
            EnsureWindowIsVisible();
            Loaded += MainWindow_Loaded;
            LoadConfiguration();

            // Registering to receive NavigationMessage
            Messenger.Default.Register<NavigationMessage>(this, OnNavigationMessageReceived);
            Messenger.Default.Register<ConfigurationChangeMessage>(this, OnConfigurationChanged);
            DataContext = this;

            this.Closed += MainWindow_Closed;

            // Load or setup default file update model
            FileUpdateModel storeUpdate = _localStorage.LoadSection<FileUpdateModel>(StorageKey.FileUpdateModel) ?? new FileUpdateModel
            {
                Client = "https://pd2-client-files.projectdiablo2.com/",
                Launcher = "https://storage.googleapis.com/storage/v1/b/pd2-launcher-update/o",
                FilePath = "Live"
            };

            //s11 hotfix
            if (storeUpdate.Client == "https://storage.googleapis.com/storage/v1/b/pd2-client-files/o")
            {
                storeUpdate.Client = "https://pd2-client-files.projectdiablo2.com/";
                _localStorage.Update(StorageKey.FileUpdateModel, storeUpdate);
            }


            // Don't try to update launcher in debug mode
            // TEST
            #if DEBUG
            #else
                CheckForUpdates();
            #endif
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
                    // If no navigation history, just clear the content of the frame,,
                    MainFrame.Content = null;
                }
            }
        }

        // trigger the update, in an async event handler
        private async void CheckForUpdates()
        {
            UpdateUIForOperationStart(); // Prepare the UI for the update operation

            // Initialize the progress handler to update progress bar
            var progressHandler = new Progress<double>(value =>
            {
                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = value * 100;
                    if (DownloadProgressBar.Visibility != Visibility.Visible)
                    {
                        DownloadProgressBar.Visibility = Visibility.Visible;
                    }
                });
            });

            // Define the completion action to reset UI after update
            Action onDownloadComplete = ResetUI;

            // Trigger the update check and download process
            await UpdateLauncherCheck(_localStorage, progressHandler, onDownloadComplete);
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

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("PlayButton_Click start");
            UpdateUIForOperationStart();

            try
            {
                if (Process.GetProcessesByName("Game").Any())
                {
                    MessageBox.Show("Game is already running.");
                    return;
                }

                var selectedAuthorAndFilter = _localStorage.LoadSection<SelectedAuthorAndFilter>(StorageKey.SelectedAuthorAndFilter);
                if (selectedAuthorAndFilter?.selectedFilter != null)
                {
                    bool isUpdated = await _filterHelpers.CheckAndUpdateFilterAsync(selectedAuthorAndFilter);
                }

                LauncherArgs launcherArgs = _localStorage.LoadSection<LauncherArgs>(StorageKey.LauncherArgs);
                if (!launcherArgs.disableAutoUpdate)
                {
                    try
                    {
                        await _gameFileUpdater.UpdateFromShaMetadataAsync(_localStorage, new Progress<double>(UpdateProgress), () => { });
                        Debug.WriteLine("made it out of the update check");
                        await _fileUpdateHelpers.SyncFilesFromEnvToRoot(_localStorage);
                    }
                    catch (HttpRequestException ex)
                    {
                        Debug.WriteLine($"Update failed: {ex.Message}. Proceeding in offline mode.");
                        MessageBox.Show("Could not check for updates. Proceeding in offline mode.", "Offline Mode", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                _launchGameHelpers.LaunchGame(_localStorage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred during PlayButton_Click: {ex.Message}");
                ShowErrorMessage($"An error occurred: {ex.Message}");
            }
            finally
            {
                // Reset UI to the default "Play" state regardless of the operation outcome
                ResetUI();
                Debug.WriteLine("PlayButton_Click end");
            }
        }

        private void UpdateUIForOperationStart()
        {
            try
            {
                var updatingImageUri = new Uri("pack://application:,,,/Resources/Images/updating_disabled.jpg");
                PlayButton.NormalImageSource = new BitmapImage(updatingImageUri);
                DownloadProgressBar.Visibility = Visibility.Visible;
                DownloadProgressBar.Value = 0;
            }
            catch (UriFormatException ex)
            {
                Debug.WriteLine($"URI format exception: {ex.Message}");
            }
        }

        private void ResetUI()
        {
            // Code to reset the Play button and hide the progress bar
            Dispatcher.Invoke(() =>
            {
                try
                {
                    var playImageUri = new Uri("pack://application:,,,/Resources/Images/play.jpg");
                    PlayButton.NormalImageSource = new BitmapImage(playImageUri);
                }
                catch (UriFormatException ex)
                {
                    Debug.WriteLine($"URI format exception: {ex.Message}");
                }
                DownloadProgressBar.Visibility = Visibility.Hidden;
            });
        }

        private void UpdateProgress(double value)
        {
            Dispatcher.Invoke(() =>
            {
                DownloadProgressBar.Value = value * 100;
            });
        }

        private void onDownloadComplete()
        {
            Dispatcher.Invoke(() =>
            {
                // Actions to take when the download is complete, before resetting UI
                _launchGameHelpers.LaunchGame(_localStorage);
            });
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
            try
            {
                Process.Start(new ProcessStartInfo("https://www.projectdiablo2.com/donate") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow_Closed(sender, e);
            this.Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.Tag is string url)
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }

        private void LoadConfiguration()
        {
            var fileUpdateModel = _localStorage.LoadSection<FileUpdateModel>(StorageKey.FileUpdateModel);
            var launcherArgs = _localStorage.LoadSection<LauncherArgs>(StorageKey.LauncherArgs);
            IsBeta = fileUpdateModel?.FilePath == "Beta";
            IsCustom = fileUpdateModel?.FilePath == "Custom";
            IsDisableUpdates = launcherArgs?.disableAutoUpdate == true;

            // Use property to control visibility
            UpdatesNotificationVisibility = IsDisableUpdates ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnConfigurationChanged(ConfigurationChangeMessage message)
        {
            IsBeta = message.IsBeta;
            OnPropertyChanged(nameof(IsBeta));
            IsCustom = message.IsCustom;
            OnPropertyChanged(nameof(IsCustom));
            // Use property to control visibility
            UpdatesNotificationVisibility = message.IsDisableUpdates ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            // Fetch and store the latest news from the repository
            await _newsHelpers.FetchAndStoreNewsAsync(_localStorage);
            // Fetch and store the latest reset info from the repository
            await _newsHelpers.FetchResetInfoAsync(_localStorage);

            // Load the stored news
            News theNews = _localStorage.LoadSection<News>(StorageKey.News);
            List<NewsItem> newsItems = theNews?.news ?? new List<NewsItem>();

            // Check and append reset news item if the reset time is in the future
            AppendResetNewsItemIfApplicable(newsItems);

            // Set the modified list as the item source for the UI
            NewsListBox.ItemsSource = newsItems;
        }

        private void LoadAndUpdateDDrawOptions()
        {
            // Read the current settings from ddraw.ini
            DdrawOptions currentDdrawOptions = _dDrawHelpers.ReadDdrawOptions();

            // Update the local storage with the current ddraw
            _localStorage.Update(StorageKey.DdrawOptions, currentDdrawOptions);
        }

        private void NewsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is NewsItem selectedItem)
            {
                var uri = selectedItem.Link;
                if (!string.IsNullOrWhiteSpace(uri))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        // If there's an error opening the link, show an error message
                        ShowErrorMessage($"Failed to open the link: {ex.Message}\nPlease check your internet connection or try again later.");
                        Debug.WriteLine($"Failed to open link: {ex.Message}");
                    }
                }
                // If the link is null or empty, do nothing

                ((ListBox)sender).SelectedItem = null;
            }
        }

        private void CenterWindowOnScreen()
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            this.Left = (screenWidth - this.Width) / 2;
            this.Top = (screenHeight - this.Height) / 2;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            var windowPosition = new WindowPositionModel
            {
                Left = this.Left,
                Top = this.Top,
            };

            Debug.WriteLine($"\n\n Saving window position: Left = {this.Left}, Top = {this.Top} \n\n");

            _localStorage.Update(StorageKey.WindowPosition, windowPosition);
        }

        private void EnsureWindowIsVisible()
        {
            var windowPosition = _localStorage.LoadSection<WindowPositionModel>(StorageKey.WindowPosition);

            // Check if the window is out of bounds
            bool isOutOfBounds =
                windowPosition.Left < SystemParameters.VirtualScreenLeft ||
                windowPosition.Top < SystemParameters.VirtualScreenTop ||
                windowPosition.Left > SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth ||
                windowPosition.Top > SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight;

            if (windowPosition == null || isOutOfBounds)
            {
                CenterWindowOnScreen();
            }
            else
            {
                // Restore the window to its last saved position
                this.Left = windowPosition.Left;
                this.Top = windowPosition.Top;
            }
        }

        private void InitWindow()
        {
            var windowPosition = _localStorage.LoadSection<WindowPositionModel>(StorageKey.WindowPosition);

            Debug.WriteLine($"\n\n Loaded window position: Left = {windowPosition?.Left}, Top = {windowPosition?.Top} \n\n");

            if (windowPosition == null || (windowPosition.Left == 0 && windowPosition.Top == 0))
            {
                CenterWindowOnScreen();
            }
            else
            {
                this.Left = windowPosition.Left;
                this.Top = windowPosition.Top;
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void InitializeDefaultSettings(ILocalStorage localStorage)
        {
            _localStorage.InitializeIfNotExists(StorageKey.FileUpdateModel, new FileUpdateModel());
            _localStorage.InitializeIfNotExists(StorageKey.DdrawOptions, new DdrawOptions());
            _localStorage.InitializeIfNotExists(StorageKey.LauncherArgs, new LauncherArgs());
            _localStorage.InitializeIfNotExists(StorageKey.SelectedAuthorAndFilter, new SelectedAuthorAndFilter());
            _localStorage.InitializeIfNotExists(StorageKey.Pd2AuthorList, new Pd2AuthorList());
            _localStorage.InitializeIfNotExists(StorageKey.News, new News());
            _localStorage.InitializeIfNotExists(StorageKey.WindowPosition, new WindowPositionModel());
            _localStorage.InitializeIfNotExists(StorageKey.ResetInfo, new ResetInfo());

            Debug.WriteLine("Default settings initialized if missing.");
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task FetchAndHandleResetInfoAsync()
        {
            await _newsHelpers.FetchResetInfoAsync(_localStorage);
        }

        private void AppendResetNewsItemIfApplicable(List<NewsItem> newsItems)
        {
            var resetInfo = _localStorage.LoadSection<ResetInfo>(StorageKey.ResetInfo);
            if (resetInfo != null && resetInfo.ResetData != null)
            {
                var resetTimeUtc = resetInfo.ResetData.ResetTime;
                // Check if the reset time is in the future
                if (resetTimeUtc > DateTime.UtcNow)
                {
                    // Convert UTC reset time to local time
                    var resetTimeLocal = resetTimeUtc.ToLocalTime();

                    // Format the local reset time
                    string formattedLocalResetTime = resetTimeLocal.ToString("MMMM dd 'at' hh:mm tt", CultureInfo.InvariantCulture);

                    // Append or insert the local reset time into the summary
                    string updatedSummary = $"{resetInfo.ResetData.ResetSummary} {formattedLocalResetTime}).";

                    var resetNewsItem = new NewsItem
                    {
                        Date = resetTimeUtc.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture),
                        Title = resetInfo.ResetData.ResetTitle,
                        Summary = updatedSummary,
                        Content = resetInfo.ResetData.ResetContent ?? "Check out the details for the upcoming season reset.",
                        Link = resetInfo.ResetData.ResetLink
                    };

                    // Prepend the reset news item to the list
                    newsItems.Insert(0, resetNewsItem);
                }
            }
        }

        public async Task UpdateLauncherCheck(ILocalStorage _localStorage, IProgress<double> progress, Action onDownloadComplete)
        {
            if (IsDisableUpdates)
            {
                onDownloadComplete?.Invoke();
                return;
            }
            var fileUpdateModel = _localStorage.LoadSection<FileUpdateModel>(StorageKey.FileUpdateModel);
            var installPath = Directory.GetCurrentDirectory();

            if (fileUpdateModel == null)
            {
                return;
            }

            // Initialise it as empty to satisfy dependencies
            var cloudFileItems = new List<CloudFileItem>();

            try
            {
                cloudFileItems = await _fileUpdateHelpers.GetCloudFileMetadataAsync(fileUpdateModel.Launcher);
                if (cloudFileItems == null || cloudFileItems.Count == 0)
                {
                    onDownloadComplete?.Invoke();
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unhandled exception: {ex}");
                UpdatesNotificationVisibility = Visibility.Visible;
                onDownloadComplete?.Invoke();
                return;
            }

            var bigFour = new List<string> { "PD2Launcher.exe", "PD2Shared.dll", "SteamPD2.exe", "UpdateUtility.exe" };
            bool big4NeedsUpdate = false;

            foreach (var fileName in bigFour)
            {
                var cloudItem = cloudFileItems.FirstOrDefault(i => i.Name == fileName);
                if (cloudItem == null)
                {
                    continue;
                }

                var localPath = Path.Combine(installPath, fileName);
                if (!File.Exists(localPath))
                {
                    big4NeedsUpdate = true;
                    break;
                }

                if (!_fileUpdateHelpers.CompareCRC(localPath, cloudItem.Crc32c))
                {
                    big4NeedsUpdate = true;
                    break;
                }
            }

            if (!big4NeedsUpdate)
            {
                //check and update all other cloud files
                foreach (var cloudItem in cloudFileItems)
                {
                    if (bigFour.Contains(cloudItem.Name)) continue;
                    if (_fileUpdateHelpers.IsFileExcluded(cloudItem.Name)) continue;

                    string localPath = Path.Combine(installPath, cloudItem.Name);
                    if (!File.Exists(localPath) || !_fileUpdateHelpers.CompareCRC(localPath, cloudItem.Crc32c))
                    {
                        bool downloaded = await _fileUpdateHelpers.PrepareLauncherUpdateAsync(cloudItem.MediaLink, localPath, progress);
                        if (!downloaded)
                        {
                            MessageBox.Show($"Failed to download {cloudItem.Name}.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                }

                onDownloadComplete?.Invoke();
                return;
            }

            foreach (var fileName in bigFour)
            {
                var cloudItem = cloudFileItems.FirstOrDefault(i => i.Name == fileName);
                if (cloudItem == null)
                {
                    continue;
                }

                string targetName = (fileName == "UpdateUtility.exe") ? fileName : "Temp" + fileName;
                string path = Path.Combine(installPath, targetName);

                bool downloaded = await _fileUpdateHelpers.PrepareLauncherUpdateAsync(cloudItem.MediaLink, path, progress);
                if (!downloaded)
                {
                    MessageBox.Show($"Failed to download {fileName}.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            foreach (var cloudItem in cloudFileItems)
            {
                if (bigFour.Contains(cloudItem.Name)) continue;
                if (_fileUpdateHelpers.IsFileExcluded(cloudItem.Name))
                {
                    continue;
                }

                string localPath = Path.Combine(installPath, cloudItem.Name);
                if (!File.Exists(localPath))
                {
                }
                else if (_fileUpdateHelpers.CompareCRC(localPath, cloudItem.Crc32c))
                {
                    continue;
                }
                else
                {
                }

                bool downloaded = await _fileUpdateHelpers.PrepareLauncherUpdateAsync(cloudItem.MediaLink, localPath, progress);
                if (!downloaded)
                {
                    MessageBox.Show($"Failed to download {cloudItem.Name}.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // Wait till downloaded and flushed
            var tempFilesToCheck = new List<string> { "PD2Launcher.exe", "PD2Shared.dll", "SteamPD2.exe" };

            foreach (var fileName in tempFilesToCheck)
            {
                string tempPath = Path.Combine(installPath, "Temp" + fileName);
                int retries = 0;
                const int maxRetries = 10;

                while ((!File.Exists(tempPath) || new FileInfo(tempPath).Length < 32768) && retries++ < maxRetries)
                {
                    await Task.Delay(300);
                }
            }

            ShowTopmostMessageBox("Launcher update is ready. The app will now close and update...", "Update Ready");
            _fileUpdateHelpers.StartUpdateProcess();

            await Task.Delay(250);
            Process.GetCurrentProcess().Kill();
        }

        public static void ShowTopmostMessageBox(string message, string title)
        {
            var topmostWindow = new Window
            {
                Width = 0,
                Height = 0,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                Topmost = true,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = -1000,
                Top = -1000
            };

            topmostWindow.Loaded += (s, e) =>
            {
                topmostWindow.Hide();
                MessageBox.Show(topmostWindow, message, title, MessageBoxButton.OK, MessageBoxImage.Information);
                topmostWindow.Close();
            };

            topmostWindow.Show();
        }
    }
}