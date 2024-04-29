using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Infrastructure.Data;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Captivlink.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApplicationUser?> GetUserById(Guid userId)
        {
            return await _dbContext.Users.Include(x => x.Company).Include(x => x.Person).ThenInclude(x => x !=null ? x.Categories : null).FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<ApplicationUser> UpdateAsync(ApplicationUser user)
        {
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }
    }
}
