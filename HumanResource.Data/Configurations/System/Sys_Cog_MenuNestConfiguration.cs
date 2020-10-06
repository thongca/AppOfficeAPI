using HumanResource.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.System
{
    class Sys_Cog_MenuNestConfiguration : IEntityTypeConfiguration<Sys_Cog_MenuNest>
    {
        public void Configure(EntityTypeBuilder<Sys_Cog_MenuNest> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(t => t.MenuId).IsRequired();
        }
    }
}
