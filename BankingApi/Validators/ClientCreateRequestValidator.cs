namespace BankingApi.Validators
{
    using BankingApi.Models.Requests;
    using FluentValidation;

    public class ClientCreateRequestValidator : AbstractValidator<ClientCreateRequest>
    {
        public ClientCreateRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(60)
                .WithMessage("FirstName cannot be longer than 60 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(60)
                .WithMessage("LastName cannot be longer than 60 characters.");

            RuleFor(x => x.PersonalId)
                .NotEmpty()
                .Length(11)
                .WithMessage("PersonalId cannot be longer than 11 characters.");

            RuleFor(x => x.MobileNumber)
                .NotEmpty()
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("MobileNumber must be in correct format with country code.");

            RuleFor(x => x.Sex)
                .NotEmpty()
                .WithMessage("Sex is a required field that cannot be empty.");

            RuleFor(x => x.Accounts)
                .NotEmpty()
                .Must(x => x.Any())
                .WithMessage("At least one account is required.");
        }
    }
}
