using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HumanResource.Data.Request;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.System;
using HumanResource.Application.Paremeters;
using HumanResoureAPI.Common;
using HumanResource.Application.Helper.Dtos;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.IO;
using HumanResource.Data.Entities.Works;

namespace HumanResoureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly humanDbContext _context;

        public CompanyController(humanDbContext context)
        {
            _context = context;
        }

        // Post: api/Company/r1GetListData
        [HttpPost]
        [Route("r1GetListData")]
        public async Task<ActionResult<IEnumerable<Sys_Dm_Company>>> r1GetListData(Options options)
        {
            try
            {
                var tables = _context.Sys_Dm_Company.Select(a => new
                {
                    a.Name,
                    a.IsActive,
                    a.Code,
                    a.CreateDate,
                    a.Id
                });
                if (!string.IsNullOrEmpty(options.s))
                {
                    tables = tables.Where(c => c.Name.ToUpper().Contains(options.s.ToUpper()));
                }
                var qrs = await tables.OrderBy(x => x.Id).Skip(options.pz * (options.p - 1)).Take(options.pz).ToListAsync();
                return new ObjectResult(new { error = 0, data = qrs, total = tables.Count() });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }

        // GET: api/Company/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sys_Dm_Company>> GetSys_Dm_Company(int id)
        {
            var sys_Dm_Company = await _context.Sys_Dm_Company.FindAsync(id);

            if (sys_Dm_Company == null)
            {
                return NotFound();
            }

            return sys_Dm_Company;
        }

        // PUT: api/Company/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSys_Dm_Company(int id, Sys_Dm_Company sys_Dm_Company)
        {
            if (id != sys_Dm_Company.Id)
            {
                return BadRequest();
            }

            _context.Entry(sys_Dm_Company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Sys_Dm_CompanyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost("r2_AddData"), DisableRequestSizeLimit]
        public async Task<ActionResult<Sys_Dm_Company>> PostSys_Dm_Company()
        {
            try
            {
                RequestToken token = CommonData.GetDataFromToken(User);
                var obj = JsonConvert.DeserializeObject<Sys_Dm_Company>(Request.Form["model"]);
                if (Request.Form.Files.Count != 0)
                {
                    foreach (var item in Request.Form.Files)
                    {
                        var file = item;
                        var folderName = Path.Combine("Resources", "Category", "Company");
                        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                        if (!Directory.Exists(pathToSave))
                        {
                            Directory.CreateDirectory(pathToSave);
                        }
                        if (file.Length > 0)
                        {
                            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            var fullPath = Path.Combine(pathToSave, fileName);
                            var dbPath = Path.Combine(folderName, fileName);
                            obj.Logo = dbPath;
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                        }
                    }

                }
                obj.CreateDate = DateTime.Now;
                obj.Creator = token.CompanyId;
                obj.LevelCom = 1;
                obj.IsActive = true;
                _context.Sys_Dm_Company.Add(obj);
                CloneNewCompany(obj.Id);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetSys_Dm_Company", new { id = obj.Id }, obj);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Clone dữ liệu sang công ty mới
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        private int CloneNewCompany(int CompanyId)
        {
            var qt_buocs = _context.VB_QT_Buoc.Where(x => x.CompanyId == 3).ToList();
            foreach (var item in qt_buocs)
            {
                item.Id = 0;
                item.CompanyId = CompanyId;
                _context.VB_QT_Buoc.Add(item);
            }
            var qt_quytrinhs = _context.VB_QT_QuyTrinh.Where(x => x.CompanyId == 3).ToList();
            foreach (var item in qt_quytrinhs)
            {
                item.Id = 0;
                item.CompanyId = CompanyId;
                _context.VB_QT_QuyTrinh.Add(item);
            }
            var dm_leveltasks = _context.CV_DM_LevelTask.Where(x => x.CompanyId == 3).ToList();
            foreach (var item in dm_leveltasks)
            {
                item.Id = 0;
                item.CompanyId = CompanyId;
                _context.CV_DM_LevelTask.Add(item);
            }
            var dm_leveltimes = _context.CV_DM_LevelTime.Where(x => x.CompanyId == 3).ToList();
            foreach (var item in dm_leveltimes)
            {
                item.Id = 0;
                item.CompanyId = CompanyId;
                _context.CV_DM_LevelTime.Add(item);
            }
            _context.SaveChanges();
            return 0;
        }
        [HttpPost]
        [Route("r4DelSys_Dm_Company")]
        public async Task<IActionResult> r4DelUsers([FromBody] List<Sys_Dm_Company> sys_Dm_Companys)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { error = 1 });
            }
            RequestToken token = CommonData.GetDataFromToken(User);
            foreach (var item in sys_Dm_Companys)
            {
                var sys_Dm_Company = _context.Sys_Dm_Company.FirstOrDefault(x => x.Id == item.Id);
                if (sys_Dm_Company == null)
                {
                    return new JsonResult(new { error = 1, ms = "Lỗi khi xóa công ty. Vui lòng thử lại!" });
                }

                _context.Sys_Dm_Company.Remove(sys_Dm_Company);
            }
            await _context.SaveChangesAsync();
            return new JsonResult(new { error = 0 });
        }
        private bool Sys_Dm_CompanyExists(int id)
        {
            return _context.Sys_Dm_Company.Any(e => e.Id == id);
        }
    }
}
