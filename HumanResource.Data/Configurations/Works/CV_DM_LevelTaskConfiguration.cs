using HumanResource.Data.Entities.Works;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations
{
    public class CV_DM_LevelTaskConfiguration : IEntityTypeConfiguration<CV_DM_LevelTask>
    {
        public void Configure(EntityTypeBuilder<CV_DM_LevelTask> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(t => t.Name).IsRequired();
            builder.Property(t => t.Point).IsRequired();
        }
    }
}
