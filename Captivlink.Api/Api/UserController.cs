using System;
using System.Threading.Tasks;
using AutoMapper;
using Captivlink.Api.Models.Requests;
using Captivlink.Api.Utility;
using Captivlink.Application.Users.Commands;
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
    [Authorize(Policy = IdentityServerConstants.LocalApi.PolicyName, Roles = "ContentCreator, Business")]
    public class UserController : AbstractController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public UserController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
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

        [HttpPut("profile")]
        [TrimInputStrings]
        [Authorize(Policy = IdentityServerConstants.LocalApi.PolicyName)]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(404, "Not found")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateProfileRequest request)
        {
            var command = _mapper.Map<UpdateProfileCommand>(request);
            command.UserId = User.GetUserGuid();
            command.UserRole = User.GetRole();

            var result = await _mediator.Send(command);
            if (!result.IsValid)
                return ValidationProblem(result);

            return Ok();
        }
    }
}
