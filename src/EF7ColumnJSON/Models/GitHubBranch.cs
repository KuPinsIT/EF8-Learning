namespace EF8Example.Models
{
    public class GitHubBranch
    {
        public string Name { get; set; }
        public GitHubCommit Commit { get; set; }
        public bool Protected { get; set; }
    }

    public class GitHubCommit
    {
        public string Sha { get; set; }
        public string Url { get; set; }
    }
}
