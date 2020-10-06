using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using HumanResource.Data.Entities.System;

namespace HumanResource.Data.Configurations.System
{
    class Sys_Dm_GroupRoleConfiguration : IEntityTypeConfiguration<Sys_Dm_GroupRole>
    {
        public void Configure(EntityTypeBuilder<Sys_Dm_GroupRole> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(t => t.Name).IsRequired();
        }
    }
}
