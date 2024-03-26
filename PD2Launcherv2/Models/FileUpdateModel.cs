namespace ProjectDiablo2Launcherv2.Models
{
    public class FileUpdateModel
    {
        public string Client { get; set; } = "https://storage.googleapis.com/storage/v1/b/pd2-client-files/o";
        public string FilePath { get; set; } = "Live";
        // Default to beta launcher for all updates for now
        // After s9 beta, we can use the prod launcher update URL
        public string Launcher { get; set; } = "https://storage.googleapis.com/storage/v1/b/pd2-beta-launcher-update/o";
        public string Other { get; set; } = "";
    }
}