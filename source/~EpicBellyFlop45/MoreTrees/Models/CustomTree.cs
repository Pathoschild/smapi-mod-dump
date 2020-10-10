/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace MoreTrees.Models
{
    /// <summary>Represents a loaded tree.</summary>
    public class CustomTree
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The type of tree.</summary>
        public int Type { get; set; }

        /// <summary>The name of the tree.</summary>
        public string Name { get; set; }

        /// <summary>The data for the tree.</summary>
        public TreeData Data { get; set; }

        /// <summary>The texture for the tree.</summary>
        public Texture2D Texture { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="type">The type of the tree.</param>
        /// <param name="name">The name of the tree.</param>
        /// <param name="data">The data for the tree.</param>
        /// <param name="texture">The texture for the tree.</param>
        public CustomTree(int type, string name, TreeData data, Texture2D texture)
        {
            Type = type;
            Name = name;
            Data = data;
            Texture = texture;
        }
    }
}
