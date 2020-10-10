/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Paradox355/SDVMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RelationshipTracker
{
    internal static class Icons
    {
        /// <summary>Icons from Cursors tile sheet</summary>
        public static class CursorIcons
        {
            /// <summary>The sprite sheet containing the Cursors icons</summary>
            public static Texture2D Sheet => Game1.mouseCursors;

            /// <summary>Left arrow icon</summary>
            public static readonly Rectangle LeftArrow = new Rectangle(352, 495, 12, 11);

            /// <summary>Right arrow icon</summary>
            public static readonly Rectangle RightArrow = new Rectangle(365, 495, 12, 11);

            /// <summary>Full heart icon</summary>
            public static readonly Rectangle FullHeart = new Rectangle(211, 428, 7, 6);

            /// <summary>Empty heart icon</summary>
            public static readonly Rectangle EmptyHeart = new Rectangle(218, 428, 7, 6);

            /// <summary>Gift box icon</summary>
            public static readonly Rectangle Gift = new Rectangle(229, 410, 14, 14);
        }

        /// <summary>Icons from MenuTiles tile sheet</summary>
        public static class MenuIcons
        {
            /// <summary>The sprite sheet containing the MenuTiles icons</summary>
            public static Texture2D Sheet => Game1.menuTexture;

            /// <summary>Large heart icon</summary>
            public static readonly Rectangle Heart = new Rectangle(62, 770, 32, 32);

            /// <summary>Large gift box icon</summary>
            public static readonly Rectangle Gift = new Rectangle(128, 896, 44, 56);
        }

        public class Portrait
        {
            public Texture2D Image;
            public Rectangle Coords = new Rectangle(0, 0, 64, 64);
            private NPC npc;

            public Portrait(NPC npc)
            {
                this.npc = npc;
                Image = npc.Portrait;
            }

        }
    }
}
