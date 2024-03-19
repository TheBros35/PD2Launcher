using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using PD2Launcherv2.Enums;
using PD2Launcherv2.Helpers;
using PD2Launcherv2.Interfaces;
using PD2Launcherv2.Models;
using PD2Launcherv2.Models.ProjectDiablo2Launcherv2.Models;
using PD2Launcherv2.Services;
using PD2Launcherv2.Storage;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using static ProjectDiablo2Launcherv2.Constants;

namespace PD2Launcherv2.ViewModels
{
    public class FiltersViewModel : ViewModelBase
    {
        private readonly HttpService _httpService = new();
        private readonly ILocalStorage _localStorage;
        private AllSettings _allSettings;

        public RelayCommand AuthorCall { get; private set; }
        public RelayCommand FilterCall { get; private set; }

        public FiltersViewModel(ILocalStorage localStorage)
        {
            CloseCommand = new RelayCommand(CloseView);
            AuthorCall = new RelayCommand(AuthorCall_Click);
            FilterCall = new RelayCommand(FilterCall_Click);
            _localStorage = localStorage;
        }

        public void AuthorCall_Click()
        {
            Debug.WriteLine("start AuthorCall_Click");
            Debug.WriteLine("end AuthorCall_Click");

        }
        public void FilterCall_Click()
        {
            Debug.WriteLine("FilterCall_Click start");
            Debug.WriteLine("FilterCall_Click end");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CloseView()
        {
            // Sending a message to anyone who's listening for NavigationMessage
            Messenger.Default.Send(new NavigationMessage { Action = NavigationAction.GoBack });
        }
    }
}