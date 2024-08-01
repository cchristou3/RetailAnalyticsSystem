using System;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InspireWebApp.SpaBackend.Emails;
using InspireWebApp.SpaBackend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NJsonSchema.Annotations;
using NLog;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace InspireWebApp.SpaBackend.Features.Identity;

[ApiController]
[Route(RoutingHelpers.ApiRoutePrefix + "/auth")]
[AutoConstructor]
public partial class AuthController : ControllerBase
{
    private readonly IAuthMailer _authMailer;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    protected static ILogger _logger = LogManager.LoadConfiguration("NLog.config").GetCurrentClassLogger();

    #region Logout

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        Console.WriteLine("User logged out");

        return Ok();
    }

    #endregion

    private async Task<string> GenerateConfirmEmailUrl(ApplicationUser user)
    {
        var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        confirmToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmToken));

        return Url.RelativeToAbsolute(Url.Content(
            $"~/auth/confirm-email?email={WebUtility.UrlEncode(user.Email)}&code={WebUtility.UrlEncode(confirmToken)}"
        ));
    }

    #region Status

    [HttpGet("status")]
    [ResponseCache(NoStore = true)]
    public /*async Task<*/ AuthStatus /*>*/ GetStatus()
    {
        _logger.Info("GET Auth.GetStatus");
            
        if (!User.Identity!.IsAuthenticated)
            return new AuthStatus
            {
                IsLoggedIn = false
            };

        // ApplicationUser user = await _userManager.GetUserAsync(User);
        return new AuthStatus
        {
            IsLoggedIn = true,
            User = new UserInfo
            {
                Email = User.FindFirstValue(ClaimTypes.Name) ?? throw new Exception("Missing Name claim for the user")
            }
        };
    }

    public class AuthStatus
    {
        public bool IsLoggedIn { get; set; }
        public UserInfo? User { get; set; }
    }

    [JsonSchema(Name = "AuthenticatedUserInfo")]
    public class UserInfo
    {
        public string Email { get; set; } = null!;
        // public IEnumerable<UserRoleType> Roles { get; set; }
    }

    #endregion

    #region Login

    public class LoginModel
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }

    [HttpPost("login")]
    [ErrorIfAuthenticated]
    public async Task<ActionResult<SignInResult>> Login(LoginModel model)
    {
        // This doesn't count login failures towards account lockout
        // To enable password failures to trigger account lockout, set lockoutOnFailure: true
        var result = await _signInManager.PasswordSignInAsync(
            model.Username, model.Password, model.RememberMe,
            false
        );

        if (result.Succeeded) Console.WriteLine("User logged in");

        return result;
    }

    #endregion

    #region Register

    public class RegisterModel
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    [HttpPost("register")]
    [ErrorIfAuthenticated]
    public async Task<ActionResult<IdentityResult>> Register(RegisterModel model)
    {
        _logger.Info("POST Auth.Register");
        
        ApplicationUser user = new()
        {
            Email = model.Email
        };

        await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded) return Ok(result);

        // var callbackUrl = await GenerateConfirmEmailUrl(user);
        //
        // // Don't wait for the email to be sent
        // _ = Task.Run(async () =>
        // {
        //     try
        //     {
        //         await _authMailer.SendEmailConfirm(model.Email, callbackUrl);
        //     }
        //     catch (Exception e)
        //     {
        //         // TODO: Retry system
        //         _logger.Error(e, "Failed to send registration email");
        //     }
        // });

        return Ok(result);
    }

    #endregion

    #region ResendEmailConfirmation

    public class ResendEmailConfirmationModel
    {
        public string Email { get; set; } = null!;
    }

    [HttpPost("resend-email-confirmation")]
    [ErrorIfAuthenticated]
    public async Task<ActionResult> ResendEmailConfirmation(ResendEmailConfirmationModel model)
    {
        _logger.Info("POST Auth.ResendEmailConfirmation");
        
        var user = await _userManager.FindByEmailAsync(model.Email);

        // Don't reveal that the user does not exist
        if (user == null) return Ok();

        var callbackUrl = await GenerateConfirmEmailUrl(user);
        await _authMailer.SendEmailConfirm(model.Email, callbackUrl);

        return Ok();
    }

    #endregion

    #region ConfirmEmail

    public class ConfirmEmailModel
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
    }

    [HttpPost("confirm-email")]
    [ErrorIfAuthenticated]
    public async Task<ActionResult<bool>> ConfirmEmail(ConfirmEmailModel model)
    {
        _logger.Info("POST Auth.ConfirmEmail");
        
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return false;

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
        var result = await _userManager.ConfirmEmailAsync(user, code);

        return result.Succeeded;
    }

    #endregion

    #region RequestPasswordReset

    public class RequestPasswordResetModel
    {
        public string Email { get; set; } = null!;
    }

    [HttpPost("request-password-reset")]
    [ErrorIfAuthenticated]
    public async Task<ActionResult> RequestPasswordReset(RequestPasswordResetModel model)
    {
        _logger.Info("POST Auth.RequestPasswordReset");
        
        var user = await _userManager.FindByEmailAsync(model.Email);

        // Don't reveal that the user does not exist or is not confirmed
        if (user == null || !await _userManager.IsEmailConfirmedAsync(user)) return Ok();

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var callbackUrl = Url.RelativeToAbsolute(Url.Content(
            $"~/auth/complete-password-reset?email={WebUtility.UrlEncode(user.Email)}&code={WebUtility.UrlEncode(code)}"
        ));

        await _authMailer.SendPasswordReset(model.Email, callbackUrl);

        return Ok();
    }

    #endregion

    #region CompletePasswordReset

    public class CompletePasswordResetModel
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    [HttpPost("complete-password-reset")]
    [ErrorIfAuthenticated]
    public async Task<ActionResult<IdentityResult>> CompletePasswordReset(CompletePasswordResetModel model)
    {
        _logger.Info("POST Auth.CompletePasswordReset");
        
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
        return await _userManager.ResetPasswordAsync(user, code, model.Password);
    }

    #endregion
}