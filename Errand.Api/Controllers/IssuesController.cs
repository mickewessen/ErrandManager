using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Errand.Api.Data;
using SharedLibraries.Models;
using SharedLibraries.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Errand.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssuesController : ControllerBase
    {
        private readonly SqlDbContext _context;

        public IssuesController(SqlDbContext context)
        {
            _context = context;
        }

        // GET: api/Issues
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Issues>>> GetIssues()
        {
            return await _context.Errands.Include(i => i.AppUser).ToListAsync();
        }

        // GET: api/Issues/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Issues>> GetIssues(int id)
        {
            var issues = await _context.Errands.FindAsync(id);

            if (issues == null)
            {
                return NotFound();
            }

            return issues;
        }

        // PUT: api/Issues/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIssues(int id, EditIssueModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var issue = await _context.Errands.FindAsync(id);
            issue.Status = model.Status;
            issue.Description = model.Description;
            issue.Category = model.Category;
            issue.CustomerFirstName = model.CustomerFirstName;
            issue.CustomerLastName = model.CustomerLastName;
            issue.AppUserId = model.AppUserId;

            if (model.Status.Contains("Completed"))
            {
                issue.ResolveDate = DateTime.Now;
            }
            else
                issue.ResolveDate = null;

            _context.Entry(issue).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IssuesExists(id))
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

        // POST: api/Issues
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Issues>> PostIssues(CreateIssueModel model)
        {
            var issue = new Issues()
            {
                Description = model.Description,
                Category = model.Category,
                CustomerEmail = model.CustomerEmail,
                CustomerFirstName = model.CustomerFirstName,
                CustomerLastName = model.CustomerLastName,
                Status = model.Status,
                AppUserId = int.Parse(model.AppUserId),
                CreateDate = DateTime.Now
            };

            _context.Errands.Add(issue);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetIssues", new { id = issue.Id }, issue);
        }

        // DELETE: api/Issues/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIssues(int id)
        {
            var issues = await _context.Errands.FindAsync(id);
            if (issues == null)
            {
                return NotFound();
            }

            _context.Errands.Remove(issues);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IssuesExists(int id)
        {
            return _context.Errands.Any(e => e.Id == id);
        }
    }
}
