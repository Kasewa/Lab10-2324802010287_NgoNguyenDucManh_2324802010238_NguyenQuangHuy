using ASC.Web.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace ASC.Web.Services
{
    public class NavigationCacheOperations : INavigationCacheOperations
    {
        private readonly IMemoryCache _cache;
        private readonly IWebHostEnvironment _env;

        public NavigationCacheOperations(IMemoryCache cache, IWebHostEnvironment env)
        {
            _cache = cache;
            _env = env;
        }

        public async Task CreateNavigationCacheAsync()
        {
            var navigationJsonPath = Path.Combine(_env.ContentRootPath, "Navigation.json");
            var jsonContent = await File.ReadAllTextAsync(navigationJsonPath);
            var navigationMenu = JsonConvert.DeserializeObject<NavigationMenu>(jsonContent);
            _cache.Set("NavigationMenu", navigationMenu);
        }

        public List<NavigationMenuItem> GetNavigationMenu(string role)
        {
            var navigationMenu = _cache.Get<NavigationMenu>("NavigationMenu");
            if (navigationMenu == null) return new List<NavigationMenuItem>();

            return navigationMenu.MenuItems
                .Where(item => item.UserRoles.Contains(role))
                .OrderBy(item => item.Sequence)
                .ToList();
        }
    }
}
