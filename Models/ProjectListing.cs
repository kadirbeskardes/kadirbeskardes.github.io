namespace Personal.Models
{
    public class ProjectListing
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string ImageUrl { get; set; }
        public string[] Technologies { get; set; }
        public string Category {  get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
