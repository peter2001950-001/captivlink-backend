using AutoMapper;
using Captivlink.Application.Campaigns.Results.Performance;
using Captivlink.Application.Commons;
using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;
using Captivlink.Application.Users.Results;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Repositories.Contracts;

namespace Captivlink.Application.Campaigns.Queries.Handlers
{
    public class GetCampaignPerformanceQueryHandler: IQueryHandler<GetCampaignPerformanceQuery, CampaignPerformanceResult?>

    {
        private readonly IUserRepository _userRepository;
        private readonly ICampaignEventRepository _campaignEventRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICampaignPartnerRepository _campaignPartnerRepository;
        private readonly IMapper _mapper;

        public GetCampaignPerformanceQueryHandler(IUserRepository userRepository, ICampaignEventRepository campaignEventRepository, ICampaignPartnerRepository campaignPartnerRepository, ICampaignRepository campaignRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _campaignEventRepository = campaignEventRepository;
            _campaignPartnerRepository = campaignPartnerRepository;
            _campaignRepository = campaignRepository;
            _mapper = mapper;
        }

        public async Task<CampaignPerformanceResult?> Handle(GetCampaignPerformanceQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            if (user == null || user.Company == null)
            {
                return null;
            }

            var campaign = await  _campaignRepository.GetFirstOrDefaultAsync(x =>  x.Id == request.CampaignId && x.Company.Id == user.Company.Id);
            if (campaign == null) return null;

            var partners = await _campaignPartnerRepository.FindWhereAsync(x => x.Campaign.Id == campaign.Id && x.Status == CampaignPartnerStatus.Approved, null);
            var events = (await _campaignEventRepository.FindWhereAsync(x => x.CampaignPartner.Campaign.Id == campaign.Id, null)).ToList();
            var eventsForPeriod = events.Where(x => x.CreatedOn > request.StartDate && x.CreatedOn < request.EndDate).ToList();
            var awards = campaign.Awards.ToList();

            var result = new CampaignPerformanceResult()
            {
                MetricsForPeriod = GenerateMetrics(awards, eventsForPeriod),
                MetricsLifetime = GenerateMetrics(awards, events),
                TotalPartnerships = partners.Count(),
                ClickData = GenerateClickData(eventsForPeriod, request.StartDate, request.EndDate),
                PurchaseData = GeneratePurchaseData(eventsForPeriod, request.StartDate, request.EndDate),
                PurchaseValueData = GeneratePurchaseValueData(eventsForPeriod, request.StartDate, request.EndDate),
                CreatorPerformances = GenerateCampaignCreatorPerformanceResults(eventsForPeriod, events, partners.Select(x=>x.ContentCreator).ToList(), awards)
            };
            return result;
        }


        private static ChartResult GenerateClickData(List<CampaignEvent> events, DateTime startTime, DateTime endDateTime)
        {
            var periods = GenerateChartLabelPeriods(startTime, endDateTime);
            var values = new List<double>();
            for (int i = 0; i < periods.Count-1; i++)
            {
                values.Add(events.Count(x => x.CreatedOn > periods[i] && x.CreatedOn< periods[i+1] && x.Type == CampaignEventType.Click));
            }

            values.Add(events.Count(x => x.CreatedOn > periods.Last() && x.Type == CampaignEventType.Click));


            return new ChartResult()
            {
                Labels = periods,
                Values = values
            };
        }

        private static ChartResult GeneratePurchaseData(List<CampaignEvent> events, DateTime startTime, DateTime endDateTime)
        {
            var periods = GenerateChartLabelPeriods(startTime, endDateTime);
            var values = new List<double>();
            for (int i = 0; i < periods.Count - 1; i++)
            {
                values.Add(events.Count(x => x.CreatedOn > periods[i] && x.CreatedOn < periods[i + 1] && x.Type == CampaignEventType.Purchase));
            }
            values.Add(events.Count(x => x.CreatedOn > periods.Last() && x.Type == CampaignEventType.Purchase));

            return new ChartResult()
            {
                Labels = periods,
                Values = values
            };
        }

        private static ChartResult GeneratePurchaseValueData(List<CampaignEvent> events, DateTime startTime, DateTime endDateTime)
        {
            var periods = GenerateChartLabelPeriods(startTime, endDateTime);
            var values = new List<double>();
            for (int i = 0; i < periods.Count - 1; i++)
            {
                values.Add(events.Where(x => x.CreatedOn > periods[i] && x.CreatedOn < periods[i + 1] && x.Type == CampaignEventType.Purchase).Sum(x => x.Amount ?? 0));
            }
            values.Add(events.Where(x => x.CreatedOn > periods.Last() && x.Type == CampaignEventType.Purchase).Sum(x => x.Amount ?? 0));

            return new ChartResult()
            {
                Labels = periods,
                Values = values
            };
        }

        private List<CampaignCreatorPerformanceResult> GenerateCampaignCreatorPerformanceResults(List<CampaignEvent> eventsForPeriod, List<CampaignEvent> allEvents, List<PersonDetails> partners, List<Award> awards) 
        {
            var result = new List<CampaignCreatorPerformanceResult>();
            foreach (var partner in partners)
            {
                var periodEventsForPartner = eventsForPeriod.Where(x => x.CampaignPartner.ContentCreator.Id == partner.Id).ToList();
                var allEventForPartner = allEvents.Where(x => x.CampaignPartner.ContentCreator.Id == partner.Id).ToList();
                
                result.Add(new CampaignCreatorPerformanceResult()
                {
                    ContentCreator = _mapper.Map<PersonResult>(partner),
                    MetricsForPeriod = GenerateMetrics(awards, periodEventsForPartner),
                    MetricsLifetime = GenerateMetrics(awards, allEventForPartner)
                });
            }

            return result;
        }

        private static List<DateTime> GenerateChartLabelPeriods(DateTime startTime, DateTime endDateTime)
        {
            var periods = new List<DateTime>();
            if ((endDateTime - startTime).TotalHours<=24)
            {
                for (DateTime start = startTime; start < endDateTime; start = start.AddHours(1))
                {
                    periods.Add(start);
                }
                return periods;
            }
            else if((endDateTime - startTime).TotalHours <= 48)
            {
                for (DateTime start = startTime; start < endDateTime; start = start.AddHours(2))
                {
                    periods.Add(start);
                }
                return periods;
            }
            else
            {
                for (DateTime start = startTime; start < endDateTime; start = start.AddHours(24))
                {
                    periods.Add(start);
                }
                return periods;
            }
        }

        private static MetricsResult GenerateMetrics(List<Award> awards, List<CampaignEvent> events)
        {
            var clicks = events.Count(x => x.Type == CampaignEventType.Click);
            var purchases = events.Count(x => x.Type == CampaignEventType.Purchase);
            var totalValue = (decimal) events.Sum(x => x.Amount ?? 0);

            return new MetricsResult()
            {
                TotalClick = clicks,
                TotalPurchases = purchases,
                TotalValue = totalValue,
                TotalAmountSpent = CalculateAmountSpent(awards, clicks, purchases, totalValue)
            };
        }


        private static decimal CalculateAmountSpent(List<Award> awards, int totalClicks, int totalPurchases, decimal totalValue)
        {
            decimal amountSpent = 0;

            if (awards.Any(x => x.Type == AwardType.PerClick))
            {
                var perClickAward = awards.First(x => x.Type == AwardType.PerClick);
                amountSpent += perClickAward.Amount * totalClicks;
            }

            if (awards.Any(x => x.Type == AwardType.PerConversion))
            {
                var perConversionAward = awards.First(x => x.Type == AwardType.PerConversion);
                amountSpent += perConversionAward.Amount * totalPurchases;
            }

            if (awards.Any(x => x.Type == AwardType.Percentage))
            {
                var percentageAward = awards.First(x => x.Type == AwardType.Percentage);
                amountSpent += (percentageAward.Amount/100) * amountSpent;
            }

            return amountSpent;
        }
    }
}
