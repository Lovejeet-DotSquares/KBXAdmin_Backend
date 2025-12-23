using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBXAdmin.Domain.Entities.Admin
{
    public class FormAuditLog
    {
        public Guid Id { get; set; }
        public Guid FormId { get; set; }

        public string Action { get; set; } = "";
        // AUTOSAVE, SAVE_DRAFT, PUBLISH, RESTORE, DELETE

        public string PerformedBy { get; set; } = "";
        public string Role { get; set; } = "";

        public DateTime PerformedAt { get; set; }
        public string? Metadata { get; set; }

        public FormEntity Form { get; set; } = null!;
    }
}
