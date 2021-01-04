using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using NetREST.Common.Errors;
using NetREST.Common.Response;
using NetREST.DAL.Repository.Users;
using NetREST.Domain.User;
using NetREST.DTO.Auth;
using NetREST.DTO.SignUp;
using NetREST.DTO.User;

namespace NetREST.API.Handlers.Auth.Implementation
{
    public class AuthHandler : IAuthHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        
        public AuthHandler(IUserRepository userRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        
        public async Task<Response<AuthInfoDTO>> Login(AuthDTO dto)
        {
            UserModel userEntity = await _userRepository.GetUserByEmail(dto.Email);

            return new Response<AuthInfoDTO>
            {
                ResultData = new AuthInfoDTO
                {
                    User = _mapper.Map<UserModel, UserDTO>(userEntity),
                }
            };
        }

        public Response Logout()
        {
            return new Response();
        }
        
        public async Task<Response<UserDTO>> SignUp(SignUpDTO dto)
        {
            UserModel user = await _userRepository.GetUserByEmail(dto.Email);
            if (user != null)
                return new ErrorResponse<UserDTO>
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    ErrorCode = ErrorCodes.Business.EmailExists,
                    ErrorMessage = ErrorMessages.Business.EmailExists,
                    Errors = new Error[0],
                };

            UserModel userEntity = new UserModel
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Age = dto.Age,
                Email = dto.Email,
                Password = dto.Password,
            };

            user = await _userRepository.Create(userEntity);
            UserDTO result = _mapper.Map<UserModel, UserDTO>(user);

            return new Response<UserDTO>
            {
                ResultData = result
            };
        }
    }
}