using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using HumanResource.Data.Entities.System;

namespace HumanResource.Data.Configurations.System
{
    public class Sys_Dm_MenuConfiguration : IEntityTypeConfiguration<Sys_Dm_Menu>
    {
        public void Configure(EntityTypeBuilder<Sys_Dm_Menu> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(t => t.Name).IsRequired();
            builder.Property(t => t.RouterLink).IsRequired();
        }
    }
}
