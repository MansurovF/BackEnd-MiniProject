using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel.DataAnnotations;

namespace Pustok_BackEndProject.Models
{
    public class Slider : BaseEntity
    {
        [StringLength(255)]
        public string Image { get; set; }
        [StringLength(255)]

        public string Title { get; set; }
        [StringLength(1000)]
        public string description { get; set; }

        [StringLength(255)]
        public string Link { get; set; }
    }
}
