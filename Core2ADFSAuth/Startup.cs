using System;
using Core2ADFSAuth.Filters;
using Core2ADFSAuth.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Core2ADFSAuth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc(opts =>
			{
				opts.Filters.Add(typeof(AdalTokenAcquisitionExceptionFilter));
			});

			services.AddAuthorization(o =>
			{
			});

			services.Configure<OpenIdConnectOptions>(Configuration.GetSection("OpenIDAuthentication"));

			services.AddAuthentication(auth =>
			{
				auth.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				auth.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
				auth.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			})
			.AddCookie()
			.AddOpenIdConnect(opts =>
			{
				Configuration.GetSection("OpenIDAuthentication").Bind(opts);
				//opts.Scope.Add("email");
				//opts.Scope.Add("profile");

				opts.Events = new OpenIdConnectEvents
				{
					OnAuthorizationCodeReceived = async ctx =>
					{
						HttpRequest request = ctx.HttpContext.Request;
						string currentUri = UriHelper.BuildAbsolute(request.Scheme, request.Host, request.PathBase, request.Path);
						var credential = new ClientCredential(ctx.Options.ClientId, ctx.Options.ClientSecret);

						IDistributedCache distributedCache = ctx.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();

						string userId = ctx.Principal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

						var cache = new AdalDistributedTokenCache(distributedCache, userId);

						var authContext = new AuthenticationContext(ctx.Options.Authority, cache);

						AuthenticationResult result = await authContext.AcquireTokenByAuthorizationCodeAsync(
							ctx.ProtocolMessage.Code, new Uri(currentUri), credential, ctx.Options.Resource);

						ctx.HandleCodeRedemption(result.AccessToken, result.IdToken);
					},

					OnRemoteFailure = context =>
					{
						context.Response.Redirect("/");
						context.HandleResponse();
						return System.Threading.Tasks.Task.FromResult(0);
					}
				};
			});
		}
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
            }
		
			app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }
    }
}
