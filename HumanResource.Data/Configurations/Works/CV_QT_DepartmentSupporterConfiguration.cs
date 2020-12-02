using HumanResource.Data.Entities.Works;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.Works
{
    public class CV_QT_DepartmentSupporterConfiguration : IEntityTypeConfiguration<CV_QT_DepartmentSupporter>
    {
        public void Configure(EntityTypeBuilder<CV_QT_DepartmentSupporter> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
