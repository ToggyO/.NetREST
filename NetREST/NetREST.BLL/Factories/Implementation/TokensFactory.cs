using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetREST.Common.Enums;
using NetREST.Common.Settings;
using NetREST.Domain.User;
using NetREST.DTO.Auth;
using ClaimTypes = NetREST.Common.Extensions.ClaimTypes;

namespace NetREST.BLL.Factories.Implementation
{
    public class TokensFactory : ITokensFactory
    {
        private readonly IOptions<ApiSettings> _settings;

        public TokensFactory(IOptions<ApiSettings> settings)
        {
            _settings = settings;
        }

        public Task<TokenDTO> CreateToken(UserModel user)
        {
            var now = DateTime.Now;
            var accessTokenExpire = now.AddMinutes(_settings.Value.AccessTokenExpiresInMinutes);
            var refreshTokenExpire = now.AddMinutes(_settings.Value.RefreshTokenExpiresInMinutes);

            var result = new TokenDTO
            {
                AccessToken = EncodeToken(GetJwtAccessToken(user, accessTokenExpire)),
                AccessTokenExpire = accessTokenExpire,
                
                RefreshToken = EncodeToken(GetJwtRefreshToken(user, refreshTokenExpire)),
                RefreshTokenExpire = refreshTokenExpire,
            };
            return Task.FromResult(result);
        }

        public TokenStatus ValidateToken(string token, out ClaimsPrincipal principal)
        {
            try
            {
                principal = new JwtSecurityTokenHandler()
                    .ValidateToken(token, GetTokenValidationParameters(), out var securityToken);

                if (DateTime.UtcNow > securityToken.ValidTo)
                    return TokenStatus.Expired;
                
                var IsValid = (securityToken is JwtSecurityToken jwtSecurityToken &&
                    jwtSecurityToken
                    .Header
                    .Alg
                    .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase));

                return IsValid ? TokenStatus.Valid : TokenStatus.Invalid;
            }
            catch
            {
                principal = null;
                return TokenStatus.Invalid;
            }
        }

        private string EncodeToken(JwtSecurityToken token)
        {
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private JwtSecurityToken GetJwtAccessToken(UserModel user, DateTime exp)
        {
            return new JwtSecurityToken(
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_settings.Value.PublicKey)), 
                        SecurityAlgorithms.HmacSha256
                    ),
                issuer: _settings.Value.ISSUER,
                audience: _settings.Value.AUDIENCE,
                expires: exp,
                claims: new []
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim(ClaimTypes.TokenId, Guid.NewGuid().ToString("N")),
                    new Claim(ClaimTypes.UserId, user.Id.ToString("D")),
                });
        }

        private JwtSecurityToken GetJwtRefreshToken(UserModel user, DateTime exp)
        {
            return new JwtSecurityToken(
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_settings.Value.PublicKey)), 
                        SecurityAlgorithms.HmacSha256
                    ),
                issuer: _settings.Value.ISSUER,
                audience: _settings.Value.AUDIENCE,
                expires: exp,
                claims: new []
                {
                    new Claim(ClaimTypes.UserId, user.Id.ToString("D")),
                });
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _settings.Value.ISSUER,
                
                ValidateAudience = true,
                ValidAudience = _settings.Value.AUDIENCE,
                
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_settings.Value.PublicKeyBytes),
                
                ValidateLifetime = false
            };
        }
    }
}