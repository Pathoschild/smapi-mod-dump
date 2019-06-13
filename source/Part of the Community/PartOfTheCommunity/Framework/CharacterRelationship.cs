namespace PartOfTheCommunity.Framework
{
    /// <summary>Tracked data for an NPC relationship.</summary>
    internal class CharacterRelationship
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The target character.</summary>
        public CharacterInfo Character { get; set; }

        /// <summary>The target character's relationship to the original character (like 'Mother').</summary>
        public Relationship Relationship { get; }

        /// <summary>Whether this is a friend (non-family) relationship.</summary>
        public bool IsFriend => this.Relationship == Relationship.Friend;

        /// <summary>Whether this is a family relationship.</summary>
        public bool IsFamily => !this.IsFriend && this.Relationship != Relationship.WarTorn;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="relationship">The target character's relationship to the original character (like 'Mother').</param>
        /// <param name="character">The target character.</param>
        public CharacterRelationship(Relationship relationship, CharacterInfo character)
        {
            this.Relationship = relationship;
            this.Character = character;
        }
    }
}
