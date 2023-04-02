using Pustok_BackEndProject.Models;
using Pustok_BackEndProject.ViewModels.BasketViewModels;
using Pustok_BackEndProject.ViewModels.WishListViewModels;

namespace Pustok_BackEndProject.Interfaces
{
    public interface ILayoutService
    {
        Task<IDictionary<string, string>> GetSettings();
        Task<IEnumerable<Category>> GetCategories();
        Task<List<BasketVM>> GetBasket();
        Task<List<WishlistVM>> GetWish();
    }
}
