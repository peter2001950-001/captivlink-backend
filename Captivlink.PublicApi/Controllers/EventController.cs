using Captivlink.PublicApi.Models;
using Captivlink.PublicApi.Utility;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Events;
using Captivlink.Infrastructure.Events.Providers;
using Captivlink.Infrastructure.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using HostingEnvironmentExtensions = Microsoft.AspNetCore.Hosting.HostingEnvironmentExtensions;

namespace Captivlink.PublicApi.Controllers
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
            if (!Request.Headers.TryGetValue("Origin", out var originHeader))
            {
                return Unauthorized(new {error = "origin is missing"});
            }

            if (Request.Cookies.TryGetValue("_ctv", out string? cookieValue) && cookieValue != null)
            {
                var decrypt = TrackingIdentifier.Parse(cookieValue);
                if (decrypt == null) return Forbid();

                var partnership = await _campaignPartnerRepository.GetCampaignPartnerByIdAsync(decrypt.CampaignCreatorId);
                if(partnership == null) return BadRequest();

                if (accessToken != partnership.Campaign.Website.AccessToken)
                {
                    return Unauthorized(new {error = "access token is invalid"});
                }

                if (!IsValidHost(partnership.Campaign.Website, originHeader[0]?.ToString()))
                {
                    return Unauthorized(new {error = originHeader[0] + " is not valid for website " + partnership.Campaign.Website.Domain});
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

        private bool IsValidHost(Website website, string? host)
        {
            Console.WriteLine(host);

            if (_webHostEnvironment.IsDevelopment())
            {
                return true;
            }

            if (host == null) return false;

            var urlSplitted = host.Split(".");
            if (website.AllowSubdomains && urlSplitted.Length>=2)
            {
                return website.Domain.Contains(urlSplitted[^2] + urlSplitted[^1]);
            }

            if (!website.AllowSubdomains && urlSplitted.Length == 2)
            {
                return website.Domain.Contains(urlSplitted[^2] + urlSplitted[^1]);
            }

            if (!website.AllowSubdomains && urlSplitted.Length == 3)
            {
                if (!urlSplitted[0].Contains("www"))
                {
                    return false;
                }
                return website.Domain.Contains(urlSplitted[^2] + urlSplitted[^1]);
            }

            return false;
        }

        private void SetNextCookie(TrackingIdentifier trackingIdentifier)
        {
            var next = TrackingIdentifier.Next(trackingIdentifier);
            if (next != null)
            {
                Response.Cookies.Append("_ctv", next, new CookieOptions() { SameSite = SameSiteMode.None, Secure = true, Expires = DateTimeOffset.Now.AddDays(30), HttpOnly = true });
            }
        }
    }
}
