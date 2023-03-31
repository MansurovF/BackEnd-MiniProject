using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pustok_BackEndProject.Models
{
    public class Author:BaseEntity
    {
        [StringLength(255)]
        public string? Name { get; set; }
        [StringLength(255)]

        public string? Surname { get; set; }
        [NotMapped]
        public string? FullName => $"{Name} {Surname}";
        public IEnumerable<Product>? Products { get; set; }
    }
}
