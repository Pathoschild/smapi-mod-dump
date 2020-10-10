/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Igorious.StardewValley.DynamicAPI.Objects;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.DynamicAPI.Services.Internal;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.NewMachinesMod
{
    partial class NewMachinesMod
    {
        private void RegisterCommands()
        {
            Command.RegisterCommand(
                "nmm_out_craftables",
                "Outputs a list of craftable items | nmm_out_craftables",
                new[] { "" })
                .CommandFired += ExecuteOutCraftables;

            Command.RegisterCommand(
                "nmm_player_addcraftable",
                "Gives the player craftable item with specified ID | nmm_player_addcraftable <id>",
                new[] { "(Int32)<id>" })
                .CommandFired += ExecutePlayerAddCraftable;

            Command.RegisterCommand(
                "nmm_machine_showinputs",
                "Show all possible input items for selected machine or machine with specified ID | nmm_machine_showinputs [id]",
                new[] { "(Int32)[id]" })
                .CommandFired += ExecuteMachineShowInputs;

            Command.RegisterCommand(
                "nmm_machine_demolish",
                "Destroy all machines with specified ID in location | nmm_machine_demolish <id> [all]",
                new[] { "(Int32)<id> (String)[all]" })
                .CommandFired += ExecuteMachineDemolish;

            Command.RegisterCommand(
                "nmm_deactivate",
                "Deactivate new objects | nmm_deactivate",
                new[] { "" })
                .CommandFired += Deactivate;
        }
    
        private static void Deactivate(object sender, EventArgsCommand e)
        {
            ClassMapperService.Instance.ForceDeactivation();
        }

        private static void ExecuteMachineDemolish(object sender, EventArgsCommand e)
        {
            int id;
            var args = e.Command.CalledArgs;
            if (!CheckArgumentIsCraftableID(args, out id)) return;

            var hasAny = false;
            if (args.Length > 1 && string.Equals(args[1], "all", StringComparison.OrdinalIgnoreCase))
            {
                Traverser.Instance.TraverseLocations(l => hasAny |= Demolish(l, id));
            }
            else
            {
                hasAny |= Demolish(Game1.currentLocation, id);
            }
            if (hasAny) Game1.playSound(Sound.Explosion.GetDescription());
        }

        private static bool Demolish(GameLocation l, int targetID)
        {
            var machines = l.Objects.Values.Where(o => o.ParentSheetIndex == targetID && o.bigCraftable).ToList();
            machines.ForEach(m => Demolish(l, m));
            return machines.Any();
        }

        private static void Demolish(GameLocation location, Object machine)
        {
            string craftingRecipeRaw;
            if (!CraftingRecipe.craftingRecipes.TryGetValue(machine.Name, out craftingRecipeRaw)) return;

            var craftingRecipe = CraftingRecipeInformation.Parse(craftingRecipeRaw);
            var debrisLocation = machine.TileLocation * Game1.tileSize + new Vector2(Game1.tileSize / 2f, Game1.tileSize / 2f);
            foreach (var ingredient in craftingRecipe.Materials)
            {
                location.debris.Add(new Debris(new SmartObject(ingredient.ID, ingredient.Count), debrisLocation));
            }
            if (machine.heldObject != null && machine.readyForHarvest)
            {
                location.debris.Add(new Debris(machine.heldObject, debrisLocation));
            }
            location.removeObject(machine.TileLocation, false);
        }

        private static void ExecuteOutCraftables(object sender, EventArgsCommand e)
        {
            foreach (var craftableID in Game1.bigCraftablesInformation.Keys)
            {
                try
                {
                    var craftable = new Object(Vector2.Zero, craftableID);
                    Log.SyncColour($"[ID={craftableID:D3}] {craftable.Name}", ConsoleColor.Gray);
                }
                catch { }
            }
        }

        private static void ExecuteMachineShowInputs(object sender, EventArgsCommand e)
        {
            int id;
            var farmer = Game1.player;

            var args = e.Command.CalledArgs;
            if (args.Length > 0)
            {
                if (!CheckArgumentIsCraftableID(args, out id)) return;
            }
            else
            {
                var activeObject = farmer.ActiveObject;
                if (activeObject == null || !activeObject.bigCraftable)
                {
                    Log.Async("Current object is not machine");
                    return;
                }
                id = activeObject.ParentSheetIndex;
            }

            var machine = ClassMapperService.Instance.ToSmartObject(new Object(Vector2.Zero, id));

            Log.SyncColour($"Inputs for {machine.Name} (ID={machine.ParentSheetIndex}):", ConsoleColor.Gray);
            var itemIDs = Game1.objectInformation.Keys.ToList();
            foreach (var itemID in itemIDs)
            {
                var item = new Object(itemID, 1);
                machine.performObjectDropInAction(item, true, farmer);
                var canDrop = (machine.heldObject != null);
                machine.heldObject = null;
                if (canDrop) Log.SyncColour($"[ID={itemID:D3}] {item.Name}", ConsoleColor.Gray);
            }
        }

        private static void ExecutePlayerAddCraftable(object sender, EventArgsCommand e)
        {
            int id;
            var args = e.Command.CalledArgs;
            if (!CheckArgumentIsCraftableID(args, out id)) return;

            var o = new Object(Vector2.Zero, id);
            Game1.player.addItemByMenuIfNecessary(o);
        }

        private static bool CheckArgumentIsCraftableID(string[] args, out int id)
        {
            id = 0;
            if (args.Length == 0)
            {
                Log.LogValueNotSpecified();
                return false;
            }

            if (!args[0].IsInt32())
            {
                Log.LogValueNotInt32();
                return false;
            }

            id = args[0].AsInt32();
            if (!Game1.bigCraftablesInformation.ContainsKey(id))
            {
                Log.AsyncR("<id> is invalid");
                return false;
            }

            return true;
        }
    }
}
