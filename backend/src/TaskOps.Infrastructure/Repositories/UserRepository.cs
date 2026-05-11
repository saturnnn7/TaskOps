using Microsoft.EntityFrameworkCore;
using TaskOps.Domain.Entities;
using TaskOps.Domain.Interfaces;
using TaskOps.Infrastructure.Persistence;

namespace TaskOps.Infrastructure.Repositories;

public sealed class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) {}

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await DbSet.FirstOrDefaultAsync(u => u.Email == email.ToLower(), cancellationToken);
    
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(u => u.Email == email.ToLower(), cancellationToken);
}