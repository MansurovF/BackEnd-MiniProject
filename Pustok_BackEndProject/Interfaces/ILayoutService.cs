using Pustok_BackEndProject.Models;
using Pustok_BackEndProject.ViewModels.BasketViewModels;

namespace Pustok_BackEndProject.Interfaces
{
    public interface ILayoutService
    {
        Task<IDictionary<string, string>> GetSettings();
        Task<IEnumerable<Category>> GetCategories();
        Task<List<BasketVM>> GetBasket();

    }
}
