using Pustok_BackEndProject.Models;
using Pustok_BackEndProject.ViewModels.BasketViewModels;
using Pustok_BackEndProject.ViewModels.WishListViewModels;

namespace Pustok_BackEndProject.ViewModels.HeaderViewModels
{
    public class HeaderVM
    {
        public List<BasketVM> BasketVMs { get; set; }
        public List<WishlistVM> WishlistVMs { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
        public IDictionary<string, string>? Settings { get; set; }
        
    }
}
