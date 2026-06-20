using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MochiMud.WebApp.Authentication;

namespace MochiMud.WebApp.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AccountService accountService;

        public AuthController(AccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
        {
            var result = await accountService.RegisterAsync(
                request.Username,
                request.Password,
                request.InviteCode,
                cancellationToken);

            switch (result.Outcome)
            {
                case RegisterOutcome.Success:
                    await SignInAsync(result.Account!);
                    return Ok();
                case RegisterOutcome.UsernameTaken:
                    return Conflict("Username is already taken.");
                case RegisterOutcome.InvalidInvite:
                    return BadRequest("Invalid invite code.");
                default:
                    return BadRequest(
                        "Username must be 3-20 characters (letters, digits, underscore) and password at least 8 characters.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {
            var account = await accountService.ValidateCredentialsAsync(
                request.Username,
                request.Password,
                cancellationToken);

            if (account is null)
            {
                return Unauthorized();
            }

            await SignInAsync(account);

            return Ok();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok();
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new { username = User.Identity?.Name });
        }

        private Task SignInAsync(Account account)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.Username),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            return HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }

    public sealed record RegisterRequest(string Username, string Password, string InviteCode);

    public sealed record LoginRequest(string Username, string Password);
}
