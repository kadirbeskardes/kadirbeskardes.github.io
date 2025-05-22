namespace Personal.Models
{
    public class ProjectListing
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string ImageUrl { get; set; }
        public string[] Technologies { get; set; }
        public DateTime CreatedDate { get; set; }
    }



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


    public class ContactModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
