/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/BlueberryMushroomMachine
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BlueberryMushroomMachine.Editors
{
    internal static class BigCraftablesTilesheetEditor
	{
        private static IRawTextureData tex;
        internal static void Initialize(IModContentHelper helper)
        {
            tex = helper.Load<IRawTextureData>(ModValues.MachinePath);
        }

		public static bool ApplyEdit(AssetRequestedEventArgs e)
		{
            if (e.NameWithoutLocale.IsEquivalentTo(@"TileSheets/Craftables"))
            {
                e.Edit(EditImpl);
                return true;
            }
            return false;
		}

		public static void EditImpl(IAssetData asset)
		{
			Log.T($"Editing {asset.Name}.",
                ModEntry.Instance.Config.DebugMode);

            // Expand the base tilesheet if needed.
            var src = tex;
			var dest = asset.AsImage();
			var srcRect = new Rectangle(0, 0, 16, 32);
			var destRect = Propagator.getSourceRectForBigCraftable(ModValues.PropagatorIndex);

            // expand if needed
            dest.ExtendImage(dest.Data.Bounds.Width, destRect.Height);

			// Append machine sprite onto the default tilesheet.
			dest.PatchImage(src, srcRect, destRect);
		}
	}
}
