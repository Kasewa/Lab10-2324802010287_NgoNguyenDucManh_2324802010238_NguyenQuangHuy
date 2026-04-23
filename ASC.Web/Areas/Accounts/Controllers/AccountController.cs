using ASC.Utilities;
using ASC.Web.Areas.Accounts.Models;
using ASC.Web.Models;
using ASC.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASC.Web.Areas.Accounts.Controllers
{
    [Authorize]
    [Area("Accounts")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, IEmailSender emailSender, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ServiceEngineers()
        {
            var serviceEngineers = await _userManager.GetUsersInRoleAsync("Engineer"); // Ensure the role name matches the database
            HttpContext.Session.SetObjectAsJson("ServiceEngineers", serviceEngineers);

            return View(new ServiceEngineerViewModel
            {
                ServiceEngineers = serviceEngineers.ToList(),
                ServiceEngineerRegistration = new ServiceEngineerRegistrationViewModel { IsEdit = false }
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        // Change parameter name to 'viewModel' to avoid Model Binding conflicts
        public async Task<IActionResult> ServiceEngineers(ServiceEngineerViewModel viewModel)
        {
            viewModel.ServiceEngineers = HttpContext.Session.GetObjectFromJson<List<IdentityUser>>("ServiceEngineers") ?? new List<IdentityUser>();

            if (viewModel.ServiceEngineerRegistration != null && viewModel.ServiceEngineerRegistration.IsEdit)
            {
                ModelState.Remove("ServiceEngineerRegistration.Password");
                ModelState.Remove("ServiceEngineerRegistration.ConfirmPassword");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Add a null check for ServiceEngineerRegistration to prevent CS8602
            if (viewModel.ServiceEngineerRegistration != null && viewModel.ServiceEngineerRegistration.IsEdit)
            {
                // Update User 
                var user = await _userManager.FindByEmailAsync(viewModel.ServiceEngineerRegistration.Email);
                if (user == null) return NotFound(); // Check null

                user.UserName = viewModel.ServiceEngineerRegistration.UserName;
                IdentityResult result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    result.Errors.ToList().ForEach(e => ModelState.AddModelError(string.Empty, e.Description));
                    return View(viewModel);
                }
                // Update Password (Only update if the user has entered a new password)
                if (!string.IsNullOrEmpty(viewModel.ServiceEngineerRegistration.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    IdentityResult passwordResult = await _userManager.ResetPasswordAsync(user, token, viewModel.ServiceEngineerRegistration.Password);
                    if (!passwordResult.Succeeded)
                    {
                        passwordResult.Errors.ToList().ForEach(e => ModelState.AddModelError(string.Empty, e.Description));
                        return View(viewModel);
                    }
                }
                // Update claims
                var identity = await _userManager.GetClaimsAsync(user);
                var isActiveClaim = identity.FirstOrDefault(c => c.Type == "IsActive");

                // Check null before Remove
                if (isActiveClaim != null)
                {
                    await _userManager.RemoveClaimAsync(user, new Claim(isActiveClaim.Type, isActiveClaim.Value));
                }
                await _userManager.AddClaimAsync(user, new Claim("IsActive", viewModel.ServiceEngineerRegistration.IsActive.ToString()));
            }
            else
            {
                // Create User
                if (viewModel.ServiceEngineerRegistration == null)
                {
                    ModelState.AddModelError(string.Empty, "Service Engineer Registration data is required.");
                    return View(viewModel);
                }

                // Check for existing email explicitly to avoid duplicates
                var existingUser = await _userManager.FindByEmailAsync(viewModel.ServiceEngineerRegistration.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Email address is already in use.");
                    return View(viewModel);
                }

                var user = new IdentityUser
                {
                    UserName = viewModel.ServiceEngineerRegistration.UserName,
                    Email = viewModel.ServiceEngineerRegistration.Email,
                    EmailConfirmed = true
                };

                IdentityResult result = await _userManager.CreateAsync(user, viewModel.ServiceEngineerRegistration.Password);

                // Check result.Succeeded 
                if (!result.Succeeded)
                {
                    result.Errors.ToList().ForEach(e => ModelState.AddModelError(string.Empty, e.Description));
                    return View(viewModel);
                }

                // Add claims after the user has been successfully created
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, viewModel.ServiceEngineerRegistration.Email));
                await _userManager.AddClaimAsync(user, new Claim("IsActive", viewModel.ServiceEngineerRegistration.IsActive.ToString()));

                // Assign user to Role Engineer
                IdentityResult roleResult = await _userManager.AddToRoleAsync(user, "Engineer");
                if (!roleResult.Succeeded)
                {
                    roleResult.Errors.ToList().ForEach(e => ModelState.AddModelError(string.Empty, e.Description));
                    return View(viewModel);
                }
            }
            // Send Email
            if (viewModel.ServiceEngineerRegistration.IsActive)
            {
                await _emailSender.SendEmailAsync(viewModel.ServiceEngineerRegistration.Email, "Account Created/Modified",
                    $"Your account has been created/modified. Your username is {viewModel.ServiceEngineerRegistration.UserName}");
            }
            else
            {
                await _emailSender.SendEmailAsync(viewModel.ServiceEngineerRegistration.Email, "Account Deactivated",
                    $"Your account has been deactivated.");
            }
            return RedirectToAction("ServiceEngineers");
        }
        [HttpGet]
        public async Task<IActionResult> Customers()
        {
            var customers = await _userManager.GetUsersInRoleAsync("User"); // Ensure the role name matches the database
            HttpContext.Session.SetObjectAsJson("Customers", customers);
            return View(new CustomerViewModel
            {
                Customers = customers.ToList() == null ? null : customers.ToList(),
                CustomerRegistration = new CustomerRegistrationViewModel { IsEdit = false }
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Customers(CustomerViewModel viewModel)
        {
            viewModel.Customers = HttpContext.Session.GetObjectFromJson<List<IdentityUser>>("Customers") ?? new List<IdentityUser>();
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            if (viewModel.CustomerRegistration != null && viewModel.CustomerRegistration.IsEdit)
            {
                //Update User 
                //Update Claims IsActive
                var user = await _userManager.FindByEmailAsync(viewModel.CustomerRegistration.Email);
                if (user == null)
                {
                    return NotFound(); // Prevents CS8604 by ensuring user is not null
                }
                var identity = await _userManager.GetClaimsAsync(user);
                var isActiveClaim = identity.FirstOrDefault(c => c.Type == "IsActive");
                if (isActiveClaim != null)
                {
                    var removeClaimResult = await _userManager.RemoveClaimAsync(user, new System.Security.Claims.Claim(isActiveClaim.Type, isActiveClaim.Value));
                }
                var addClaimResult = await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("IsActive", viewModel.CustomerRegistration.IsActive.ToString()));
            }
            if (viewModel.CustomerRegistration != null && viewModel.CustomerRegistration.IsActive)
            {
                await _emailSender.SendEmailAsync(viewModel.CustomerRegistration.Email, "Account Created/Modified",
                    $"Your account has been created/modified. Your username is {viewModel.CustomerRegistration.UserName}");
            }
            else if (viewModel.CustomerRegistration != null)
            {
                await _emailSender.SendEmailAsync(viewModel.CustomerRegistration.Email, "Account Deactivated",
                    $"Your account has been deactivated.");
            }
            return RedirectToAction("Customers");
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var currentUserDetails = HttpContext.User.GetCurrentUserDetails();
            if (currentUserDetails == null || string.IsNullOrEmpty(currentUserDetails.Email))
            {
                return NotFound();
            }

            var user = await _userManager.FindByEmailAsync(currentUserDetails.Email);
            if (user == null)
            {
                return NotFound();
            }

            return View(new ProfileModel
            {
                UserName = user.UserName
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileModel profile)
        {
            if (!ModelState.IsValid)
            {
                return View(profile);
            }

            var currentUserDetails = HttpContext.User.GetCurrentUserDetails();
            if (currentUserDetails == null || string.IsNullOrEmpty(currentUserDetails.Email))
            {
                return NotFound(); // Prevents CS8602 by ensuring currentUserDetails and Email are not null
            }

            var user = await _userManager.FindByEmailAsync(currentUserDetails.Email);
            if (user == null)
            {
                return NotFound(); // Prevents CS8602 by ensuring user is not null
            }

            user.UserName = profile.UserName;
            IdentityResult result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(e => ModelState.AddModelError(string.Empty, e.Description));
                return View(profile);
            }
            await _signInManager.RefreshSignInAsync(user);

            return RedirectToAction("Dashboard", "Dashboard", new { area = "ServiceRequests" });
        }
    }
}