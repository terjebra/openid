using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace OpenId
{
    public class AuthorizationProvider : OpenIdConnectServerProvider
    {
        public override Task ValidateAuthorizationRequest(ValidateAuthorizationRequestContext context)
        {
            return base.ValidateAuthorizationRequest(context);
        }

        public override Task ValidateTokenRequest(ValidateTokenRequestContext context)
        {
            context.Skip();
            return Task.FromResult(0);
        }
        public override async Task HandleTokenRequest(HandleTokenRequestContext context)
        {
            if (context.Request.IsPasswordGrantType())
            {

                var userManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<User>>();

                var user =
                    await
                        userManager.PasswordSignInAsync(context.Request.Username, context.Request.Password, false, true);


                if (!user.Succeeded)
                {
                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidGrant,
                        description: "Invalid user credentials.");
                }


                var identity = new ClaimsIdentity(OpenIdConnectServerDefaults.AuthenticationScheme);


                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString(),
                    OpenIdConnectConstants.Destinations.AccessToken,
                    OpenIdConnectConstants.Destinations.IdentityToken));


                identity.AddClaim(new Claim(
                    ClaimTypes.Name, context.Request.Username,
                    OpenIdConnectConstants.Destinations.AccessToken,
                    OpenIdConnectConstants.Destinations.IdentityToken));

                var ticket = new AuthenticationTicket(
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties(),
                    OpenIdConnectServerDefaults.AuthenticationScheme);


            
                context.Validate(ticket);
            }
        }

    }
}
