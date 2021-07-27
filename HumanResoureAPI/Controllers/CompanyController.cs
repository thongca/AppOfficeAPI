using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.System;
using HumanResource.Application.Paremeters;
using HumanResoureAPI.Common;
using HumanResource.Application.Helper.Dtos;

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
                var tables = _context.Sys_Dm_Company.Select(a => new {
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
                return new ObjectResult(new { error = 0, data = qrs, total= tables.Count()});
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

        // POST: api/Company
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Sys_Dm_Company>> PostSys_Dm_Company(Sys_Dm_Company sys_Dm_Company)
        {
            _context.Sys_Dm_Company.Add(sys_Dm_Company);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSys_Dm_Company", new { id = sys_Dm_Company.Id }, sys_Dm_Company);
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
