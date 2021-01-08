using System.Threading.Tasks;
using AutoMapper;
using NetREST.BLL.Factories;
using NetREST.Common.Enums;
using NetREST.Common.Errors;
using NetREST.Common.Extensions;
using NetREST.Common.Response;
using NetREST.DAL.Repository.Users;
using NetREST.Domain.User;
using NetREST.DTO.Auth;
using NetREST.DTO.User;

namespace NetREST.BLL.Services.Tokens.Implementation
{
    public class TokensService : ITokensService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokensFactory _tokensFactory;
        private readonly IMapper _mapper;
        
        public TokensService(IUserRepository userRepository,
            ITokensFactory factory, IMapper mapper)
        {
            _userRepository = userRepository;
            _tokensFactory = factory;
            _mapper = mapper;
        }

        public async Task<Response<AuthInfoDTO>> Authenticate(AuthDTO dto)
        {
            var userEntity = await _userRepository.GetUserByEmail(dto.Email);

            if (userEntity == null || userEntity.Password != dto.Password)
                return new SecurityErrorResponse<AuthInfoDTO>(new Error[0]);

            var tokenDto = await _tokensFactory.CreateToken(userEntity);
            var userDto = _mapper.Map<UserModel, UserDTO>(userEntity);
            var authInfo = new AuthInfoDTO
            {
                User = userDto,
                Token = tokenDto,
            };
            return new Response<AuthInfoDTO>
            {
                ResultData = authInfo,
            };
        }

        public async Task<Response<TokenDTO>> RefreshToken(string refreshToken)
        {
            var tokenStatus = _tokensFactory.ValidateToken(refreshToken, out var tokenPrincipal);

            if (tokenStatus != TokenStatus.Valid)
                return new PermissionDeniedErrorResponse<TokenDTO>(new Error[0]);

            var userId = tokenPrincipal.GetUserId();
            if (userId == null)
                return new SecurityErrorResponse<TokenDTO>(new Error
                {
                    Code = ErrorCodes.Security.Unauthorized,
                    Message = ErrorMessages.Security.Unauthorized,
                    Field = ErrorFields.User.Token,
                });

            var userEntity = await _userRepository.GetById((int)userId);
            if (userEntity == null)
                return new BusinessConflictErrorResponse<TokenDTO>(new Error
                {
                    Code = ErrorCodes.Business.UserDoesNotExists,
                    Message = ErrorMessages.Business.UserDoesNotExists,
                    Field = ErrorFields.User.Id
                });
            
            var tokenDto = await _tokensFactory.CreateToken(userEntity);
            return new Response<TokenDTO>
            {
                ResultData = tokenDto,
            };
        }
    }
}