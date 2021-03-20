using SharedLibraries.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibraries.Models
{
    public class EditIssueModel
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime? ResolveDate { get; set; }
        public string Status { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerEmail { get; set; }
        public int AppUserId { get; set; }
        public virtual AppUser AppUser { get; set; }
    }
}
