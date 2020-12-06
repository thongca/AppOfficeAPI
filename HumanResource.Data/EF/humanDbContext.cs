using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using HumanResource.Data.Configurations;
using HumanResource.Data.Configurations.System;

using HumanResource.Data.Entities.System;
using HumanResource.Data.Entities.VanBan;
using HumanResource.Data.Configurations.VanBan;
using HumanResource.Data.Entities.Works;
using HumanResource.Data.Configurations.Works;
using HumanResource.Data.Entities.Works.Reponses;

namespace HumanResource.Data.EF
{
   public class humanDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public humanDbContext(DbContextOptions<humanDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Sys_ctg_UserConfiguration());
            modelBuilder.ApplyConfiguration(new Sys_Dm_ProvinceConfiguration());
            modelBuilder.ApplyConfiguration(new Sys_Dm_MenuConfiguration());
            modelBuilder.ApplyConfiguration(new Sys_Dm_GroupRoleConfiguration());
            modelBuilder.ApplyConfiguration(new Sys_Dm_DistrictConfiguration());
            modelBuilder.ApplyConfiguration(new Sys_cog_UsersGroupConfiguration());
            modelBuilder.ApplyConfiguration(new Sys_cog_PermissionConfiguration());
            modelBuilder.ApplyConfiguration(new Sys_Cog_MenuCustomerConfiguration());
            modelBuilder.ApplyConfiguration(new Sys_Dm_CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new Sys_Dm_DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new Sys_Cog_MenuDepConfiguration());
            modelBuilder.ApplyConfiguration(new Sys_Cog_MenuNestConfiguration());
            modelBuilder.ApplyConfiguration(new Sys_TK_SaveLogConfiguration());
            modelBuilder.ApplyConfiguration(new Sys_Dm_ConnectionConfiguration());


            modelBuilder.ApplyConfiguration(new VB_QT_QuyTrinhConfiguration());
            modelBuilder.ApplyConfiguration(new VB_QT_LenhTuongTacConfiguration());
            modelBuilder.ApplyConfiguration(new VB_QT_BuocLenhGroupRoleConfiguration());
            modelBuilder.ApplyConfiguration(new VB_QT_BuocConfiguration());
            modelBuilder.ApplyConfiguration(new VB_QT_LenhTuongTacConfiguration());
            modelBuilder.ApplyConfiguration(new VB_Dm_LoaiVanBanConfiguration());
            modelBuilder.ApplyConfiguration(new VB_Dm_LinhVucConfiguration());
            modelBuilder.ApplyConfiguration(new VB_QT_VanBanMoiSoHoaConfiguration());
            modelBuilder.ApplyConfiguration(new VB_QT_FileVBMoiSoHoaConfiguration());
            modelBuilder.ApplyConfiguration(new VB_QT_LuanChuyenVanBanConfiguration());
            modelBuilder.ApplyConfiguration(new Sys_QT_ThongBaoConfiguration());
            modelBuilder.ApplyConfiguration(new Sys_Dm_PositionConfiguration());

            // cong viec
            modelBuilder.ApplyConfiguration(new CV_QT_MyWorkConfiguration());
            modelBuilder.ApplyConfiguration(new CV_DM_DefaultTaskConfiguration());
            modelBuilder.ApplyConfiguration(new CV_DM_GroupTaskConfiguration());
            modelBuilder.ApplyConfiguration(new CV_QT_MyScheduleWorkConfiguration());
            modelBuilder.ApplyConfiguration(new CV_QT_MySupportWorkConfiguration());
            modelBuilder.ApplyConfiguration(new CV_QT_MyWorkConfiguration());
            modelBuilder.ApplyConfiguration(new CV_QT_WorkFlowFileConfiguration());
            modelBuilder.ApplyConfiguration(new CV_QT_StartPauseHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new CV_DM_ErrorConfiguration());
            modelBuilder.ApplyConfiguration(new CV_QT_CounterErrorConfiguration());
            modelBuilder.ApplyConfiguration(new CV_QT_WorkNoteConfiguration());
            modelBuilder.ApplyConfiguration(new CV_QT_MyWorkChangeDateConfiguration());
            modelBuilder.ApplyConfiguration(new CV_QT_SpaceTimeOnDayConfiguration());
            modelBuilder.ApplyConfiguration(new CV_QT_DepartmentSupporterConfiguration());

        }
        // hệ thống
        public DbSet<Sys_Dm_User> Sys_Dm_User { get; set; }
        public DbSet<Sys_Dm_Province> Sys_Dm_Province { get; set; }
        public DbSet<Sys_Dm_Menu> Sys_Dm_Menu { get; set; }
        public DbSet<Sys_Dm_District> Sys_Dm_District { get; set; }
        public DbSet<Sys_Dm_GroupRole> Sys_Dm_GroupRole { get; set; }
        public DbSet<Sys_Cog_UsersGroup> Sys_Cog_UsersGroup { get; set; }
        public DbSet<Sys_Cog_Permission> Sys_Cog_Permission { get; set; }
        public DbSet<Sys_Cog_MenuCom> Sys_Cog_MenuCom { get; set; }
        public DbSet<Sys_Dm_Company> Sys_Dm_Company { get; set; }
        public DbSet<Sys_Dm_Department> Sys_Dm_Department { get; set; }
        public DbSet<Sys_Cog_MenuDep> Sys_Cog_MenuDep { get; set; }
        public DbSet<Sys_Cog_MenuNest> Sys_Cog_MenuNest { get; set; }
        public DbSet<Sys_TK_SaveLog> Sys_TK_SaveLog { get; set; }
        public DbSet<Sys_Dm_Connection> Sys_Dm_Connection { get; set; }


        public DbSet<VB_QT_QuyTrinh> VB_QT_QuyTrinh { get; set; }
        public DbSet<VB_QT_Buoc> VB_QT_Buoc { get; set; }
        public DbSet<VB_QT_LenhTuongTac> VB_QT_LenhTuongTac { get; set; }
        public DbSet<VB_QT_BuocLenhGroupRole> VB_QT_BuocLenhGroupRole { get; set; }
        public DbSet<VB_Dm_LinhVuc> VB_Dm_LinhVuc { get; set; }
        public DbSet<VB_Dm_LoaiVanBan> VB_Dm_LoaiVanBan { get; set; }
        public DbSet<VB_QT_BuocLenhTuongTac> VB_QT_BuocLenhTuongTac { get; set; }
        public DbSet<VB_QT_VanBanMoiSoHoa> VB_QT_VanBanMoiSoHoa { get; set; }
        public DbSet<VB_QT_FileVBMoiSoHoa> VB_QT_FileVBMoiSoHoa { get; set; }
        public DbSet<VB_QT_LuanChuyenVanBan> VB_QT_LuanChuyenVanBan { get; set; }
        public DbSet<Sys_QT_ThongBao> Sys_QT_ThongBao { get; set; }
        public DbSet<Sys_Dm_Position> Sys_Dm_Position { get; set; }

        // cong viec
        public DbSet<CV_QT_MyWork> CV_QT_MyWork { get; set; }
        public DbSet<CV_QT_WorkFlow> CV_QT_WorkFlow { get; set; }
        public DbSet<CV_QT_WorkFlowFile> CV_QT_WorkFlowFile { get; set; }
        public DbSet<CV_QT_MySupportWork> CV_QT_MySupportWork { get; set; }
        public DbSet<CV_QT_MyScheduleWork> CV_QT_MyScheduleWork { get; set; }
        public DbSet<CV_DM_GroupTask> CV_DM_GroupTask { get; set; }
        public DbSet<CV_DM_DefaultTask> CV_DM_DefaultTask { get; set; }
        public DbSet<CV_QT_StartPauseHistory> CV_QT_StartPauseHistory { get; set; }
        public DbSet<CV_DM_Error> CV_DM_Error { get; set; }
        public DbSet<CV_QT_CounterError> CV_QT_CounterError { get; set; }
        public DbSet<CV_QT_WorkNote> CV_QT_WorkNote { get; set; }
        public DbSet<CV_QT_MyWorkChangeDate> CV_QT_MyWorkChangeDate { get; set; }
        public DbSet<CV_QT_SpaceTimeOnDay> CV_QT_SpaceTimeOnDay { get; set; }
        public DbSet<CV_QT_DepartmentSupporter> CV_QT_DepartmentSupporter { get; set; }

        // reponses bao cao kpi thang

        public DbSet<RePort_KpiForUseraMonth> RePort_KpiForUseraMonth { get; set; }
        public DbSet<Report_TotalTimeWork> Report_TotalTimeWork { get; set; }

    }
}
