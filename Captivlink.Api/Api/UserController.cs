using System;
using System.Linq;
using System.Threading.Tasks;
using Captivlink.Api.Utility;
using Captivlink.Application.Users.Queries;
using Captivlink.Application.Users.Results;
using IdentityServer4;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Captivlink.Api.Api
{
    [Route("api/user")]
    [ApiController]
    [Authorize(Policy = IdentityServerConstants.LocalApi.PolicyName, Roles = "ContentCreator")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("profile")]
        [TrimInputStrings]
        [SwaggerResponse(200, "Success", typeof(UserProfileResult))]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(404, "Not found")]
        public async Task<IActionResult> Profile()
        {
            var user = await _mediator.Send(new GetUserProfileQuery()
                {UserId = Guid.Parse(User.GetUserId())});

            if (user == null)
            {
                return NotFound();
            }
            user.Role = User.GetRole();

            return Ok(user);
        }
    }
}
