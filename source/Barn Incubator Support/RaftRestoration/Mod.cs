/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using RaftRestoration.Patches;
using Spacechase.Shared.Harmony;
using SpaceShared;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using StardewValley.Tools;

namespace RaftRestoration
{
    internal class Mod : StardewModdingAPI.Mod
    {
        public static Mod Instance;

        public override void Entry(IModHelper helper)
        {
            Mod.Instance = this;
            Log.Monitor = this.Monitor;

            helper.Events.Display.MenuChanged += this.OnMenuChanged;

            HarmonyPatcher.Apply(this,
                new FarmerPatcher(),
                new RaftPatcher(),
                new ToolPatcher()
            );
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu shop)
            {
                if (shop.portraitPerson?.Name != "Pierre")
                    return;

                var raft = new Raft();
                shop.forSale.Add(raft);
                shop.itemPriceAndStock.Add(raft, new[] { 5000, 1 });
            }
        }
    }
}
