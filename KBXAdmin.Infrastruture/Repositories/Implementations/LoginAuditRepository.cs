using KBXAdmin.Domain.Entities;
using KBXAdmin.Infrastructure.Persistence;
using KBXAdmin.Infrastructure.Repositories.Interfaces;

namespace KBXAdmin.Infrastructure.Repositories.Implementations;

public class LoginAuditLogRepository : GenericRepository<LoginAuditLog>, ILoginAuditLogRepository
{
    public LoginAuditLogRepository(AppDbContext context) : base(context) { }
}
