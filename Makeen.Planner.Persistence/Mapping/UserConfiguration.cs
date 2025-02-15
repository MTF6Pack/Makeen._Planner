using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Mapping
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasMany(x => x.Tasks);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.HasMany(x => x.Groups);/*.WithMany(x => x.Members);*/
        }
    }
}