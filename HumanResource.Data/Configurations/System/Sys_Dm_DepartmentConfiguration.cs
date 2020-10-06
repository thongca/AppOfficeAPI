using HumanResource.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Configurations.System
{
    public class Sys_Dm_DepartmentConfiguration : IEntityTypeConfiguration<Sys_Dm_Department>
    {
        public void Configure(EntityTypeBuilder<Sys_Dm_Department> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(t => t.Code).IsRequired();
            builder.Property(t => t.Name).IsRequired();
        }
    }
}
