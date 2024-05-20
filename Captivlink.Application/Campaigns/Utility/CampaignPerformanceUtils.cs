using Captivlink.Application.Commons;
using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Application.Campaigns.Results.Performance;

namespace Captivlink.Application.Campaigns.Utility
{
    public static class CampaignPerformanceUtils
    {
        public static ChartResult GenerateClickData(List<CampaignEvent> events, DateTime startTime, DateTime endDateTime)
        {
            var periods = GenerateChartLabelPeriods(startTime, endDateTime);
            var values = new List<double>();
            for (int i = 0; i < periods.Count - 1; i++)
            {
                values.Add(events.Count(x => x.CreatedOn > periods[i] && x.CreatedOn < periods[i + 1] && x.Type == CampaignEventType.Click));
            }

            values.Add(events.Count(x => x.CreatedOn > periods.Last() && x.Type == CampaignEventType.Click));


            return new ChartResult()
            {
                Labels = periods,
                Values = values
            };
        }

        public static ChartResult GeneratePurchaseData(List<CampaignEvent> events, DateTime startTime, DateTime endDateTime)
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

        public static ChartResult GeneratePurchaseValueData(List<CampaignEvent> events, DateTime startTime, DateTime endDateTime)
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

        public static List<DateTime> GenerateChartLabelPeriods(DateTime startTime, DateTime endDateTime)
        {
            var periods = new List<DateTime>();
            if ((endDateTime - startTime).TotalHours <= 24)
            {
                for (DateTime start = startTime; start < endDateTime; start = start.AddHours(1))
                {
                    periods.Add(start);
                }
                return periods;
            }
            else if ((endDateTime - startTime).TotalHours <= 48)
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

        public static MetricsResult GenerateMetrics(List<Award> awards, List<CampaignEvent> events)
        {
            var clicks = events.Count(x => x.Type == CampaignEventType.Click);
            var purchases = events.Count(x => x.Type == CampaignEventType.Purchase);
            var totalValue = (decimal)events.Sum(x => x.Amount ?? 0);

            return new MetricsResult()
            {
                TotalClick = clicks,
                TotalPurchases = purchases,
                TotalValue = totalValue,
                TotalAmountSpent = CalculateAmountSpent(awards, clicks, purchases, totalValue)
            };
        }


        public static decimal CalculateAmountSpent(List<Award> awards, int totalClicks, int totalPurchases, decimal totalValue)
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
                amountSpent += percentageAward.Amount / 100 * totalValue;
            }

            return amountSpent;
        }
    }
}
