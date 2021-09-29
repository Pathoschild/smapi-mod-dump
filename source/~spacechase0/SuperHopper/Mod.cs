/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Spacechase.Shared.Patching;
using SpaceShared;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using SuperHopper.Patches;

namespace SuperHopper
{
    internal class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;

        public override void Entry(IModHelper helper)
        {
            Mod.instance = this;
            Log.Monitor = this.Monitor;

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            HarmonyPatcher.Apply(this,
                new ObjectPatcher()
            );
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            Game1.currentLocation.objects.TryGetValue(e.Cursor.GrabTile, out StardewValley.Object obj);
            if (obj is Chest { SpecialChestType: Chest.SpecialChestTypes.AutoLoader } chest && (e.Button is SButton.MouseLeft or SButton.ControllerA))
            {
                if (chest.heldObject.Value == null)
                {
                    if (Utility.IsNormalObjectAtParentSheetIndex(Game1.player.ActiveObject, StardewValley.Object.iridiumBar))
                    {
                        chest.Tint = Color.DarkViolet;
                        chest.heldObject.Value = (StardewValley.Object)Game1.player.ActiveObject.getOne();
                        if (Game1.player.ActiveObject.Stack > 1)
                            Game1.player.ActiveObject.Stack--;
                        else
                            Game1.player.ActiveObject = null;
                        Game1.playSound("furnace");
                    }
                }
                else if (Game1.player.CurrentItem == null)
                {
                    chest.Tint = Color.White;
                    chest.heldObject.Value = null;
                    Game1.player.addItemToInventory(new StardewValley.Object(StardewValley.Object.iridiumBar, 1));
                    Game1.playSound("shiny4");
                }
            }
        }
    }
}
