namespace MyWebApp.Models.ViewModels
{
    public class ProductVM
    {
        public Product product { get; set; } = new Product();
        public IEnumerable<Product> products { get; set; } = new List<Product>();
    }
}
