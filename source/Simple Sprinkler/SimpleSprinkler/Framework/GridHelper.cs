using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SimpleSprinkler.Framework
{
    /// <summary>Encapsulates the logic for building sprinkler grids.</summary>
    internal class GridHelper
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>
        private readonly SimpleConfig Config;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="config">The mod configuration.</param>
        public GridHelper(SimpleConfig config)
        {
            this.Config = config;
        }

        /// <summary>Get the coverage radius for a given sprinkler.</summary>
        /// <param name="sprinklerId">The sprinkler ID.</param>
        /// <returns>Returns the sprinkler coverage radius, or -1 if it's not a valid sprinkler.</returns>
        public int GetRadius(int sprinklerId)
        {
            foreach (var sprinkler in this.Config.Radius)
            {
                if (sprinklerId == sprinkler.Key)
                    return (int)sprinkler.Value;
            }

            return -1;
        }

        /// <summary>Get the tiles covered by the given sprinkler.</summary>
        /// <param name="sprinklerId">The sprinkler ID.</param>
        /// <param name="origin">The sprinkler's tile position.</param>
        public IEnumerable<Vector2> GetGrid(int sprinklerId, Vector2 origin)
        {
            // get range
            int radius = this.GetRadius(sprinklerId);
            if (radius == -1)
                return new Vector2[0];

            // get grid
            switch (Config.CalculationMethod)
            {
                case CalculationMethods.BOX:
                    return this.GetCircleOrBoxGrid(origin, radius, circle: false);
                case CalculationMethods.CIRCLE:
                    return this.GetCircleOrBoxGrid(origin, radius, circle: true);
                case CalculationMethods.HORIZONTAL:
                    return this.GetHorizontalGrid(origin, radius);
                case CalculationMethods.VERTICAL:
                    return this.CalculateVertical(origin, radius);
                default:
                    return new Vector2[0];
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get a circular or box grid.</summary>
        /// <param name="origin">The central tile position.</param>
        /// <param name="radius">The grid radius (excluding the origin).</param>
        /// <param name="circle">Whether to draw a circle; else a box.</param>
        private IEnumerable<Vector2> GetCircleOrBoxGrid(Vector2 origin, float radius, bool circle)
        {
            for (float x = origin.X - radius; x <= origin.X + radius; x++)
            {
                for (float y = origin.Y - radius; y <= origin.Y + radius; y++)
                {
                    Vector2 position = new Vector2(x, y);

                    //Circle Mode Clamp, AwayFromZero is used to get a cleaner look which creates longer outer edges
                    if (circle && Math.Round(Vector2.Distance(origin, position), MidpointRounding.AwayFromZero) > radius)
                        continue;

                    yield return new Vector2(x, y);
                }
            }
        }

        /// <summary>Get a horizontal grid.</summary>
        /// <param name="origin">The central tile position.</param>
        /// <param name="radius">The grid radius (excluding the origin).</param>
        private IEnumerable<Vector2> GetHorizontalGrid(Vector2 origin, float radius)
        {
            // horizontal tiles
            for (float x = origin.X - radius; x <= origin.X + radius; x++)
                yield return new Vector2(x, origin.Y);
        }

        /// <summary>Get a vertical grid.</summary>
        /// <param name="origin">The central tile position.</param>
        /// <param name="radius">The grid radius (excluding the origin).</param>
        private IEnumerable<Vector2> CalculateVertical(Vector2 origin, float radius)
        {
            for (float y = origin.Y - radius; y <= origin.Y + radius; y++)
                yield return new Vector2(origin.X, y);
        }
    }
}
