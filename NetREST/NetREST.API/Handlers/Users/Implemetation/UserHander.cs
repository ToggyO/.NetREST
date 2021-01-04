using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using NetREST.Common.Errors;
using NetREST.Common.Pagination;
using NetREST.Common.Response;
using NetREST.DAL.Repository.Users;
using NetREST.Domain.User;
using NetREST.DTO.User;

namespace NetREST.API.Handlers.Users.Implemetation
{
    public class UserHander : IUserHandler
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserHander(IUserRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Response<PaginationModel<UserDTO>>> GetList(PaginationModel model)
        {
            var users = await _repository.GetList(model);
            var result = new PaginationModel<UserDTO>
            {
                Page = users.Page,
                PageSize = users.PageSize,
                Total = users.Total,
                Items = _mapper.Map<IEnumerable<UserModel>, IEnumerable<UserDTO>>(users.Items),
            };
            
            return new Response<PaginationModel<UserDTO>>
            {
                ResultData = result,
            };
        }

        public async Task<Response<UserDTO>> GetUserById(int id)
        {
            var userEntity = await _repository.GetById(id);
            if (userEntity == null)
                return new ErrorResponse<UserDTO>
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    ErrorCode = ErrorCodes.Common.NotFound,
                    ErrorMessage = ErrorMessages.Business.UserDoesNotExists,
                    Errors = new Error[0],
                };
            
            UserDTO dto = _mapper.Map<UserModel, UserDTO>(userEntity);

            return new Response<UserDTO>
            {
                ResultData = dto
            };
        }

        public async Task<Response<UserDTO>> UpdateUser(int id, UserDTO dto)
        {
            var userEntity = await _repository.GetById(id);
            if (userEntity == null)
                return new ErrorResponse<UserDTO>
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    ErrorCode = ErrorCodes.Common.NotFound,
                    ErrorMessage = ErrorMessages.Business.UserDoesNotExists,
                    Errors = new Error[0],
                };

            userEntity.FirstName = dto.FirstName;
            userEntity.LastName = dto.LastName;
            userEntity.Age = dto.Age;
            
            userEntity = await _repository.Update(userEntity);
            UserDTO result = _mapper.Map<UserModel, UserDTO>(userEntity);

            return new Response<UserDTO>
            {
                ResultData = result
            };
        }

        public async Task<Response<UserDTO>> DeleteUser(int id)
        {
            UserModel userEntity = await _repository.GetById(id);
            if (userEntity == null)
                return new ErrorResponse<UserDTO>
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    ErrorCode = ErrorCodes.Common.NotFound,
                    ErrorMessage = ErrorMessages.Business.UserDoesNotExists,
                    Errors = new Error[0],
                };

            await _repository.Delete(id);

            return new Response<UserDTO>
            {
                ResultData = null
            };
        }
    }
}