using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBXAdmin.Domain.Entities.Admin
{
    public class FormVersion
    {
        public Guid Id { get; set; }
        public Guid FormId { get; set; }
        public string SchemaJson { get; set; } = "{}";

        public string CreatedBy { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public FormEntity Form { get; set; } = null!;
    }
}
