using HumanResource.Data.Entities.Works;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.Works
{
    public class CV_DM_GroupTaskConfiguration : IEntityTypeConfiguration<CV_DM_GroupTask>
    {
        public void Configure(EntityTypeBuilder<CV_DM_GroupTask> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
