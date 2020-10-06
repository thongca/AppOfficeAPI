using HumanResource.Data.Entities.Works;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.Works
{
    class CV_QT_MyWorkConfiguration : IEntityTypeConfiguration<CV_QT_MyWork>
    {
        public void Configure(EntityTypeBuilder<CV_QT_MyWork> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Id).HasColumnType("varchar(50)");
            builder.Property(c => c.WorkTime).HasDefaultValue(0.0);
            builder.Property(c => c.PauseTime).HasDefaultValue(0.0);
            builder.Property(c => c.DeliverType).HasDefaultValue(0);
            builder.Property(c => c.Code).HasDefaultValue(0);
        }
    }
}
