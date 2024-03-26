using Captivlink.Infrastructure.Repositories.Contracts;

namespace Captivlink.Infrastructure.Domain
{
    public class Category : Entity
    {
        public string Name { get; set; }
        public Category? Parent { get; set; }
    }
}
