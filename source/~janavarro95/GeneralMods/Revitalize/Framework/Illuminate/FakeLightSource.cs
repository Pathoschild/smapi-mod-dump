/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Revitalize.Framework.Illuminate
{
    /// <summary>
    /// Deals with recreating light sources in SDV.
    /// </summary>
    public class FakeLightSource
    {
        /// <summary>
        /// The id for the light. Refers to the type of texture used.
        /// </summary>
        public int id;
        /// <summary>
        /// The position offset from the source object this is attached to.
        /// </summary>
        public Vector2 positionOffset;
        /// <summary>
        /// The color for the light.
        /// </summary>
        public Color color;
        /// <summary>
        /// The radius for the light.
        /// </summary>
        public float radius;

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public FakeLightSource()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ID">The id for the light source.</param>
        /// <param name="Position">The position for the light source.</param>
        /// <param name="Color">The color for the light.</param>
        /// <param name="Raidus">The radius for the light.</param>
        public FakeLightSource(int ID, Vector2 Position, Color Color, float Raidus)
        {
            this.id = ID;
            this.positionOffset = Position;
            this.color = Color;
            this.radius = Raidus;
        }

        /// <summary>
        /// Gets a copy of the fake light source.
        /// </summary>
        /// <returns></returns>
        public FakeLightSource Copy()
        {
            return new FakeLightSource(this.id, new Vector2(this.positionOffset.X, this.positionOffset.Y), new Color(this.color.R, this.color.G, this.color.B, this.color.A), this.radius);
        }

    }
}
