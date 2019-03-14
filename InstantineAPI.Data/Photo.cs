using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace InstantineAPI.Data
{
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
}
