using System.ComponentModel.DataAnnotations;

namespace Pustok_BackEndProject.Models
{
    public class Category:BaseEntity
    {
       
        [StringLength(255)]
        public string Name { get; set; }
        public bool IsMain { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Category Parent { get; set; }
        public IEnumerable<Category>Children { get; set; }
    }
}
