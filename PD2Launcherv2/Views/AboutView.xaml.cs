using PD2Launcherv2.ViewModels;
using System.Windows.Controls;

namespace PD2Launcherv2.Views
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : Page
    {
        public AboutView()
        {
            InitializeComponent();
            DataContext = App.Resolve<AboutViewModel>();
        }
    }
}