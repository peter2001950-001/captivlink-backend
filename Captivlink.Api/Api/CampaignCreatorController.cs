using System;
using Captivlink.Api.Models.Requests;
using Captivlink.Application.Campaigns.Queries;
using Captivlink.Application.Campaigns.Results;
using Captivlink.Infrastructure.Utility;
using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using AutoMapper;
using Captivlink.Api.Utility;
using MediatR;
using Azure.Core;
using Captivlink.Application.Campaigns.Commands;
using Captivlink.Application.Campaigns.Results.Performance;

namespace Captivlink.Api.Api
{
    [Route("api/campaign-creator")]
    [ApiController]
    [Authorize(Policy = IdentityServerConstants.LocalApi.PolicyName, Roles = "ContentCreator")]
    public class CampaignCreatorController : AbstractController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public CampaignCreatorController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet("feed")]
        [SwaggerResponse(200, "Success", typeof(PaginatedResult<CampaignBusinessResult>))]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(404, "Not found")]
        public async Task<IActionResult> GetFeedAsync([FromQuery] PaginationRequest request)
        {
            var query = _mapper.Map<GetCampaignFeedQuery>(request);
            query.UserId = User.GetUserGuid();

            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


        [HttpPost("{id}/apply")]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(404, "Not found")]
        public async Task<IActionResult> ApplyCampaignAsync(Guid id)
        {
            var command = new ApplyCampaignCommand()
            {
                CampaignId = id,
                UserId = User.GetUserGuid()
            };

            var result = await _mediator.Send(command);

            if (!result.IsValid)
            {
                return ValidationProblem(result);
            }

            return Ok();
        }

        [HttpPost("{id}/revoke")]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(404, "Not found")]
        public async Task<IActionResult> RevokeCampaignAsync(Guid id)
        {
            var command = new RevokeCampaignCommand()
            {
                CampaignId = id,
                UserId = User.GetUserGuid()
            };

            var result = await _mediator.Send(command);

            if (!result.IsValid)
            {
                return ValidationProblem(result);
            }

            return Ok();
        }

        [HttpGet("partnership")]
        [SwaggerResponse(200, "Success", typeof(PaginatedResult<CampaignCreatorResult>))]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(404, "Not found")]
        public async Task<IActionResult> GetPartnershipsAsync([FromQuery] CampaignPartnershipRequest request)
        {
            var query = _mapper.Map<GetCreatorCampaignsQuery>(request);
            query.UserId = User.GetUserGuid();

            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerResponse(200, "Success", typeof(CampaignCreatorResult))]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(404, "Not found")]
        public async Task<IActionResult> GetCampaignByIdAsync(Guid id)
        {
            var query = new GetCreatorCampaignByIdQuery()
            {
                CampaignId = id,
                UserId = User.GetUserGuid()
            };
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("{id}/performance")]
        [SwaggerResponse(200, "Success", typeof(CreatorCampaignPerformanceResult))]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(404, "Not found")]
        public async Task<IActionResult> GetCampaignPerformanceAsync(Guid id, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
        {
            var query = new GetCreatorCampaignPerformanceQuery()
            {
                CampaignId = id,
                UserId = User.GetUserGuid(),
                StartDate = startTime,
                EndDate = endTime
            };
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
