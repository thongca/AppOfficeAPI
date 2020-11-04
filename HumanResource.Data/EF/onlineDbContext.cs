using HumanResource.Data.Configurations;
using HumanResource.Data.Configurations.Online;
using HumanResource.Data.Entities.Online;
using HumanResource.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.EF
{
    public class onlineDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public onlineDbContext(DbContextOptions<onlineDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Sys_ctg_UserConfiguration());
            modelBuilder.ApplyConfiguration(new Sys_Dm_LisesionConfiguration());
        }
        public DbSet<Sys_Dm_User> Sys_Dm_User { get; set; }
        public DbSet<Sys_Dm_Lisesion> Sys_Dm_Lisesion { get; set; }
    }
}
