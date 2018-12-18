using Microsoft.Xna.Framework;

namespace Omegasis.SaveAnywhere.Framework.Models
{
    /// <summary>Represents saved data for an NPC.</summary>
    public class CharacterData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The character type.</summary>
        public CharacterType Type { get; set; }

        /// <summary>The character name.</summary>
        public string Name { get; set; }

        /// <summary>The map name.</summary>
        public string Map { get; set; }

        /// <summary>The X position.</summary>
        public int X { get; set; }

        /// <summary>The Y position.</summary>
        public int Y { get; set; }

        /// <summary>The direction the character is facing.</summary>
        public int FacingDirection { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <remarks>This default constructor is needed by Json.NET.</remarks>
        public CharacterData() { }

        /// <summary>Construct an instance.</summary>
        /// <param name="type">The character type.</param>
        /// <param name="name">The character name.</param>
        /// <param name="map">The map name.</param>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <param name="facingDirection">The direction the character is facing.</param>
        public CharacterData(CharacterType type, string name, string map, int x, int y, int facingDirection)
        {
            this.Type = type;
            this.Name = name;
            this.Map = map;
            this.X = x;
            this.Y = y;
            this.FacingDirection = facingDirection;
        }

        /// <summary>Construct an instance.</summary>
        /// <param name="type">The character type.</param>
        /// <param name="name">The character name.</param>
        /// <param name="map">The map name.</param>
        /// <param name="tile">The tile position.</param>
        /// <param name="facingDirection">The direction the character is facing.</param>
        public CharacterData(CharacterType type, string name, string map, Point tile, int facingDirection)
        : this(type, name, map, tile.X, tile.Y, facingDirection) { }
    }
}
