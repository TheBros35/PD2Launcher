namespace ProjectDiablo2Launcherv2.Models
{
    public class FileUpdateModel
    {
        public string Client { get; set; } = "https://storage.googleapis.com/storage/v1/b/pd2-client-files/o";
        public string FilePath { get; set; } = "null for now";
        public string Launcher { get; set; } = "null for now";
        public string Other { get; set; } = "null for now";
    }
}