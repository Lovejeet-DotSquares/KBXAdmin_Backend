using KBXAdmin.Domain.Entities;
using KBXAdmin.Infrastructure.Persistence;
using KBXAdmin.Infrastructure.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public Task<User?> GetByEmailAsync(string email)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
    }
}
