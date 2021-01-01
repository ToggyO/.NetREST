using NetREST.DAL.Repository.Users;

namespace NetREST.BLL.Services.Auth.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        
        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
    }
}