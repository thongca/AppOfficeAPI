using HumanResource.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.System
{
    class Sys_Dm_CompanyConfiguration : IEntityTypeConfiguration<Sys_Dm_Company>
    {
        public void Configure(EntityTypeBuilder<Sys_Dm_Company> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(t => t.Code).IsRequired();
            builder.Property(t => t.Name).IsRequired();
        }
    }
}
