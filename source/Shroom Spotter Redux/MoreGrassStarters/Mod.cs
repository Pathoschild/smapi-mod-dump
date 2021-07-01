/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using SpaceShared;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;

namespace MoreGrassStarters
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            instance = this;
            Log.Monitor = Monitor;

            helper.Events.Display.MenuChanged += onMenuChanged;

            if ( File.Exists(Path.Combine(Helper.DirectoryPath, "assets", "grass.png")) )
            {
                GrassStarterItem.tex2 = Mod.instance.Helper.Content.Load<Texture2D>("assets/grass.png");
            }
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!(e.NewMenu is ShopMenu menu) || menu.portraitPerson == null)
                return;

            if (menu.portraitPerson.Name == "Pierre")
            {
                var forSale = Helper.Reflection.GetField<List<ISalable>>(menu, "forSale").GetValue();
                var itemPriceAndStock = Helper.Reflection.GetField<Dictionary<ISalable, int[]>>(menu, "itemPriceAndStock").GetValue();

                for (int i = Grass.caveGrass; i < 5 + GrassStarterItem.ExtraGrassTypes; ++i)
                {
                    var item = new GrassStarterItem(i);
                    forSale.Add(item);
                    itemPriceAndStock.Add(item, new int[] { 100, int.MaxValue });
                }
            }
        }
    }
}
