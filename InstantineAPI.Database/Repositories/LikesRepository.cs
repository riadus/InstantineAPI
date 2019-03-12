using System.Linq;
using Microsoft.EntityFrameworkCore;
using InstantineAPI.Data;

namespace InstantineAPI.Database.Repositories
{
    public class LikesRepository : BaseRepository<Like>
    {
        public LikesRepository(InstantineDbContext instantineDbContext) : base(instantineDbContext)
        {
        }

        protected override DbSet<Like> DbSet => InstantineDbContext.Likes;

        protected override IQueryable<Like> Query => InstantineDbContext.Likes
                .Include(x => x.Reactor);
    }
}
