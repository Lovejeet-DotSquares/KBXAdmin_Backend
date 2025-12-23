using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBXAdmin.Application.Interfaces.Admin
{
    public interface IAuditLogService
    {
        Task LogAsync(
            Guid formId,
            string action,
            string userId,
            string role,
            string? metadata = null
        );
    }   
}
