using Errand.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedLibraries.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Errand.Api.Services
{
    public class SearchService : ISearchService
    {
        private readonly SqlDbContext _context;
        private IConfiguration _configuration { get; }

        public SearchService(SqlDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
       

        public async Task<IEnumerable<Issues>> SearchCreatedDateAsync(string created)
        {
            IQueryable<Issues> result = _context.Errands;

            if (DateTime.TryParse(created, out DateTime pdatetime))
            {
                result = result.Where(x => x.CreateDate > pdatetime);
            }
            else if (created == "latest")
            {
                result = result.OrderByDescending(x => x.CreateDate);
            }

            return await result.ToListAsync();
        }

        public async Task<IEnumerable<Issues>> SearchCustomerAsync(string firstname)
        {
            IQueryable<Issues> query = _context.Errands;

            if (!string.IsNullOrEmpty(firstname))
            {
                query = query.Where(e => e.CustomerFirstName.Contains(firstname));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Issues>> SearchStatusAsync(string status)
        {
            IQueryable<Issues> query = _context.Errands;

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(e => e.Status.Contains(status));
            }

            return await query.ToListAsync();
        }
    }
}
