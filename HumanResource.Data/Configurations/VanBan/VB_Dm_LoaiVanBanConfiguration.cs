using HumanResource.Data.Entities.VanBan;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.VanBan
{
   public class VB_Dm_LoaiVanBanConfiguration : IEntityTypeConfiguration<VB_Dm_LoaiVanBan>
    {
        public void Configure(EntityTypeBuilder<VB_Dm_LoaiVanBan> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
