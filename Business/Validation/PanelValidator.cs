using Entities.Dto;
using FluentValidation;

public class PanelValidator : AbstractValidator<PanelRegisterDto>
{
    public PanelValidator()
    {
        RuleFor(p => p.PanelMail).NotEmpty().WithMessage("Mail cannot be empty!");
        RuleFor(p => p.PanelPassword).NotEmpty().WithMessage("Password cannot be empty!");
        RuleFor(p => p.ApplicationId).NotEmpty().WithMessage("Application ID cannot be empty!");
        RuleFor(p => p.Balance).NotEmpty().WithMessage("Balance cannot be empty!");
        RuleFor(p => p.Balance).GreaterThan(0).WithMessage("Balance must be greater than 0!");
        RuleFor(p => p.Balance).LessThan(1000000).WithMessage("Balance must be less than 1000000!");
    }
}