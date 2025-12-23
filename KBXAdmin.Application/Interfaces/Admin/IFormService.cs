using KBXAdmin.Domain.Entities.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBXAdmin.Application.Interfaces.Admin
{
    public interface IFormService
    {
        Task<object> GetFormsAsync(int page, int pageSize, string search);
        Task<FormEntity?> GetByIdAsync(Guid id);

        Task<FormEntity> CreateAsync(string title, string userId);
        Task SaveDraftAsync(Guid id, string schemaJson, string userId);
        Task AutoSaveAsync(Guid id, string schemaJson, string userId);

        Task StartSessionAsync(Guid id, string userId);
        Task EndSessionAsync(Guid id, string userId);

        Task<IEnumerable<FormVersion>> GetVersionsAsync(Guid id);
        Task RestoreVersionAsync(Guid formId, Guid versionId, string userId);

        Task PublishAsync(Guid id, string userId);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<FormAuditLog>> GetAuditLogsAsync(Guid formId);

    }
}
