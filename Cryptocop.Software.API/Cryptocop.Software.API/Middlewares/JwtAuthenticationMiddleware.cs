using System.Text;
using Cryptocop.Software.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Cryptocop.Software.API.Middlewares
{
    public static class JwtAuthenticationMiddleware
    {
        public static AuthenticationBuilder AddJwtTokenAuthentication(this AuthenticationBuilder builder,
            IConfiguration config)
        {
            var jwtConfig = config.GetSection("JwtConfig");
            var secret = jwtConfig.GetSection("secret").Value;
            var issuer = jwtConfig.GetSection("issuer").Value;
            var audience = jwtConfig.GetSection("audience").Value;
            var key = Encoding.ASCII.GetBytes(secret);

            builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    NameClaimType = "name"
                };
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var claim = context.Principal?.Claims.FirstOrDefault(c => c.Type == "tokenID")?.Value;
                        if (!int.TryParse(claim, out var tokenId)) { return Task.CompletedTask; }
                        var jwtTokenService = context.HttpContext.RequestServices.GetService<IJwtTokenService>();

                        if (jwtTokenService != null && jwtTokenService.IsTokenBlacklisted(tokenId))
                        {
                            context.Fail("JWT token provided is invalid.");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            return builder;
        }
    }
}