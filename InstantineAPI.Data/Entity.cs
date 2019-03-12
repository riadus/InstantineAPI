using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace InstantineAPI.Data
{
    public class Entity
    {
        public int Id { get; set; }
    }

    public class Album : Entity
    {
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public User Creator { get; set; }
        [JsonIgnore]
        public List<AlbumAdmin> AlbumAdmins { get; set; }
        [JsonIgnore]
        public List<AlbumFollower> AlbumFollowers { get; set; }
        public string AlbumId { get; set; }
        public List<Photo> Photos { get; set; }
        public Photo Cover { get; set; }

        [NotMapped]
        public IEnumerable<User> Admins => AlbumAdmins?.Select(x => x.Admin);
        [NotMapped]
        public IEnumerable<User> Followers => AlbumFollowers?.Select(x => x.Follower);
    }

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

    public class User : Entity
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool InvitationSent { get; set; }
        public bool InvitationAccepted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime SendingDate { get; set; }
        public DateTime AcceptingDate { get; set; }
        public string UserId { get; set; }
        [JsonIgnore]
        public string Code { get; set; }

        public override bool Equals(object obj)
        {
            return obj is User user &&
                   UserId == user.UserId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Code);
        }
    }

    public class Photo : Entity
    {
        public string Link { get; set; }
        public DateTime TakeDate { get; set; }
        public string PhotoId { get; set; }

        public User Author { get; set; }

        [JsonIgnore]
        public List<PhotoLike> PhotoLikes { get; set; }
        [JsonIgnore]
        public List<PhotoComment> PhotoComments { get; set; }

        [NotMapped]
        public IEnumerable<Like> Likes => PhotoLikes?.Select(x => x.Like);
        [NotMapped]
        public IEnumerable<Comment> Comments => PhotoComments?.Select(x => x.Comment);

        public override bool Equals(object obj)
        {
            return obj is Photo photo &&
                   PhotoId == photo.PhotoId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PhotoId);
        }
    }

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

    public class Reaction : Entity
    {
        public string ReactionId { get; set; }
        public User Reactor { get; set; }
        public DateTime ReactionDate { get; set; }
    }

    public class Like : Reaction
    {
        public override bool Equals(object obj)
        {
            return obj is Like like &&
                   ReactionId == like.ReactionId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ReactionId);
        }
    }

    public class Comment : Reaction
    {
        public string Text { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Comment comment &&
                   ReactionId == comment.ReactionId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ReactionId);
        }
    }

    public class EmailObject
    {
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public string DisplayName { get; set; }
        public byte[] QRCode { get; set; }
    }
}
