using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using HumanResource.Data.Entities.System;

namespace HumanResource.Data.Configurations
{
    class Sys_ctg_UserConfiguration : IEntityTypeConfiguration<Sys_Dm_User>
    {
        public void Configure(EntityTypeBuilder<Sys_Dm_User> builder)
        {
            builder.HasKey(x=> x.Id);
            builder.Property(t => t.Code).IsRequired();
            builder.Property(t => t.Username).IsRequired();
            builder.Property(t => t.Password).IsRequired();
        }
    }
}
