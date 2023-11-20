using Manager.Domain.Entities;
using Manager.Infra.Context;
using Manager.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Manager.Infra.Repositories
{
    public class UserRepository(ManagerContext context) : BaseRepository<User>(context), IUserRepository
    {
        private readonly ManagerContext _context = context;

        public async Task<User?> GetByEmailAsync(string email)
            => await _context.Set<User>().AsNoTracking().FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());

        public async Task<List<User>> SearchByEmailAsync(string email)
            => await _context.Users.AsNoTracking().Where(x => x.Email.ToLower().Contains(email.ToLower())).ToListAsync();

        public async Task<List<User>> SearchByNameAsync(string name)
            => await _context.Users.AsNoTracking().Where(x => x.Name.ToLower().Contains(name.ToLower())).ToListAsync();
    }
}
