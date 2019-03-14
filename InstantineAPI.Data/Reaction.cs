using System;

namespace InstantineAPI.Data
{
    public class Reaction : Entity
    {
        public string ReactionId { get; set; }
        public User Reactor { get; set; }
        public DateTime ReactionDate { get; set; }
    }
}
