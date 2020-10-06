using HumanResource.Data.Entities.Works;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.Works
{
    public class CV_QT_MyScheduleWorkConfiguration : IEntityTypeConfiguration<CV_QT_MyScheduleWork>
    {
        public void Configure(EntityTypeBuilder<CV_QT_MyScheduleWork> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
