using System;

namespace InstantineAPI.Data
{
    public class PhotoComment
    {
        public int PhotoId { get; set; }
        public int CommentId { get; set; }

        public Photo Photo { get; set; }
        public Comment Comment { get; set; }

        public override bool Equals(object obj)
        {
            return obj is PhotoComment comment &&
                   PhotoId == comment.PhotoId &&
                   CommentId == comment.CommentId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PhotoId, CommentId);
        }
    }
}
