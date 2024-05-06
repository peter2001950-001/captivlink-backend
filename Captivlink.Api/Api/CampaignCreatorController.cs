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

        [HttpGet]
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
    }
}
