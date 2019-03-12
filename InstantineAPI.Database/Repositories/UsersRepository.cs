using System.Linq;
using Microsoft.EntityFrameworkCore;
using InstantineAPI.Data;

namespace InstantineAPI.Database.Repositories
{
    public class UsersRepository : BaseRepository<User>
    {
        public UsersRepository(InstantineDbContext instantineDbContext) : base(instantineDbContext)
        {
        }

        protected override DbSet<User> DbSet => InstantineDbContext.Users;

        protected override IQueryable<User> Query => InstantineDbContext.Users;
    }
}
