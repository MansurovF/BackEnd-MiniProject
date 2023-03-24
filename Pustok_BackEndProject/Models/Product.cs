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
        public IEnumerable<ProductTag>? ProductTags { get; set; }
        public IEnumerable<ProductImage>? ProductImages { get; set; }
    }
}
