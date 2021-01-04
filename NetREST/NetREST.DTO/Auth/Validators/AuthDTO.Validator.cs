using System.Text.RegularExpressions;
using FluentValidation;
using NetREST.Common.Errors;

namespace NetREST.DTO.Auth.Validators
{
    public class AuthDTOValidator : AbstractValidator<AuthDTO>
    {
        public AuthDTOValidator()
        {
            // string pattern = "/^[0-9a-zA-Z~!@#$%^&*_\\-+=`|(){}[\\]:;\"'<>,.?/]+$";
            RuleFor(auth => auth.Password)
                .NotEmpty()
                // .Must(pass => Regex.IsMatch(pass, pattern, RegexOptions.Compiled))
                .WithErrorCode(ErrorCodes.Common.FieldInvalid);
            RuleFor(auth => auth.Email)
                .EmailAddress().WithErrorCode(ErrorCodes.Common.FieldInvalid);
        }   
    }
}