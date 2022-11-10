using CardStorageService.Data;
using CardStorageService.Models.Requests;
using FluentValidation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CardStorageService.Models.Validators
{
    public class CreateClientRequestValidator : AbstractValidator<CreateClientRequest>
    {
        public CreateClientRequestValidator()
        {
            RuleFor(x => x.Surname)
                .NotNull()
                .Length(1, 255);
            RuleFor(x => x.FirstName)
                .NotNull()
                .Length(1, 255);
            RuleFor(x => x.Patronymic)
                .NotNull()
                .Length(1, 255);
        }

    }
}
