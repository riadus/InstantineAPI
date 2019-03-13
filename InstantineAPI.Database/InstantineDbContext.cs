using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using InstantineAPI.Data;

namespace InstantineAPI.Database
{
    public class InstantineDbContext : DbContext
    {
        public InstantineDbContext(DbContextOptions<InstantineDbContext> options) : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AlbumAdmin>()
                .HasKey(aa => new { aa.AlbumId, aa.AdminId });

            modelBuilder.Entity<AlbumFollower>()
                .HasKey(aa => new { aa.AlbumId, aa.FollowerId });
           
             modelBuilder.Entity<PhotoComment>()
                .HasKey(pc => new { pc.PhotoId, pc.CommentId });

            modelBuilder.Entity<PhotoLike>()
                .HasKey(pl => new { pl.PhotoId, pl.LikeId });
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Album> Albums { get; set; }
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<InstantineDbContext>
    {
        public InstantineDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<InstantineDbContext>();
            builder.UseSqlite("Data Source=Instantine.db");

            return new InstantineDbContext(builder.Options);
        }
    }
}
