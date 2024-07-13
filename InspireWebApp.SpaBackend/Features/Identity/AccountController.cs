using System;
using System.Threading.Tasks;
using InspireWebApp.SpaBackend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace InspireWebApp.SpaBackend.Features.Identity;

[ApiController]
[Route(RoutingHelpers.ApiRoutePrefix + "/account")]
[Authorize]
[AutoConstructor]
public partial class AccountController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    protected static ILogger _logger = LogManager.LoadConfiguration("NLog.config").GetCurrentClassLogger();

    #region Helpers

    private async Task<ApplicationUser> GetApplicationUser()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null) throw new Exception("Failed to fetch the current user for changing the password");

        return user;
    }

    #endregion

    #region ChangePassword

    public class ChangePasswordModel
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

    [HttpPost("change-password")]
    public async Task<ActionResult<IdentityResult>> ChangePassword(ChangePasswordModel model)
    {
        _logger.Info("POST Account.Password");
        
        var user = await GetApplicationUser();

        var result = await _userManager.ChangePasswordAsync(
            user, model.CurrentPassword, model.NewPassword
        );

        if (result.Succeeded) await _signInManager.RefreshSignInAsync(user);

        return result;
    }

    #endregion
}