using PD2Launcherv2.Models.ProjectDiablo2Launcherv2.Models;
using ProjectDiablo2Launcherv2.Models;

namespace PD2Launcherv2.Models
{
    public class AllSettings
    {
        public DdrawOptions DdrawOptions { get; set; }
        public FileUpdateModel FileUpdateModel { get; set; }
        public FilterStorage FilterStorage { get; set; }
        public LauncherArgs LauncherArgs { get; set; }
        public News News { get; set; }
    }
}