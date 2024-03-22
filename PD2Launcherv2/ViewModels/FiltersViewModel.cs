using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PD2Launcherv2.Enums;
using PD2Launcherv2.Helpers;
using PD2Launcherv2.Interfaces;
using PD2Launcherv2.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace PD2Launcherv2.ViewModels
{
    public class FiltersViewModel : ViewModelBase
    {
        private readonly FilterHelpers _filterHelpers;
        private readonly ILocalStorage _localStorage;

        private List<FilterAuthor> _authorsList;
        public List<FilterAuthor> AuthorsList
        {
            get => _authorsList;
            set
            {
                if (_authorsList != value)
                {
                    _authorsList = value;
                    OnPropertyChanged();
                }
            }
        }

        private FilterAuthor _selectedAuthor;
        public FilterAuthor SelectedAuthor
        {
            get => _selectedAuthor;
            set
            {
                if (_selectedAuthor != value)
                {
                    _selectedAuthor = value;
                    OnPropertyChanged();
                    FetchDataFromAuthorUrl(value.Url);
                }
            }
        }

        private List<FilterFile> _filtersList;
        public List<FilterFile> FiltersList
        {
            get => _filtersList;
            set
            {
                _filtersList = value;
                OnPropertyChanged();
            }
        }
        private FilterFile _selectedFilter;
        public FilterFile SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (_selectedFilter != value)
                {
                    _selectedFilter = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _changeProperty;
        public string ChangeProperty
        {
            get => _changeProperty;
            set
            {
                if (_changeProperty != value)
                {
                    _changeProperty = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand AuthorCall { get; private set; }
        public RelayCommand FilterCall { get; private set; }

        public FiltersViewModel(ILocalStorage localStorage)
        {
            CloseCommand = new RelayCommand(CloseView);
            _localStorage = localStorage;
            _filterHelpers = new FilterHelpers(new HttpClient(), _localStorage);
            AuthorCall = new RelayCommand(async () => await AuthorCall_Click());
            FilterCall = new RelayCommand(FilterCall_Click);
        }

        public async Task InitializeAsync()
        {
            Debug.WriteLine("Initializing FiltersViewModel...");
            await FetchAndStoreFilterAuthorsAsync();
        }

        private async void FetchDataFromAuthorUrl(string url)
        {
            // Use HttpClient to fetch data from the specified URL
            Debug.WriteLine($"Fetching data from {url}");
            _filterHelpers.FetchFilterContentsAsync(url);
            var filterContents = await _filterHelpers.FetchFilterContentsAsync(url);
            if (filterContents != null)
            {
                // Filter to include only .filter files and README.md, if necessary
                var validFiles = filterContents.Where(f => f.Name.EndsWith(".filter") || f.Name == "README.md").ToList();
                FiltersList = validFiles;
            }
        }

        private async Task FetchAndStoreFilterAuthorsAsync()
        {
            Debug.WriteLine("start FetchAndStoreFilterAuthorsAsync");
            await _filterHelpers.FetchAndStoreFilterAuthorsAsync();
            LoadAuthorsFromStorage();
            Debug.WriteLine("end FetchAndStoreFilterAuthorsAsync");
        }

        private void LoadAuthorsFromStorage()
        {
            // Load the Pd2AuthorList which contains ETag and the actual AuthorList
            var storedData = _localStorage.LoadSection<Pd2AuthorList>(StorageKey.Pd2AuthorList);
            if (storedData?.StorageAuthorList != null)
            {
                AuthorsList = storedData.StorageAuthorList;
            }
        }

        public async Task AuthorCall_Click()
        {
            Debug.WriteLine("start AuthorCall_Click");
            await FetchAndStoreFilterAuthorsAsync();
            Debug.WriteLine("end AuthorCall_Click");
        }

        public void FilterCall_Click()
        {
            Debug.WriteLine("FilterCall_Click start");
            Debug.WriteLine("FilterCall_Click end");
        }

        private void CloseView()
        {
            Messenger.Default.Send(new NavigationMessage { Action = NavigationAction.GoBack });
        }
    }
}