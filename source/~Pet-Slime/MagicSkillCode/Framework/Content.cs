/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using MagicSkillCode.Core;

namespace MagicSkillCode.Framework
{
    public class Content
    {
        /*********
        ** Public methods
        *********/
        public static Texture2D LoadTexture(string path)
        {
            return ModEntry.Instance.Helper.ModContent.Load<Texture2D>($"Assets/{path}");
        }

        public static string LoadTextureKey(string path)
        {
            return ModEntry.Instance.Helper.ModContent.GetInternalAssetName($"Assets/{path}").BaseName;
        }
    }
}
