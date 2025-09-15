using BabeNest_Backend.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace BabeNest_Backend.Middlewares
{
    public class TokenRefreshMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenRefreshMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService authService)
        {
            var accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var refreshToken = context.Request.Headers["x-refresh-token"].FirstOrDefault();

            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                var handler = new JwtSecurityTokenHandler();
                try
                {
                    // validate token
                    var jwt = handler.ReadJwtToken(accessToken);
                    if (jwt.ValidTo < DateTime.UtcNow)
                    {
                        // token expired, try to refresh
                        var newTokens = await authService.RefreshTokenAsync(refreshToken);
                        if (newTokens != null)
                        {
                            context.Response.Headers.Add("x-access-token", newTokens.AccessToken);
                            context.Response.Headers.Add("x-refresh-token", newTokens.RefreshToken);
                        }
                    }
                }
                catch
                {
                    // invalid token, do nothing
                }
            }

            await _next(context);
        }
    }
}
