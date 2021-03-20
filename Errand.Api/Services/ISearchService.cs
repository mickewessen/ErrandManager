using SharedLibraries.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Errand.Api.Services
{
    public interface ISearchService
    {
        Task<IEnumerable<Issues>> SearchStatusAsync(string status);
        Task<IEnumerable<Issues>> SearchCustomerAsync(string firstname);
        Task<IEnumerable<Issues>> SearchCreatedDateAsync(string created);
    }
}
