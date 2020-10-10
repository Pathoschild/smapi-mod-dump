/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace Pong.Framework.Common
{
    internal static class AssetManager
    {
        public static Texture2D SquareTexture;
        public static Texture2D CircleTexture;

        public static bool Init(IModHelper helper)
        {
            try
            {
                SquareTexture = helper.Content.Load<Texture2D>("assets/square.png");
                CircleTexture = helper.Content.Load<Texture2D>("assets/circle.png");

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
