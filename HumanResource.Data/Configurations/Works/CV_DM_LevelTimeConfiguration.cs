using HumanResource.Data.Entities.Works;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.Works
{
    public class CV_DM_LevelTimeConfiguration : IEntityTypeConfiguration<CV_DM_LevelTime>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CV_DM_LevelTime> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(t => t.Name).IsRequired();
            builder.Property(t => t.Point).IsRequired();
        }
    }
}
