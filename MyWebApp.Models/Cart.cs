namespace MyWebApp.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string ProductId { get; set; }
        public Product Product { get; set; }
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int Count { get; set; }
    }
}
