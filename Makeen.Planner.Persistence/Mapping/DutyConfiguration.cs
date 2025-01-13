using Domains;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapping
{
    public class TaskConfiguration : IEntityTypeConfiguration<Domains.Task>
    {
        public void Configure(EntityTypeBuilder<Domains.Task> builder)
        {
            builder.HasOne(x => x.User);
        }
    }
}
