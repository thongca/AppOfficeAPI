using HumanResource.Data.Entities.VanBan;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.VanBan
{
    public class VB_Dm_LinhVucConfiguration : IEntityTypeConfiguration<VB_Dm_LinhVuc>
    {
        public void Configure(EntityTypeBuilder<VB_Dm_LinhVuc> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
