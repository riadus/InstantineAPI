using System;

namespace InstantineAPI.Data
{
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
}
