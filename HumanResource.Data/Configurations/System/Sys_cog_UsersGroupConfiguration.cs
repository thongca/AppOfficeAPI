using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using HumanResource.Data.Entities.System;

namespace HumanResource.Data.Configurations.System
{
    class Sys_cog_UsersGroupConfiguration : IEntityTypeConfiguration<Sys_Cog_UsersGroup>
    {
        public void Configure(EntityTypeBuilder<Sys_Cog_UsersGroup> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(t => t.UserId).IsRequired();
            builder.Property(t => t.GroupRoleId).IsRequired();
        }
    }
}
