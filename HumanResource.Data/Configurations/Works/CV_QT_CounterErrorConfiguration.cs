using HumanResource.Data.Entities.Works;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.Works
{
    public class CV_QT_CounterErrorConfiguration : IEntityTypeConfiguration<CV_QT_CounterError>
    {
        public void Configure(EntityTypeBuilder<CV_QT_CounterError> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.MyWorkId).IsRequired();
        }
    }
}
