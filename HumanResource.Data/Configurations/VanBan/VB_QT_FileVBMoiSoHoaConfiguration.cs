using HumanResource.Data.Entities.VanBan;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.VanBan
{
    public class VB_QT_FileVBMoiSoHoaConfiguration : IEntityTypeConfiguration<VB_QT_FileVBMoiSoHoa>
    {
        public void Configure(EntityTypeBuilder<VB_QT_FileVBMoiSoHoa> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(t => t.VbMoiSoHoaId).IsRequired();
        }
    }
}
