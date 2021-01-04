using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using NetREST.DAL.Repository.Users;
using NetREST.DTO.Auth;

namespace NetREST.BLL.Services.Auth.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        
        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ClaimsPrincipal> Authenticate(AuthDTO dto)
        {
            var userEntity = await _userRepository.GetUserByEmail(dto.Email);

            if (userEntity == null)
                return null;

            if (userEntity.Password != dto.Password)
                return null;

            return CreateClaims(dto.Email);
        }
        
        private ClaimsPrincipal CreateClaims(string userLogin)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userLogin)
            };
            ClaimsIdentity id = new ClaimsIdentity(
                claims, 
                "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultNameClaimType);
            return new ClaimsPrincipal(id);
        }
    }
}