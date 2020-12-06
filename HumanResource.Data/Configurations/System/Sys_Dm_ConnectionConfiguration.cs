using HumanResource.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.System
{
    public class Sys_Dm_ConnectionConfiguration : IEntityTypeConfiguration<Sys_Dm_Connection>
    {
        public void Configure(EntityTypeBuilder<Sys_Dm_Connection> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
