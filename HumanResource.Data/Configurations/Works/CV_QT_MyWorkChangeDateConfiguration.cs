using HumanResource.Data.Entities.Works;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.Works
{
    public class CV_QT_MyWorkChangeDateConfiguration : IEntityTypeConfiguration<CV_QT_MyWorkChangeDate>
    {
        public void Configure(EntityTypeBuilder<CV_QT_MyWorkChangeDate> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("varchar(50)");
        }
    }
}
