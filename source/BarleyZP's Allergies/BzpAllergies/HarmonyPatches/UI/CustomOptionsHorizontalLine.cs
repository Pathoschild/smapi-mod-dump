/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace BZP_Allergies.HarmonyPatches.UI
{
    internal class CustomOptionsHorizontalLine : OptionsElement
    {
        public CustomOptionsHorizontalLine() : base(string.Empty)
        {
        }

        public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
        {
            if (context != null)
            {
                object[] drawPartitionArgs = { b, this.bounds.Y + slotY, true, -1, -1, -1 };
                Traverse.Create(context).Method("drawHorizontalPartition", drawPartitionArgs).GetValue(drawPartitionArgs);
            }
        }
    }
}
