using Captivlink.Infrastructure.Repositories.Contracts;

namespace Captivlink.Infrastructure.Domain
{
    public class Category : Entity
    {
        public string Name { get; set; }
        public Category? Parent { get; set; }
        public virtual ICollection<Campaign> Campaigns { get; set; }
        public virtual ICollection<PersonDetails> PersonDetails { get; set; }
    }
}
