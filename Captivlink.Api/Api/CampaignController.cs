using System;
using IdentityServer4;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using Captivlink.Api.Models.Requests;
using Captivlink.Application.Campaigns.Commands;
using Captivlink.Api.Utility;
using Captivlink.Application.Campaigns.Queries;
using Captivlink.Application.Campaigns.Results;
using Captivlink.Infrastructure.Utility;
using Swashbuckle.AspNetCore.Annotations;

namespace Captivlink.Api.Api
{
    [Route("api/campaign")]
    [ApiController]
    [Authorize(Policy = IdentityServerConstants.LocalApi.PolicyName, Roles = "Business")]
    public class CampaignController : AbstractController
    {
        private readonly IMediator _mediatr;
        private readonly IMapper _mapper;

        public CampaignController(IMediator mediatr, IMapper mapper)
        {
            _mediatr = mediatr;
            _mapper = mapper;
        }

        [HttpPost]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(404, "Not found")]
        public async Task<IActionResult> CreateCampaignAsync([FromBody] CampaignRequest request)
        {
            var command = _mapper.Map<CreateCampaignCommand>(request);
            command.UserId = User.GetUserGuid();

            var result = await _mediatr.Send(command);

            if (!result.IsValid)
            {
                return ValidationProblem(result);
            }

            return Ok();
        }

        [HttpGet]
        [SwaggerResponse(200, "Success", typeof(PaginatedResult<CampaignResult>))]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(404, "Not found")]
        public async Task<IActionResult> GetAllCampaignsAsync([FromQuery] PaginationRequest request)
        {
            var query = _mapper.Map<GetAllCampaignQuery>(request);
            query.UserId = User.GetUserGuid();

            var result = await _mediatr.Send(query);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPut("{id}")]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(404, "Not found")]
        public async Task<IActionResult> UpdateCampaignAsync(Guid id, [FromBody] CampaignRequest request)
        {
            var command = _mapper.Map<UpdateCampaignCommand>(request);
            command.UserId = User.GetUserGuid();
            command.Id = id;

            var result = await _mediatr.Send(command);

            if (!result.IsValid)
            {
                return ValidationProblem(result);
            }

            return Ok();
        }

        [HttpGet("{id}")]
        [SwaggerResponse(200, "Success", typeof(CampaignResult))]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(404, "Not found")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var query = new GetByIdCampaignQuery()
            {
                CampaignId = id,
                UserId = User.GetUserGuid()
            }; 

            var result = await _mediatr.Send(query);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
