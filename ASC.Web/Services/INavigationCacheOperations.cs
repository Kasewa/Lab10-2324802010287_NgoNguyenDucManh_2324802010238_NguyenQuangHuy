using ASC.Web.Models;

namespace ASC.Web.Services
{
    public interface INavigationCacheOperations
    {
        Task CreateNavigationCacheAsync();
        List<NavigationMenuItem> GetNavigationMenu(string role);
    }
}
