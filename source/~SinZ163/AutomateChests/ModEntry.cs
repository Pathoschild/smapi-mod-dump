/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SinZ163/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace AutomateChests
{
    public class ModEntry : Mod
    {

        /// <summary>The <see cref="Item.modData"/> flag which indicates a chest is automatable.</summary>
        public const string ModDataFlag = "SinZ.AutomateChests";
        /// <summary>The <see cref="Item.modData"/> flag which indicates a chest should not have interactions applied from this mod (taking/removing hopper or equivalent).</summary>
        public const string ModDataExemptFlag = "SinZ.AutomateChests/ExemptInteractivity";

        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

            this.Config = this.Helper.ReadConfig<ModConfig>();


            ObjectPatches.Initialize(Monitor);
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(Type.GetType("Pathoschild.Stardew.Automate.Framework.AutomationFactory,Automate"), "GetFor", parameters: new Type[] { typeof(SObject), typeof(GameLocation), typeof(Vector2).MakeByRefType() }),
                postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.Automate_AutomationFactory_GetFor_SObject__Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(Type.GetType("Pathoschild.Stardew.Automate.ModEntry,Automate"), "OnModMessageReceived", parameters: new Type[] { typeof(object), typeof(ModMessageReceivedEventArgs)}),
                postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.Automate_ModEntry_OnModMessageReceived__Postfix))
             );
            harmony.Patch(
                original: AccessTools.Method(typeof(Chest), nameof(Chest.performObjectDropInAction)),
                transpiler: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.Chest__performObjectDropInAction__Transpiler))
            );
            this.Monitor.Log("This mod patches Automate. If you notice issues with Automate, make sure it happens without this mod before reporting it to the Automate page.", LogLevel.Trace);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {   
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );
            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => "The following config is for controlling the item required to enable a chest for automation, and the item you get back when undoing the chest automation. If you change the config while having chests automated, if you remove the item you will get the new configs item instead");
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Activation Item ID",
                getValue: () => this.Config.ActivationItemId,
                setValue: value => this.Config.ActivationItemId = value
            );
        }

        /**
         * This is strongly based on SuperHoppers OnButtonPressed method available at https://github.com/spacechase0/StardewValleyMods/blob/develop/SuperHopper/Mod.cs
         */
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;
            if (!e.Button.IsActionButton())
                return;

            Game1.currentLocation.objects.TryGetValue(e.Cursor.GrabTile, out SObject obj);
            if (obj != null && obj is Chest { SpecialChestType: Chest.SpecialChestTypes.None or Chest.SpecialChestTypes.BigChest } chest)
            {
                if (chest.modData.ContainsKey(ModDataExemptFlag))
                    return;

                var heldItem = Game1.player.ActiveObject;
                // if the player is holding a hopper and the chest isn't tagged by us, it should be now
                if (heldItem != null && heldItem.QualifiedItemId == Config.ActivationItemId && !chest.modData.ContainsKey(ModDataFlag))
                {
                    chest.Tint = Color.DarkViolet;
                    chest.modData[ModDataFlag] = "1";

                    if (heldItem.Stack > 1)
                        Game1.player.ActiveObject.Stack--;
                    else
                        Game1.player.ActiveObject = null;

                    Game1.playSound("furnace");
                    NotifyAutomateOfChestUpdate(Game1.currentLocation.Name, chest.TileLocation);
                }
                // otherwise if it is already marked by us, we should just return it to them if they have an empty hand
                else if (Game1.player.CurrentItem == null && chest.modData.ContainsKey(ModDataFlag))
                {
                    chest.Tint = Color.White;
                    chest.heldObject.Value = null;
                    chest.modData.Remove(ModDataFlag);

                    Item item = ItemRegistry.Create(Config.ActivationItemId);
                    Game1.player.addItemByMenuIfNecessary(item);

                    Game1.playSound("shiny4");
                    NotifyAutomateOfChestUpdate(Game1.currentLocation.Name, chest.TileLocation);
                }
            }
        }

        public void NotifyAutomateOfChestUpdate(string location, Vector2 tile)
        {
            long hostId = Game1.MasterPlayer.UniqueMultiplayerID;
            var message = new AutomateUpdateChestMessage { LocationName = location, Tile = tile };
            Helper.Multiplayer.SendMessage(message, nameof(AutomateUpdateChestMessage), modIDs: new[] { "Pathoschild.Automate" }, playerIDs: new[] { hostId });
        }
    }
}
