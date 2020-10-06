using HumanResource.Data.Entities.VanBan;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.VanBan
{
    public class VB_QT_LuanChuyenVanBanConfiguration : IEntityTypeConfiguration<VB_QT_LuanChuyenVanBan>
    {
        public void Configure(EntityTypeBuilder<VB_QT_LuanChuyenVanBan> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(t => t.VbMoiSoHoaId).IsRequired();
            builder.Property(t => t.NguoiGuiId).IsRequired();
        }
    }
}
