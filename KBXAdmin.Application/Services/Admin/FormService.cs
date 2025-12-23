using KBXAdmin.Application.Interfaces.Admin;
using KBXAdmin.Domain.Entities.Admin;
using KBXAdmin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace KBXAdmin.Application.Services.Admin
{
    public class FormService : IFormService
    {
        private readonly AppDbContext _db;
        private readonly IAuditLogService _audit;

        public FormService(AppDbContext db, IAuditLogService audit)
        {
            _db = db;
            _audit = audit;
        }

        public async Task<object> GetFormsAsync(int page, int pageSize, string search)
        {
            var query = _db.Forms.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(f => f.Title.Contains(search));

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(f => f.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new {
                    f.Id,
                    f.Title,
                    f.Status,
                    f.LockedBy,
                    f.UpdatedAt
                })
                .ToListAsync();

            return new { items, total };
        }

        public async Task<FormEntity?> GetByIdAsync(Guid id) =>
            await _db.Forms.FindAsync(id);

        public async Task<FormEntity> CreateAsync(string title, string userId)
        {
            var form = new FormEntity
            {
                Id = Guid.NewGuid(),
                Title = title,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Forms.Add(form);
            await _db.SaveChangesAsync();

            await _audit.LogAsync(
                form.Id,
                "CREATE",
                userId,
                "User",
                $"Title: {title}"
            );

            return form;
        }

        public async Task SaveDraftAsync(Guid id, string schemaJson, string userId)
        {
            var form = await _db.Forms.FindAsync(id)
                ?? throw new Exception("Form not found");

            form.SchemaJson = schemaJson;
            form.UpdatedAt = DateTime.UtcNow;

            _db.FormVersions.Add(new FormVersion
            {
                Id = Guid.NewGuid(),
                FormId = id,
                SchemaJson = schemaJson,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();

            await _audit.LogAsync(
                id,
                "SAVE_DRAFT",
                userId,
                "User"
            );
        }

        public async Task AutoSaveAsync(Guid id, string schemaJson, string userId)
        {
            var form = await _db.Forms.FindAsync(id)
                ?? throw new Exception("Form not found");

            form.SchemaJson = schemaJson;
            form.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await _audit.LogAsync(
                id,
                "AUTOSAVE",
                userId,
                "User"
            );
        }


        public async Task StartSessionAsync(Guid id, string userId)
        {
            var form = await _db.Forms.FindAsync(id)
                ?? throw new Exception("Form not found");

            if (form.LockedBy != null && form.LockedBy != userId)
                throw new Exception("Form locked");

            form.LockedBy = userId;
            form.LockedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        public async Task EndSessionAsync(Guid id, string userId)
        {
            var form = await _db.Forms.FindAsync(id);
            if (form?.LockedBy == userId)
            {
                form.LockedBy = null;
                form.LockedAt = null;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<FormVersion>> GetVersionsAsync(Guid id) =>
            await _db.FormVersions
                .Where(v => v.FormId == id)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();

        public async Task RestoreVersionAsync(Guid formId, Guid versionId, string userId)
        {
            var version = await _db.FormVersions.FindAsync(versionId)
                ?? throw new Exception("Version not found");

            var form = await _db.Forms.FindAsync(formId)
                ?? throw new Exception("Form not found");

            form.SchemaJson = version.SchemaJson;
            form.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            await _audit.LogAsync(
                formId,
                "RESTORE_VERSION",
                userId,
                "Admin",
                $"VersionId: {versionId}"
            );
        }

        public async Task PublishAsync(Guid id, string userId)
        {
            var form = await _db.Forms.FindAsync(id)
                ?? throw new Exception("Form not found");

            form.Status = "PUBLISHED";
            form.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            await _audit.LogAsync(
                id,
                "PUBLISH",
                userId,
                "Admin"
            );
        }


        public async Task DeleteAsync(Guid id)
        {
            var form = await _db.Forms.FindAsync(id);
            if (form != null)
            {
                _db.Forms.Remove(form);
                await _db.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<FormAuditLog>> GetAuditLogsAsync(Guid formId)
        {
            return await _db.FormAuditLogs
                .Where(a => a.FormId == formId)
                .OrderByDescending(a => a.PerformedAt)
                .ToListAsync();
        }
    }
}
