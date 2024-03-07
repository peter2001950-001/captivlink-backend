using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Captivlink.Api.Api
{
    public class AbstractController: ControllerBase
    {
        protected IActionResult ValidationProblem(FluentValidation.Results.ValidationResult result)
        {
            result.AddToModelState(ModelState, string.Empty);
            return ValidationProblem();
        }
    }
}
