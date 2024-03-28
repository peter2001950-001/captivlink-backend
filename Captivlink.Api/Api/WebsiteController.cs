using System;
using System.Threading.Tasks;
using AutoMapper;
using Captivlink.Api.Models.Requests;
using Captivlink.Api.Utility;
using Captivlink.Application.Websites.Commands;
using Captivlink.Application.Websites.Queries;
using Captivlink.Application.Websites.Results;
using Captivlink.Infrastructure.Utility;
using IdentityServer4;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Captivlink.Api.Api
{
    [Route("api/website")]
    [Authorize(Policy = IdentityServerConstants.LocalApi.PolicyName, Roles = "ContentCreator, Business")]
    [ApiController]
    public class WebsiteController : AbstractController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public WebsiteController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        [TrimInputStrings]
        [SwaggerResponse(200, "Success", typeof(WebsiteResult))]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(404, "Not found")]
        public async Task<IActionResult> CreateWebsiteAsync([FromBody] WebsiteRequest request)
        {
            var command = _mapper.Map<CreateWebsiteCommand>(request);
            command.UserId = User.GetUserGuid();

            var result = await _mediator.Send(command);

            if (!result.IsValid)
            {
                return ValidationProblem(result);
            }

            return Ok(result.Value);
        }

        [HttpPut("{id}")]
        [TrimInputStrings]
        [SwaggerResponse(200, "Success", typeof(WebsiteResult))]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(404, "Not found")]
        public async Task<IActionResult> UpdateWebsiteAsync(Guid id, [FromBody] WebsiteRequest request)
        {
            var command = _mapper.Map<UpdateWebsiteCommand>(request);
            command.UserId = User.GetUserGuid();
            command.Id = id;

            var result = await _mediator.Send(command);

            if (!result.IsValid)
            {
                return ValidationProblem(result);
            }

            return Ok(result.Value);
        }

        [HttpGet]
        [SwaggerResponse(200, "Success", typeof(PaginatedResult<WebsiteResult>))]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(404, "Not found")]
        public async Task<IActionResult> UpdateWebsiteAsync([FromQuery] PaginationRequest request)
        {
            var command = _mapper.Map<GetAllWebsiteQuery>(request);
            command.UserId = User.GetUserGuid();

            var result = await _mediator.Send(command);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(404, "Not found")]
        public async Task<IActionResult> DeleteWebsiteAsync(Guid id)
        {
            var command = new DeleteWebsiteCommand { WebsiteId = id, UserId = User.GetUserGuid() };

            var result = await _mediator.Send(command);

            if (!result.IsValid)
            {
                return ValidationProblem(result);
            }

            return Ok();
        }
    }
}
