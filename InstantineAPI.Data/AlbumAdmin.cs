using System;

namespace InstantineAPI.Data
{
    public class AlbumAdmin
    {
        public int AlbumId { get; set; }
        public int AdminId { get; set; }

        public Album Album { get; set; }
        public User Admin { get; set; }

        public override bool Equals(object obj)
        {
            return obj is AlbumAdmin admin &&
                   AlbumId == admin.AlbumId &&
                   AdminId == admin.AdminId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AlbumId, AdminId);
        }
    }
}
