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
            return new UnitOfWork(new PhotosRepository(new InstantineDbContextBuilder().Build()),
                              new CommentsRepository(new InstantineDbContextBuilder().Build()),
                              new LikesRepository(new InstantineDbContextBuilder().Build()),
                              new UsersRepository(new InstantineDbContextBuilder().Build()),
                              new AlbumRepository(new InstantineDbContextBuilder().Build()));
        }
    }
}
