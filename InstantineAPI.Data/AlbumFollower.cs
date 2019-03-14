using System;

namespace InstantineAPI.Data
{
    public class AlbumFollower
    {
        public int AlbumId { get; set; }
        public int FollowerId { get; set; }

        public Album Album { get; set; }
        public User Follower { get; set; }

        public override bool Equals(object obj)
        {
            return obj is AlbumFollower follower &&
                   AlbumId == follower.AlbumId &&
                   FollowerId == follower.FollowerId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AlbumId, FollowerId);
        }
    }
}
