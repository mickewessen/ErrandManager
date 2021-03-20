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
using Errand.Api.Services;

namespace Errand.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssuesController : ControllerBase
    {
        private readonly SqlDbContext _context;
        private readonly ISearchService _services;

        public IssuesController(SqlDbContext context, ISearchService services)
        {
            _context = context;
            _services = services;
        }

        // GET: All Issues
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Issues>>> GetIssues()
        {
            return await _context.Errands.Include(i => i.AppUser).ToListAsync();
        }

        // GET: Issues by Id
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

        //Search by Status
        [HttpGet("searchstatus/{status}")]
        public async Task<ActionResult<Issues>> SearchStatus(string status)
        {
            try
            {
                var result = await _services.SearchStatusAsync(status);

                if (result.Any())
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        //Seach by customers firstname
        [HttpGet("searchcustomer/{firstname}")]
        public async Task<ActionResult<Issues>> SearchCustomer(string firstname)
        {
            try
            {
                var result = await _services.SearchCustomerAsync(firstname);

                if (result.Any())
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        //Seach by created date
        [HttpGet("searchcreateddate/{created}")]
        public async Task<ActionResult<Issues>> SearchCreatedDate(string created)
        {
            try
            {
                var result = await _services.SearchCreatedDateAsync(created);

                return Ok(result);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        // PUT: Edit Issue by Id
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

        // POST: Create Issues
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

        // DELETE: Delete Issues
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
