using PD2Launcherv2.ViewModels;
using System.Windows.Controls;

namespace PD2Launcherv2.Views
{
    /// <summary>
    /// Interaction logic for FiltersView.xaml
    /// </summary>
    public partial class FiltersView : Page
    {
        public FiltersView()
        {
            InitializeComponent();
            DataContext = App.Resolve<FiltersViewModel>();
        }
    }
}
