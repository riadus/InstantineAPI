using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace InstantineAPI.Data
{
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
}
