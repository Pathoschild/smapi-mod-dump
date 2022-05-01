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
using Pathoschild.Stardew.Automate;
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
        private readonly string ModDataFlag = "SinZ.AutomateChests";

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            //debug only
            helper.Events.World.ObjectListChanged += this.OnObjectListChanged;

            ObjectPatches.Initialize(Monitor);
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                // the default AutomationFactory is an internal class, so need to access via string anyway, and modapi does not expose containers, only machines
                original: AccessTools.Method(Type.GetType("Pathoschild.Stardew.Automate.Framework.AutomationFactory,Automate"), "GetFor", parameters: new Type[] { typeof(SObject), typeof(GameLocation), typeof(Vector2).MakeByRefType() }),
                postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.Automate_AutomationFactory_GetFor_SObject__Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(Type.GetType("Pathoschild.Stardew.Automate.ModEntry,Automate"), "OnModMessageReceived", parameters: new Type[] { typeof(object), typeof(ModMessageReceivedEventArgs)}),
                postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.Automate_ModEntry_OnModMessageReceived__Postfix))
             );
            this.Monitor.Log("This mod patches Automate. If you notice issues with Automate, make sure it happens without this mod before reporting it to the Automate page.", LogLevel.Trace);
        }

        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            this.Monitor.VerboseLog("Science");
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
            if (obj is Chest { SpecialChestType: Chest.SpecialChestTypes.None} chest)
            {
                // if the player is holding a hopper and the chest isn't tagged by us, it should be now
                if (Game1.player.ActiveObject is SObject { ParentSheetIndex: 275, bigCraftable: {Value: true} } && !chest.modData.ContainsKey(this.ModDataFlag))
                {
                    chest.Tint = Color.DarkViolet;
                    chest.modData[this.ModDataFlag] = "1";

                    if (Game1.player.ActiveObject.Stack > 1)
                        Game1.player.ActiveObject.Stack--;
                    else
                        Game1.player.ActiveObject = null;

                    Game1.playSound("furnace");
                    NotifyAutomateOfChestUpdate(Game1.currentLocation.Name, chest.TileLocation);
                }
                // otherwise if it is already marked by us, we should just return it to them if they have an empty hand
                else if (Game1.player.CurrentItem == null && chest.modData.ContainsKey(this.ModDataFlag))
                {
                    chest.Tint = Color.White;
                    chest.heldObject.Value = null;
                    chest.modData.Remove(this.ModDataFlag);

                    Game1.player.addItemToInventory((Item)new SObject(Vector2.Zero, 275, false));

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
