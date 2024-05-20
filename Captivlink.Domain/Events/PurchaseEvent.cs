namespace Captivlink.Infrastructure.Events
{
    public class PurchaseEvent : BaseEvent
    {
        public override string Type => "PurchaseEvent";
        public decimal? Amount { get; set; }
        public string? Identifier { get; set; }
    }
}
