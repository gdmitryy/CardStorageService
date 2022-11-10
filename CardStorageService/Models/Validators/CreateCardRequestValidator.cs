using CardStorageService.Data;
using CardStorageService.Models.Requests;
using FluentValidation;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CardStorageService.Models.Validators
{
    public class CreateCardRequestValidator : AbstractValidator<CreateCardRequest>
    {
        public CreateCardRequestValidator()
        {
            RuleFor(x => x.ClientId)
                .NotNull();
            RuleFor(x => x.CardNo)
                .NotNull()
                .Length(20);
            RuleFor(x => x.Name)
                .NotNull()
                .Length(1, 50);
            RuleFor(x => x.CVV2)
                .NotNull()
                .Length(3, 50);
            RuleFor(x => x.ExpDate)
                .NotNull();                
        }

    }
}
