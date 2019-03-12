using InstantineAPI.Core.Database;
using InstantineAPI.Data;

namespace InstantineAPI.Database
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IRepository<Photo> photos,
                          IRepository<Comment> comments,
                          IRepository<Like> likes,
                          IRepository<User> users,
                          IRepository<Album> albums)
        {
            Photos = photos;
            Comments = comments;
            Likes = likes;
            Users = users;
            Albums = albums;
        }

        public IRepository<Photo> Photos { get; }

        public IRepository<Comment> Comments { get; }

        public IRepository<Like> Likes { get; }

        public IRepository<User> Users { get; }

        public IRepository<Album> Albums { get; }
    }
}
