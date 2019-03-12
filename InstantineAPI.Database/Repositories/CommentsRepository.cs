using System.Linq;
using Microsoft.EntityFrameworkCore;
using InstantineAPI.Data;

namespace InstantineAPI.Database.Repositories
{
    public class CommentsRepository : BaseRepository<Comment>
    {
        public CommentsRepository(InstantineDbContext instantineDbContext) : base(instantineDbContext)
        {
        }

        protected override DbSet<Comment> DbSet => InstantineDbContext.Comments;

        protected override IQueryable<Comment> Query => InstantineDbContext.Comments
                .Include(x => x.Reactor);
    }
}
