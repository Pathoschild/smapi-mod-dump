/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Survivalistic
**
*************************************************/

using StardewModdingAPI;
using Microsoft.Xna.Framework.Graphics;

namespace Survivalistic.Framework.Common
{
    public class Textures
    {
        public static Texture2D hunger_sprite,
            thirst_sprite,
            filler_sprite;

        public static void LoadTextures()
        {
            hunger_sprite = ModEntry.instance.Helper.ModContent.Load<Texture2D>("assets/Bars/Hunger_Sprite.png");
            thirst_sprite = ModEntry.instance.Helper.ModContent.Load<Texture2D>("assets/Bars/Thirst_Sprite.png");
            filler_sprite = ModEntry.instance.Helper.ModContent.Load<Texture2D>("assets/Bars/Filler_Sprite.png");
        }
    }
}
