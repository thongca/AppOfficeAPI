using HumanResource.Data.Entities.Works;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.Works
{
    public class CV_QT_SpaceTimeOnDayConfiguration : IEntityTypeConfiguration<CV_QT_SpaceTimeOnDay>
    {
        public void Configure(EntityTypeBuilder<CV_QT_SpaceTimeOnDay> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
