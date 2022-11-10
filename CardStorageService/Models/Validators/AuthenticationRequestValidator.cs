using CardStorageService.Models.Requests;
using FluentValidation;

namespace CardStorageService.Models.Validators
{
    public class AuthenticationRequestValidator : AbstractValidator<AuthenticationRequest>
    {
        public AuthenticationRequestValidator()
        {
            RuleFor(x => x.Login)
                .NotNull()
                .Length(1, 255)
                .EmailAddress();
            RuleFor(x => x.Password)
                .NotNull()
                .Length(1, 50);
        }

    }
}
