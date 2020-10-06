using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using HumanResource.Data.Entities.System;

namespace HumanResource.Data.Configurations.System
{
    class Sys_Dm_DistrictConfiguration : IEntityTypeConfiguration<Sys_Dm_District>
    {
        public void Configure(EntityTypeBuilder<Sys_Dm_District> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(t => t.ProvinceId).IsRequired();
        }
    }
}
