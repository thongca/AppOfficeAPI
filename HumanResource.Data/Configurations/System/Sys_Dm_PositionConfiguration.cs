using HumanResource.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.System
{
    public class Sys_Dm_PositionConfiguration : IEntityTypeConfiguration<Sys_Dm_Position>
    {
        public void Configure(EntityTypeBuilder<Sys_Dm_Position> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
        }
    }
}
