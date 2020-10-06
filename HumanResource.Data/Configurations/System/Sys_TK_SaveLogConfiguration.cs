using HumanResource.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.System
{
    public class Sys_TK_SaveLogConfiguration : IEntityTypeConfiguration<Sys_TK_SaveLog>
    {
        public void Configure(EntityTypeBuilder<Sys_TK_SaveLog> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UrlApi).HasColumnType("varchar(50)");
            builder.Property(x => x.ErrorLog).HasColumnType("nvarchar(500)");
        }
    }
}
