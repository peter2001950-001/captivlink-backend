using Captivlink.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Captivlink.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserProfileAsync()
        {
            var userClaim = User.Claims.First(x => x.Type== "sub");
            var user = await _mediator.Send(new GetUserProfileQuery() {UserId = Guid.Parse(userClaim.Value)});

            return Ok(user);

        }
    }
}
