using System.Threading.Tasks;
using Captivlink.Application.Categories.Queries;
using IdentityServer4;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Captivlink.Api.Api
{
    [Route("api/category")]
    [ApiController]
    [Authorize(Policy = IdentityServerConstants.LocalApi.PolicyName, Roles = "ContentCreator, Business")]
    public class CategoryController : AbstractController
    {
        private readonly IMediator _mediatr;

        public CategoryController(IMediator mediatr)
        {
            _mediatr = mediatr;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            var categories = await _mediatr.Send(new GetAllCategoriesQuery());

            return Ok(categories);
        }
    }
}
