namespace ProjectDiablo2Launcherv2.Models
{
    public class AuthorAndFilter

    {
        public FilterAuthor PreviousSelectedAuthor { get; set; } = new();

        public FilterFile PreviousSelectedFilter { get; set; } = new();
    }
}
