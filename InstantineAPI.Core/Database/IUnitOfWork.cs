using InstantineAPI.Data;

namespace InstantineAPI.Core.Database
{
    public interface IUnitOfWork
    {
        IRepository<Photo> Photos { get; }
        IRepository<Comment> Comments { get; }
        IRepository<Like> Likes { get; }
        IRepository<User> Users { get; }
        IRepository<Album> Albums { get; }
    }
}
