/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/ParticleFramework
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace ParticleFramework.Framework.Data
{
    /// <summary>Represents particle effect data.</summary>
    public class ParticleEffectData
    {
        /// <summary>The unique key for the particle effect</summary>
        public string key;

        /// <summary>The type of thing to target; one of "object", "furniture", "npc", "location", "hat", "shirt", "boots", "pants", "tool", "ring"</summary>
        public string type;

        /// <summary>The QualifiedItemId or Name of the target</summary>
        public string name;

        /// <summary>Should the particles follow the farmer or npc when moving</summary>
        public bool follow = false;

        /// <summary>How particles move; one of "none", "away", "towards", "up", "down", "left", "right", "random"</summary>
        public string movementType = "none";

        /// <summary>How many pixels a particle moves per tick</summary>
        public float movementSpeed;

        /// <summary>How many pixels/tick the movement speed increases per tick</summary>
        public float acceleration;

        /// <summary>How many animation frames change per tick (see textures below)</summary>
        public float frameSpeed;

        /// <summary>Whether particles disappear when they move outside the field (default false)</summary>
        public bool restrictOuter;

        /// <summary>Whether particles disappear when they move inside the inner boundary (default false)</summary>
        public bool restrictInner;

        /// <summary>If set to > 0, allows particles to move behind the thing by the given layer depth offset (default -1)</summary>
        public float belowOffset = -1;

        /// <summary>If set to > 0, allows particles to move in front of the thing by the given layer depth offset (default 0.001)</summary>
        public float aboveOffset = 0.001f;

        /// <summary>If set to > 0, allows particles to rotate around their center</summary>
        public float minRotationRate;

        /// <summary>If set to > 0, allows particles to rotate around their center</summary>
        public float maxRotationRate;

        /// <summary>Allows transparency (default: 1)</summary>
        public float minAlpha = 1;

        /// <summary>Allows transparency (default: 1)</summary>
        public float maxAlpha = 1;

        /// <summary>The width of each particle on the sprite sheet</summary>
        public int particleWidth;

        /// <summary>The height of each particle on the sprite sheet</summary>
        public int particleHeight;

        /// <summary>If set to > 0, specifies a circular particle spawn field and sets the outer radius of the field</summary>
        public float fieldOuterRadius;

        /// <summary>If fieldOuterRadius is set to 0, designates the outer width of the rectangular particle field</summary>
        public int fieldOuterWidth;

        /// <summary>If fieldOuterRadius is set to 0, designates the outer height of the rectangular particle field</summary>
        public int fieldOuterHeight;

        /// <summary>If set to > 0, sets the inner radius of the field</summary>
        public float fieldInnerRadius;

        /// <summary>If set to > 0, sets the inner width of the rectangular particle field</summary>
        public int fieldInnerWidth;

        /// <summary>If set to > 0, sets the inner height of the rectangular particle field</summary>
        public int fieldInnerHeight;

        /// <summary>For locations, specifies the X coordinate of the field center on the map; for everything else, specifies the field X offset from the thing's center</summary>
        public int fieldOffsetX;

        /// <summary>For locations, specifies the Y coordinate of the field center on the map; for everything else, specifies the field Y offset from the thing's center</summary>
        public int fieldOffsetY = -32;

        /// <summary>Sets the maximum number of simultaneous particles in a field</summary>
        public int maxParticles = 1;

        /// <summary>Minimum number of ticks a particle lives</summary>
        public int minLifespan = 1;

        /// <summary>Maximum number of ticks a particle lives</summary>
        public int maxLifespan = 1;

        /// <summary>Minimum scale of each particle in the field</summary>
        public float minParticleScale = 4;

        /// <summary>Maximum scale of each particle in the field</summary>
        public float maxParticleScale = 4;

        /// <summary>The chance (0 to 1) that a new particle will be created, checked each tick</summary>
        public float particleChance = 1;

        /// <summary>The path to this particle's spritesheet.</summary>
        public string spriteSheetPath;

        /// <summary>The sprite sheet used by the effect.</summary>
        public Texture2D spriteSheet;
    }
}
