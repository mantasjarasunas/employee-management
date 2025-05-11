using Domain.Employee;
using FluentValidation;

namespace Business.Validators;

public class CreateEmployeeRequestModelValidator : AbstractValidator<CreateEmployeeRequestModel>
{
    public CreateEmployeeRequestModelValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().NotNull().WithMessage("First Name must not be empty");

        RuleFor(x => x.LastName)
            .NotEmpty().NotNull().WithMessage("Last Name must not be empty");
    }
}