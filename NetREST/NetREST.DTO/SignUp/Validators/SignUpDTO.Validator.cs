using FluentValidation;
using NetREST.Common.Errors;

namespace NetREST.DTO.SignUp.Validators
{
    public class SignUpDTOValidator : AbstractValidator<SignUpDTO>
    {
        public SignUpDTOValidator()
        {
            RuleFor(user => user.FirstName)
                .NotEmpty().WithErrorCode(ErrorCodes.Common.FieldInvalid);
            RuleFor(user => user.LastName)
                .NotEmpty().WithErrorCode(ErrorCodes.Common.FieldInvalid);
            RuleFor(user => user.Age)
                .NotEmpty().WithErrorCode(ErrorCodes.Common.FieldInvalid);
            RuleFor(user => user.Email)
                .NotEmpty().EmailAddress().WithErrorCode(ErrorCodes.Common.FieldInvalid);
            // string pattern = "/^[0-9a-zA-Z~!@#$%^&*_\\-+=`|(){}[\\]:;\"'<>,.?/]+$";
            // string pattern = "/^[0-9a-zA-Z~!@#$%^&*_\\-+=`|(){}[\\]:;\"'<>,.?/]+$";
            RuleFor(auth => auth.Password)
                .NotEmpty()
                // .Must(pass => Regex.IsMatch(pass, pattern, RegexOptions.Compiled))
                .WithErrorCode(ErrorCodes.Common.FieldInvalid);
        }
    }
}