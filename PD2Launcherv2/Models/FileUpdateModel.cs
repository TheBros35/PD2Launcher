namespace ProjectDiablo2Launcherv2.Models
{
    public class FileUpdateModel
    {
        public string client { get; set; } = "https://storage.googleapis.com/storage/v1/b/pd2-client-files/o";
        public string launcher { get; set; } = "null for now";
        public string other { get; set; } = "null for now";
    }
}
