using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using HumanResource.Data.Entities.System;

namespace HumanResource.Data.Configurations.System
{
    class Sys_cog_PermissionConfiguration : IEntityTypeConfiguration<Sys_Cog_Permission>
    {
        public void Configure(EntityTypeBuilder<Sys_Cog_Permission> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(t => t.GroupRoleId).IsRequired();
            builder.Property(t => t.MenuId).IsRequired();
        }
    }
}
