using System;

namespace InstantineAPI.Data
{
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
}
