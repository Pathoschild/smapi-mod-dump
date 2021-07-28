/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace Magic.Framework
{
    internal class Content
    {
        /*********
        ** Public methods
        *********/
        public static Texture2D LoadTexture(string path)
        {
            return Mod.Instance.Helper.Content.Load<Texture2D>($"assets/{path}");
        }

        public static string LoadTextureKey(string path)
        {
            return Mod.Instance.Helper.Content.GetActualAssetKey($"assets/{path}");
        }
    }
}
