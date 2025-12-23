using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBXAdmin.Domain.Entities.Admin
{
    public class FormEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string SchemaJson { get; set; } = "{}";
        public string Status { get; set; } = "DRAFT";

        public string? LockedBy { get; set; }
        public DateTime? LockedAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<FormVersion> Versions { get; set; } = new List<FormVersion>();
    }
}
