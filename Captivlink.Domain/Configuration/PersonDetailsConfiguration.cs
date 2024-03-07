using Captivlink.Infrastructure.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Captivlink.Infrastructure.Configuration
{
    public class PersonDetailsConfiguration: IEntityTypeConfiguration<PersonDetails>
    {
        public void Configure(EntityTypeBuilder<PersonDetails> builder)
        {
            builder.Property(x => x.SocialMediaLinks).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<SocialLink>>(v));
        }
    }
}
