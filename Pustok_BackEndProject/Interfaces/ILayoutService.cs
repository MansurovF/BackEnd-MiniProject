using Pustok_BackEndProject.Models;

namespace Pustok_BackEndProject.Interfaces
{
    public interface ILayoutService
    {
        Task<IDictionary<string, string>> GetSettings();
        Task<IEnumerable<Category>> GetCategories();
    }
}
