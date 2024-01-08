using Agency.Models;

namespace Agency.Areas.Admin.ViewModels
{
    public class UpdateProductVM
    {
        public string Name { get; set; }
        public IFormFile? Photo { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public List<Category>? Categories { get; set; }
    }
}
