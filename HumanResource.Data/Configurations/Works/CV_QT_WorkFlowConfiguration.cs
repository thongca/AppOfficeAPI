using HumanResource.Data.Entities.Works;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.Works
{
    public class CV_QT_WorkFlowConfiguration : IEntityTypeConfiguration<CV_QT_WorkFlow>
    {
        public void Configure(EntityTypeBuilder<CV_QT_WorkFlow> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.MyWorkId).HasColumnType("varchar(50)");
            builder.Property(x => x.Id).HasColumnType("varchar(50)");
        }
    }
}
