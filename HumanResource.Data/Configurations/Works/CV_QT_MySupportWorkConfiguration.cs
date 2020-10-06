using HumanResource.Data.Entities.Works;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.Works
{
   public class CV_QT_MySupportWorkConfiguration : IEntityTypeConfiguration<CV_QT_MySupportWork>
    {
        public void Configure(EntityTypeBuilder<CV_QT_MySupportWork> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
