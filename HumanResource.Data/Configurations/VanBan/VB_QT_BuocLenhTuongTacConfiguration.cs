using HumanResource.Data.Entities.VanBan;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.VanBan
{
    public class VB_QT_BuocLenhTuongTacConfiguration : IEntityTypeConfiguration<VB_QT_BuocLenhTuongTac>
    {
        public void Configure(EntityTypeBuilder<VB_QT_BuocLenhTuongTac> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
