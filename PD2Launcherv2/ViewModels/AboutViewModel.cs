using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PD2Launcherv2.Enums;
using PD2Launcherv2.Helpers;
using PD2Launcherv2.Interfaces;
using System.Diagnostics;

namespace PD2Launcherv2.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public string cloudFileBucket { get; set; }
        public string folderPath { get; set; }
        private readonly ILocalStorage _localStorage;
        public RelayCommand ProdBucket { get; private set; }
        public RelayCommand BetaBucket { get; private set; }
        public RelayCommand UpdateFilesCall { get; private set; }

        public AboutViewModel(ILocalStorage localStorage)
        {
            CloseCommand = new RelayCommand(CloseView);
            _localStorage = localStorage;
            ProdBucket = new RelayCommand(ProdBucketAssign);
            BetaBucket = new RelayCommand(BetaBucketAssign);
            UpdateFilesCall = new RelayCommand(UpdateFilesCheck);
        }

        public void ProdBucketAssign()
        {
            Debug.WriteLine("\nstart ProdBucketAssign");
             folderPath = "E:\\Games\\Blizzard\\d2\\Diablo II\\ProjectD2";
            cloudFileBucket = "https://storage.googleapis.com/storage/v1/b/pd2-client-files/o";
            Debug.WriteLine("end ProdBucketAssign\n");
        }
        public void BetaBucketAssign()
        {
            Debug.WriteLine("\nstart BetaBucketAssign");
            folderPath = "E:\\Games\\Blizzard\\d2\\Diablo II\\ProjectD2Beta";
            cloudFileBucket = "https://storage.googleapis.com/storage/v1/b/pd2-beta-client-files/o";
            Debug.WriteLine("end BetaBucketAssign \n");
        }
        public void UpdateFilesCheck()
        {
            Debug.WriteLine("\nstart BetaBucketAssign");
            Debug.WriteLine("end BetaBucketAssign \n");
        }


        private void CloseView()
        {
            // Sending a message to anyone who's listening for NavigationMessage
            Messenger.Default.Send(new NavigationMessage { Action = NavigationAction.GoBack });
        }
    }
}