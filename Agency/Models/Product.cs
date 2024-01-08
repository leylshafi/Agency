using Agency.Models.Base;

namespace Agency.Models
{
    public class Product:BaseEntity
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
