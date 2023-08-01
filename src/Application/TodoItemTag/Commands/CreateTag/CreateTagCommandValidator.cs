using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Todo_App.Application.Common.Interfaces;

namespace Todo_App.Application.TodoItemTag.Commands.CreateTag;
public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(v => v.TagName)
            .MaximumLength(100)
            .NotEmpty();
    }
}
