using Captivlink.PublicApi.Services.Contract;
using Captivlink.Infrastructure.Events;
using Captivlink.Infrastructure.Events.Providers;
using Microsoft.AspNetCore.Mvc;

namespace Captivlink.PublicApi.Controllers
{
    [ApiController]
    public class ClickController : ControllerBase
    {
        private readonly ILinkService _linkService;
        private readonly IConfiguration _configuration;
        private readonly IProducerProvider _producerProvider;

        public ClickController(ILinkService linkService, IConfiguration configuration, IProducerProvider producerProvider)
        {
            _linkService = linkService;
            _configuration = configuration;
            _producerProvider = producerProvider;
        }

        [HttpGet("{affCode}")]
        public async Task<IActionResult> LinkResolveAsync(string affCode)
        {
            var link = await _linkService.GetCampaignPartnerAsync(affCode);
            if (link == null)
            {
                return Redirect(_configuration.GetValue<string>("MainAppUrl"));
            }

            var userAgent = Request.Headers.UserAgent.ToString();
            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress;
            bool isValid = false;

            if(Request.Cookies.TryGetValue("_ctv", out string? cookieValue) && cookieValue != null)
            {
                isValid = TrackingIdentifier.CheckIfValid(cookieValue, userAgent, link.Id);
            }

            if (!isValid)
            {
                cookieValue = TrackingIdentifier.Create(userAgent, link.Id);
                if (cookieValue == null) return Redirect(_configuration.GetValue<string>("MainAppUrl"));

                Response.Cookies.Append("_ctv", cookieValue, new CookieOptions(){SameSite = SameSiteMode.Strict, Secure = true, Expires = DateTimeOffset.Now.AddDays(30), HttpOnly = true});

                var clickEvent = new ClickEvent()
                {
                    AffCode = link.AffiliateCode!,
                    CreatedOn = DateTime.Now,
                    Id = Guid.NewGuid(),
                    IpAddress = ipAddress?.ToString(),
                    SessionId = cookieValue
                };

                await _producerProvider.ProduceAsync(clickEvent);
               
            }
            return Redirect(link.Campaign.Link);

        }

    }
}
