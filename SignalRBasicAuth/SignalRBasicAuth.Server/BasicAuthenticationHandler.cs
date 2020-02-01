using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SignalRBasicAuth.Server
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private AuthService _auth;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            AuthService auth) : base(options, logger, encoder, clock)
        {
            _auth = auth;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Logger.LogInformation("Authentication user");

            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("The Authorization Header is missing");

            bool isValid = false;
            string userId = string.Empty;
            string displayName = string.Empty;

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                var username = credentials[0];
                var password = credentials[1];
                (isValid, userId, displayName) = await _auth.ValidateCredentialsAsync(username, password);

                Logger.LogInformation($"IsValid: {isValid}  UserId: {userId}  Display Name: {displayName}");
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header supplied.");
            }

            if (isValid is false)
                return AuthenticateResult.Fail("Invalid credentials");

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, displayName),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
