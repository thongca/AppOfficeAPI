using HumanResource.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.System
{
   public class Sys_Cog_MenuDepConfiguration : IEntityTypeConfiguration<Sys_Cog_MenuDep>
    {
        public void Configure(EntityTypeBuilder<Sys_Cog_MenuDep> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(t => t.MenuId).IsRequired();
        }
    }
}
