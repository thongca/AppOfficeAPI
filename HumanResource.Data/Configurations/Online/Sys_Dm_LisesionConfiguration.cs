using HumanResource.Data.Entities.Online;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.Online
{
    public class Sys_Dm_LisesionConfiguration : IEntityTypeConfiguration<Sys_Dm_Lisesion>
    {
        public void Configure(EntityTypeBuilder<Sys_Dm_Lisesion> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
