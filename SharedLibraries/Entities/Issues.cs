using System;
using System.Collections.Generic;

#nullable disable

namespace SharedLibraries.Entities
{
    public partial class Issues
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ResolveDate { get; set; }
        public string Status { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerEmail { get; set; }
        public int AppUserId { get; set; }

        public virtual AppUser AppUser { get; set; }
    }
}
