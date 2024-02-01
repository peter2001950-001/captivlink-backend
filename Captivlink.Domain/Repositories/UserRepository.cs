using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Infrastructure.Data;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Repositories.Contracts;

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
            return _dbContext.Users.FirstOrDefault(x => x.Id == userId);
        }
    }
}
