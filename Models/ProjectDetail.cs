namespace Personal.Models
{
    public class ProjectDetail : ProjectListing
    {
        public string FullDescription { get; set; }
        public string ProjectUrl { get; set; }
        public string GitHubUrl { get; set; }
        public string[] Features { get; set; }
        public string[] Screenshots { get; set; }
        public string Challenges { get; set; }
        public string Solutions { get; set; }
    }
}
