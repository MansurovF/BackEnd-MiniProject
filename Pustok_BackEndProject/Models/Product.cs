using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pustok_BackEndProject.Models
{
    public class Product:BaseEntity
    {
        [StringLength(255)]
        public string Title { get; set; }
        [Column(TypeName ="money")]
        public double Price { get; set; }
        [Column(TypeName = "money")]
        public double DiscountedPrice { get; set; }
        [Column(TypeName = "money")]
        public int Count { get; set; }
        public double Extax { get; set; }
        [StringLength(5)]
        public string? Seria { get; set; }
        [StringLength(255)]
        public int? Code { get; set; }
        [StringLength(1000)]
        public string? MainDescription { get; set; }
        [StringLength(1000)]
        public string? SecondDescription { get; set; }
        [StringLength(255)]
        public string? MainImage { get; set; }
        [StringLength(255)]
        public string? HoverImage { get; set; }
        public bool IsNewArrival { get; set; }
        public bool IsMostviewProducts { get; set; }
        public bool IsFeatured { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category{ get; set; }
        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }

        public int? AuthorId { get; set; }
        public Author? Author { get; set; }
        public List<ProductTag>? ProductTags { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
        public IEnumerable<Basket>? Baskets { get; set; }
        public List<Review>? Reviews { get; set; }

        [NotMapped]
        public IFormFile? MainFile { get; set; }
        [NotMapped]
        public IFormFile? HoverFile { get; set; }
        [NotMapped]
        public IEnumerable<IFormFile>? Files { get; set; }
        [NotMapped]
        public IEnumerable<int>? TagIds { get; set; }
    }
}
