using Captivlink.Backend.Models;
using Captivlink.Backend.Utility;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Events;
using Captivlink.Infrastructure.Events.Providers;
using Captivlink.Infrastructure.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using HostingEnvironmentExtensions = Microsoft.AspNetCore.Hosting.HostingEnvironmentExtensions;

namespace Captivlink.Backend.Controllers
{
    public class PurchaseController : ControllerBase
    {
        private readonly ICampaignPartnerRepository _campaignPartnerRepository;
        private readonly IProducerProvider _producerProvider;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PurchaseController(ICampaignPartnerRepository campaignPartnerRepository, IProducerProvider producerProvider, IWebHostEnvironment configuration)
        {
            _campaignPartnerRepository = campaignPartnerRepository;
            _producerProvider = producerProvider;
            _webHostEnvironment = configuration;
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> CollectEventAsync([FromBody] EventRequest request, [FromHeader(Name = Constants.AccessTokenHeader)] string accessToken)
        {
            var userAgent = Request.Headers.UserAgent.ToString();
            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress;
            string host = Request.HttpContext.Request.Host.Host;

            if (Request.Cookies.TryGetValue("_ctv", out string? cookieValue) && cookieValue != null)
            {
                var decrypt = TrackingIdentifier.Parse(cookieValue);
                if (decrypt == null) return Forbid();

                var partnership = await _campaignPartnerRepository.GetCampaignPartnerByIdAsync(decrypt.CampaignCreatorId);
                if(partnership == null) return BadRequest();

                if (accessToken != partnership.Campaign.Website.AccessToken)
                {
                    return Unauthorized();
                }

                if (!IsValidHost(partnership.Campaign.Website, host))
                {
                    return Unauthorized();
                }


                var purchaseEvent = new PurchaseEvent()
                {
                    AffCode = partnership.AffiliateCode!,
                    Amount = request.Amount,
                    Identifier = request.Identifier,
                    CreatedOn = DateTime.UtcNow,
                    Id = Guid.NewGuid(),
                    IpAddress = ipAddress?.ToString(),
                    SessionId = cookieValue
                };
                await _producerProvider.ProduceAsync(purchaseEvent);
                
                SetNextCookie(decrypt);
                return Ok();
            }

            return Ok();

        }

        private bool IsValidHost(Website website, string host)
        {
            if (_webHostEnvironment.IsDevelopment())
            {
                return true;
            }

            var urlSplitted = host.Split(".");
            if (urlSplitted.Length == 1)
            {
                return website.Domain == host;
            }

            if (urlSplitted.Length > 1)
            {
                var index = 0;
                if (urlSplitted[0] == "www")
                    index = 1;

                if (urlSplitted.Length == index + 3)
                {
                    if (!website.AllowSubdomains)
                        return false;

                    if (urlSplitted[index + 1] + "." + urlSplitted[index + 2] == website.Name)
                        return true;
                }
                else if (urlSplitted.Length == index + 2)
                {
                    if (urlSplitted[index] + "." + urlSplitted[index + 1] == website.Name)
                        return true;
                }
            }

            return false;
        }

        private void SetNextCookie(TrackingIdentifier trackingIdentifier)
        {
            var next = TrackingIdentifier.Next(trackingIdentifier);
            if (next != null)
            {
                Response.Cookies.Append("_ctv", next, new CookieOptions() { SameSite = SameSiteMode.Strict, Secure = true, Expires = DateTimeOffset.Now.AddDays(30), HttpOnly = true });
            }
        }
    }
}
