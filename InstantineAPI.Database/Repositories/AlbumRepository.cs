using System.Linq;
using Microsoft.EntityFrameworkCore;
using InstantineAPI.Data;

namespace InstantineAPI.Database.Repositories
{
    public class AlbumRepository : BaseRepository<Album>
    {
        public AlbumRepository(InstantineDbContext instantineDbContext) : base(instantineDbContext)
        {
        }

        protected override DbSet<Album> DbSet => InstantineDbContext.Albums;

        protected override IQueryable<Album> Query =>
             InstantineDbContext.Albums
                .Include(x => x.Creator)
                .Include(x => x.AlbumAdmins)
                .Include(x => x.AlbumFollowers)
                    .ThenInclude(f => f.Album)
                .Include(x => x.AlbumFollowers)
                    .ThenInclude(f => f.Follower)
                .Include(x => x.Photos)
                    .ThenInclude(c => c.Author)
                .Include(x => x.Photos)
                    .ThenInclude(c => c.PhotoLikes)
                        .ThenInclude(l => l.Like)
                            .ThenInclude(l => l.Reactor)
                .Include(x => x.Photos)
                    .ThenInclude(c => c.PhotoComments)
                        .ThenInclude(l => l.Comment)
                            .ThenInclude(l => l.Reactor);
    }
}