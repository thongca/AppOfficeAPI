using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using HumanResource.Data.Entities.System;

namespace HumanResource.Data.Configurations.System
{
    class Sys_Cog_MenuCustomerConfiguration : IEntityTypeConfiguration<Sys_Cog_MenuCom>
    {
        public void Configure(EntityTypeBuilder<Sys_Cog_MenuCom> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(t => t.MenuId).IsRequired();
            builder.Property(t => t.CompanyId).IsRequired();
        }
    }
}
