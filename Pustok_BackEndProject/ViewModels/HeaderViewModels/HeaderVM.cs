using Pustok_BackEndProject.Models;
using Pustok_BackEndProject.ViewModels.BasketViewModels;

namespace Pustok_BackEndProject.ViewModels.HeaderViewModels
{
    public class HeaderVM
    {
        public List<BasketVM> BasketVMs { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
        public IDictionary<string, string>? Settings { get; set; }
        
    }
}
