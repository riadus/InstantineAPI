using System;

namespace InstantineAPI.Data
{
    public class PhotoLike
    {
        public int PhotoId { get; set; }
        public int LikeId { get; set; }

        public Photo Photo { get; set; }
        public Like Like { get; set; }

        public override bool Equals(object obj)
        {
            return obj is PhotoLike like &&
                   PhotoId == like.PhotoId &&
                   LikeId == like.LikeId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PhotoId, LikeId);
        }
    }
}
