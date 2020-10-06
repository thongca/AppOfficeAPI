using HumanResource.Data.Entities.Works;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.Works
{
    public class CV_DM_DefaultTaskConfiguration : IEntityTypeConfiguration<CV_DM_DefaultTask>
    {
        public void Configure(EntityTypeBuilder<CV_DM_DefaultTask> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
