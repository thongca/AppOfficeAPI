using HumanResource.Data.Entities.Works;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.Works
{
    public class CV_QT_WorkFlowFileConfiguration : IEntityTypeConfiguration<CV_QT_WorkFlowFile>
    {
        public void Configure(EntityTypeBuilder<CV_QT_WorkFlowFile> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
