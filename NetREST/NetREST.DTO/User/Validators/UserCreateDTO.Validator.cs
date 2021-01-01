using FluentValidation;
using NetREST.Common.Errors;
using NetREST.Domain.User;

namespace NetREST.DTO.User.Validators
{
    public class UserCreateDTOValidator : AbstractValidator<UserModel>
    {
        public UserCreateDTOValidator()
        {
            RuleFor(user => user.FirstName)
                .NotEmpty().WithErrorCode(ErrorCodes.Common.FieldInvalid);
            RuleFor(user => user.LastName)
                .NotEmpty().WithErrorCode(ErrorCodes.Common.FieldInvalid);
            RuleFor(user => user.Age)
                .NotEmpty().WithErrorCode(ErrorCodes.Common.FieldInvalid);
            RuleFor(user => user.Email)
                .NotEmpty().EmailAddress().WithErrorCode(ErrorCodes.Common.FieldInvalid);
            RuleFor(user => user.FirstName)
                .NotEmpty().WithErrorCode(ErrorCodes.Common.FieldInvalid);
        }
    }
}