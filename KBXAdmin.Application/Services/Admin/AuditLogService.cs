using KBXAdmin.Application.Interfaces.Admin;
using KBXAdmin.Domain.Entities.Admin;
using KBXAdmin.Infrastructure.Persistence;

namespace KBXAdmin.Application.Services.Admin
{
    public class AuditLogService : IAuditLogService
    {
        private readonly AppDbContext _db;

        public AuditLogService(AppDbContext db)
        {
            _db = db;
        }

        public async Task LogAsync(
            Guid formId,
            string action,
            string userId,
            string role,
            string? metadata = null)
        {
            _db.FormAuditLogs.Add(new FormAuditLog
            {
                Id = Guid.NewGuid(),
                FormId = formId,
                Action = action,
                PerformedBy = userId,
                Role = role,
                Metadata = metadata,
                PerformedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
        }
    }

}
