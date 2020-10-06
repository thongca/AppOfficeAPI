using HumanResource.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.System
{
    class Sys_QT_ThongBaoConfiguration : IEntityTypeConfiguration<Sys_QT_ThongBao>
    {
        public void Configure(EntityTypeBuilder<Sys_QT_ThongBao> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
