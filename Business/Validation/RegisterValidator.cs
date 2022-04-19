using System.Linq;
using DataAccess.Concrete;
using Entities.Dto;
using FluentValidation;

namespace Business.Validation
{
    public class RegisterValidator : AbstractValidator<UserRegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(p => p.Email).NotEmpty().WithMessage("Email can not be empty!");
            RuleFor(p => p.Email).EmailAddress().WithMessage("Invalid mail format!");
            RuleFor(p => p.Email).MaximumLength(299).WithMessage("Email maximum 300 character!");
            RuleFor(p => p.Email).MinimumLength(10).WithMessage("Email minimum length is 10!");
            RuleFor(p => p.Email).Must(IsEmailUnique).WithMessage("There is a registered user for this e-mail!");
            RuleFor(p => p.Password).NotEmpty().NotNull().WithMessage("Password cannot be empty!");
            RuleFor(p => p.Password).MinimumLength(9).WithMessage("Password minimum length is 9!");

        }
        
        private bool IsEmailUnique(string arg)
        {
            LicenseSystemContext context = new();

            var result = context.Users
                .FirstOrDefault(p => p.Email.ToLower() == arg.ToLower());

            if (result == null)
            {
                return true;
            }

            return false;
        }
    }
}