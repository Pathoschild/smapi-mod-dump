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
using Pathoschild.Stardew.Automate;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AutomateChests
{
    internal class ObjectPatches
    {
        private static IMonitor Monitor;

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static IEnumerable<CodeInstruction> Chest__performObjectDropInAction__Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            var output = new List<CodeInstruction>();
            foreach (var instruction in instructions)
            {
                output.Add(instruction);
                if (instruction.opcode == OpCodes.Stloc_1)
                {
                    output.Add(new CodeInstruction(OpCodes.Ldarg_0));
                    output.Add(new CodeInstruction(OpCodes.Ldloc_1));
                    output.Add(new CodeInstruction(OpCodes.Call, typeof(ObjectPatches).GetMethod(nameof(Chest__performObjectDropInAction__PreserveAutomateModData))));
                }
            }
            return output;
        }
        public static void Chest__performObjectDropInAction__PreserveAutomateModData(Chest instance, Chest otherChest)
        {
            if (instance.modData.ContainsKey(ModEntry.ModDataExemptFlag))
                otherChest.modData[ModEntry.ModDataExemptFlag] = instance.modData[ModEntry.ModDataExemptFlag];
            if (instance.modData.ContainsKey(ModEntry.ModDataFlag))
                otherChest.modData[ModEntry.ModDataFlag] = instance.modData[ModEntry.ModDataFlag];
        }

        public static void Automate_AutomationFactory_GetFor_SObject__Postfix(ref IAutomatable __result)
        {
            try
            {
                if (__result is IContainer container)
                {
                    var obj = container.Location.getObjectAtTile(__result.TileArea.X, __result.TileArea.Y);
                    // if it is a normal ordinary chest and isn't flagged by AutomateChests, it is no longer a valid container (but if it is flagged, don't alter it (keeping it automated)
                    if (obj is Chest { SpecialChestType: Chest.SpecialChestTypes.None or Chest.SpecialChestTypes.BigChest } chest && !chest.modData.ContainsKey("SinZ.AutomateChests"))
                    {
                        __result = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"An error occured while postfixing Automate GetFor(SObject), this is a SinZ.AutomateChest bug not Automate, Technical Details:\n{ex.Message}\n{ex.StackTrace}", LogLevel.Error);
            }
        }

        public static void Automate_ModEntry_OnModMessageReceived__Postfix(object sender, ModMessageReceivedEventArgs e, ref object ___MachineManager)
        {
            try
            {
                // update automation if chest options changed
                if (Context.IsMainPlayer && e.FromModID == "SinZ.AutomateChests" && e.Type == nameof(AutomateUpdateChestMessage))
                {
                    var message = e.ReadAs<AutomateUpdateChestMessage>();
                    var location = Game1.getLocationFromName(message.LocationName);
                    var player = Game1.getFarmer(e.FromPlayerID);

                    string label = player != Game1.MasterPlayer
                        ? $"{player.Name}/{e.FromModID}"
                        : e.FromModID;

                    if (location != null)
                    {
                        Monitor.Log($"Received chest update from {label} for chest at {message.LocationName} ({message.Tile}), updating machines.");
                        ___MachineManager.GetType().GetMethod("QueueReload", new Type[] {typeof(GameLocation)}).Invoke(___MachineManager, new object[] { location });
                    }
                    else
                        Monitor.Log($"Received chest update from {label} for chest at {message.LocationName} ({message.Tile}), but no such location was found.");
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"An error occured while postfixing Automate OnModMessageReceived, this is a SinZ.AutomateChest bug not Automate, Technical Details:\n{ex.Message}\n{ex.StackTrace}", LogLevel.Error);
            }
        }
    }
}
