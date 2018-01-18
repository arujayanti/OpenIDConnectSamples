using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace Core2ADFSAuth.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult SignIn()
        {
			return Challenge(new AuthenticationProperties
			{
				RedirectUri = "/"
			}, OpenIdConnectDefaults.AuthenticationScheme);			
		}

        public IActionResult SignOut()
        {
            return SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = "/Account/SignedOut"
                },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult SignedOut() => View();
    }
}
