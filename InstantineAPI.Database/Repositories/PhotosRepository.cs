using System.Linq;
using Microsoft.EntityFrameworkCore;
using InstantineAPI.Data;

namespace InstantineAPI.Database.Repositories
{
    public class PhotosRepository : BaseRepository<Photo>
    {
        public PhotosRepository(InstantineDbContext instantineDbContext) : base(instantineDbContext)
        {
        }

        protected override DbSet<Photo> DbSet => InstantineDbContext.Photos;

        protected override IQueryable<Photo> Query =>
             InstantineDbContext.Photos
            .Include(x => x.Author)
            .Include(x => x.PhotoComments)
                .ThenInclude(c => c.Comment)
                    .ThenInclude(c => c.Reactor)
            .Include(x => x.PhotoLikes)
                .ThenInclude(c => c.Like)
                    .ThenInclude(l => l.Reactor);
    }
}