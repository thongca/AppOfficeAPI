using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using HumanResource.Data.Entities.System;

namespace HumanResource.Data.Configurations.System
{
    class Sys_Dm_ProvinceConfiguration : IEntityTypeConfiguration<Sys_Dm_Province>
    {
        public void Configure(EntityTypeBuilder<Sys_Dm_Province> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
