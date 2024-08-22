using backendProjesi.Models;
using Lucene.Net.Support;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using webapi.Interfaces;
using WebApi.Data;

namespace webapi.Helpers
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;
        private readonly IUser _Iuser;

        public JwtMiddleware(RequestDelegate next,IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }
        public async Task Invoke(HttpContext context,IUser userservice)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault();
            if(token == null)
            {
                await attachUserToContext(context, userservice, token);
            }
            await _next(context);
        }

        private async Task attachUserToContext(HttpContext context, IUser userservice, string? token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                context.Items["User"] = await userservice.GetUserByIdAsync(userId);
            }
            catch
            {
                // Do nothing if token validation fails
            }
        }
    }
}
