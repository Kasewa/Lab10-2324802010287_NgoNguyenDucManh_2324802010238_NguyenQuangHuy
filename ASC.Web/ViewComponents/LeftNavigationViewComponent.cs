using ASC.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.ViewComponents
{
    public class LeftNavigationViewComponent : ViewComponent
    {
        private readonly INavigationCacheOperations _navigationCacheOperations;

        public LeftNavigationViewComponent(INavigationCacheOperations navigationCacheOperations)
        {
            _navigationCacheOperations = navigationCacheOperations;
        }

        public IViewComponentResult Invoke()
        {
            string role = "User";
            if (UserClaimsPrincipal.IsInRole("Admin"))
                role = "Admin";
            else if (UserClaimsPrincipal.IsInRole("Engineer"))
                role = "Engineer";

            var menuItems = _navigationCacheOperations.GetNavigationMenu(role);
            return View(menuItems);
        }
    }
}
