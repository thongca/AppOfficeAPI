using HumanResource.Application.Paremeters.Dtos;
using HumanResource.Data.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResoureAPI.Common
{
    public static class CheckNguoiNhan
    {
        public static int DuocHienThiNguoiNhan(humanDbContext _context, int GroupRoleId, int BuocLenhGroupId)
        {
            var buoclenhGroup = _context.VB_QT_BuocLenhGroupRole.Find(BuocLenhGroupId);
            if (buoclenhGroup.IsAll)
            {
                // Hiện tất cả
                return 0;
            }
            if (buoclenhGroup.IsAllComCha)
            {
                // chỉ hiện công ty mẹ
                return 1;
            }
            if (buoclenhGroup.IsAllComCon)
            {
                // chỉ hiện công ty hiện tại của user
                return 2;
            }
            if (buoclenhGroup.IsDepartment)
            {
                // chỉ hiện phòng
                return 3;
            }
            if (buoclenhGroup.IsNest)
            {
                // chỉ hiện tổ
                return 4;
            }
            if (buoclenhGroup.IsNguoiGui)
            {
                // chỉ hiện tổ
                return 5;
            }
            if (buoclenhGroup.IsNguoiLap)
            {
                // chỉ hiện tổ
                return 6;
            }
            if (buoclenhGroup.IsManagement)
            {
                // chỉ hiện trưởng phòng
                return 7;
            }
            return 0;
        }
        public async static Task<List<MoHinhToChuc>> IsAllCom(humanDbContext _context)
        {
            var _listUniOn = await _context.Sys_Dm_Department.Where(x => x.ParentId == null && x.CompanyId == 1).Select(a => new
            {
                a.Id,
                a.Name,
                ParentId = a.CompanyId,
                Loai = 1
            }).Union(
                  _context.Sys_Dm_Company.Where(x => x.ParentId == 1).Select(c => new
                  {
                      c.Id,
                      c.Name,
                      ParentId = 1,
                      Loai = 0
                  })).ToListAsync();

            var _listDepartMents = _context.Sys_Dm_Department.Where(x => x.ParentId == null).Select(a => new
            {
                a.Id,
                a.Name,
                a.CompanyId,
                Loai = 1,
                children = _context.Sys_Dm_Department.Where(x => x.ParentId == a.Id).Select(v => new {
                    v.Id,
                    v.Name,
                    Loai = 2
                }).ToList()
            });
            var s = _listUniOn.Select(x => new
            {
                x.Id,
                x.Name,
                x.ParentId,
                x.Loai,
                children = x.Loai == 0 ? _listDepartMents.Where(b => b.CompanyId == x.Id).Select(k => new
                {
                    k.Id,
                    k.Name,
                    k.Loai,
                    k.children
                }).ToList() : _listDepartMents.Where(b => b.CompanyId == 0).Select(k => new
                {
                    k.Id,
                    k.Name,
                    k.Loai,
                    k.children
                }).ToList()
            });
            var tables =((List<MoHinhToChuc>) from a in _context.Sys_Dm_Company
                         where a.ParentId == null
                         select new
                         {
                             a.Id,
                             a.Name,
                             Loai = 0,
                             children = s.ToList()
                         }).ToList();
            return tables;
        }
        public static List<MoHinhToChuc> IsComParentOnly(humanDbContext _context)
        {
            
            var _listDepartMents = _context.Sys_Dm_Department.Where(x => x.ParentId == null).Select(a => new
            {
                a.Id,
                a.Name,
                a.CompanyId,
                Loai = 1,
                children = _context.Sys_Dm_Department.Where(x => x.ParentId == a.Id).Select(v => new {
                    v.Id,
                    v.Name,
                    Loai = 2
                }).ToList()
            });
            
            var tables = ((List<MoHinhToChuc>)from a in _context.Sys_Dm_Company
                                             where a.ParentId == null
                                             select new
                                             {
                                                 a.Id,
                                                 a.Name,
                                                 Loai = 0,
                                                 children = _listDepartMents.Where(x=>x.CompanyId == a.Id).Select(c=> new {
                                                 c.Id,
                                                 c.Name,
                                                 c.Loai,
                                                 c.children
                                                 }).ToList()
                                             }).ToList();
            return tables;
        }
        public static List<MoHinhToChuc> IsComCurrentOnly(humanDbContext _context,int CompanyId)
        {

            var _listDepartMents = _context.Sys_Dm_Department.Where(x => x.ParentId == null).Select(a => new
            {
                a.Id,
                a.Name,
                a.CompanyId,
                Loai = 1,
                children = _context.Sys_Dm_Department.Where(x => x.ParentId == a.Id).Select(v => new {
                    v.Id,
                    v.Name,
                    Loai = 2
                }).ToList()
            });

            var tables = ((List<MoHinhToChuc>)from a in _context.Sys_Dm_Company
                                              where a.Id == CompanyId
                                              select new
                                              {
                                                  a.Id,
                                                  a.Name,
                                                  Loai = 0,
                                                  children = _listDepartMents.Where(x => x.CompanyId == CompanyId).Select(c => new {
                                                      c.Id,
                                                      c.Name,
                                                      c.Loai,
                                                      c.children
                                                  }).ToList()
                                              }).ToList();
            return tables;
        }
        public  static List<MoHinhToChuc> IsDepCurrentOnly(humanDbContext _context, int CompanyId, int DepartmentId)
        {

            var _listDepartMents = ((List<MoHinhToChuc>)_context.Sys_Dm_Department.Where(x => x.CompanyId == CompanyId && x.Id == DepartmentId).Select(a => new
            {
                a.Id,
                a.Name,
                a.CompanyId,
                Loai = 1,
                children = _context.Sys_Dm_Department.Where(x => x.ParentId == a.Id).Select(v => new {
                    v.Id,
                    v.Name,
                    Loai = 2
                }).ToList()
            }));
            var s = 1;
            return _listDepartMents;
        }
        public static List<MoHinhToChuc> IsNestCurrentOnly(humanDbContext _context, int CompanyId, int DepartmentId)
        {

            var _listDepartMents = ((List<MoHinhToChuc>)_context.Sys_Dm_Department.Where(x => x.CompanyId == CompanyId && x.Id == DepartmentId ).Select(a => new
            {
                a.Id,
                a.Name,
                a.CompanyId,
                Loai = 2,
                children = _context.Sys_Dm_Department.Where(x => x.ParentId == a.Id).Select(v => new {
                    v.Id,
                    v.Name,
                    Loai = 2
                }).ToList()
            })).ToList();
            return _listDepartMents;
        }
    }
}
