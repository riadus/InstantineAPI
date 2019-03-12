using InstantineAPI.Core.Database;
using InstantineAPI.Database;
using InstantineAPI.Database.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace InstantineAPI.UnitTests.Builders
{
    public class InstantineDbContextBuilder
    {
        public InstantineDbContext Build()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<InstantineDbContext>()
                .UseSqlite(connection)
                .EnableSensitiveDataLogging()
                .Options;

            return new InstantineDbContext(options);
        }
    }
    public class UnitOfWorkBuilder
    {
        public IUnitOfWork Build()
        {
            var dbContext = new InstantineDbContextBuilder().Build();
            return new UnitOfWork(new PhotosRepository(dbContext),
                              new CommentsRepository(dbContext),
                              new LikesRepository(dbContext),
                              new UsersRepository(dbContext),
                              new AlbumRepository(dbContext));
        }
    }
}
