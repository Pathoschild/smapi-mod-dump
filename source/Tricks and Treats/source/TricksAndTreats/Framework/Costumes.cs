/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cl4r3/Halloween-Mod-Jam-2023
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using static TricksAndTreats.Globals;

namespace TricksAndTreats
{
    internal class Costumes
    {
        internal static void Initialize(IMod ModInstance)
        {
            Helper = ModInstance.Helper;
            Monitor = ModInstance.Monitor;

            Helper.Events.GameLoop.DayStarted += (object sender, DayStartedEventArgs e) => { CheckForCostume(); };
            Helper.Events.Player.Warped += OnWarp;
            Helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        private static void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady || e.OldMenu is null)
                return;

            //Log.Debug($"TaT: OldMenu is " + e.OldMenu.GetType());
            if (e.OldMenu is not GameMenu)
                return;

            CheckForCostume();
        }

        internal static void OnWarp(object sender, WarpedEventArgs e)
        {
            if (!Context.IsWorldReady || !(Game1.currentSeason == "fall" && Game1.dayOfMonth == 27))
                return;

            if (e.OldLocation.Name == "Town")
            {
                if (Game1.player.activeDialogueEvents.ContainsKey(TreatCT))
                    Game1.player.activeDialogueEvents.Remove(TreatCT);
            }
            else if (e.NewLocation.Name == "Town")
                CheckForCostume();
        }

        internal static void CheckForCostume(bool verbose = false)
        {
            if (!Context.IsWorldReady || !(Game1.currentSeason == "fall" && Game1.dayOfMonth == 27))
                return;

            Farmer farmer = Game1.player;

            string hat = farmer.hat.Value is null ? "" : farmer.hat.Value.Name;
            string top = farmer.shirtItem.Value is null ? "" : farmer.shirtItem.Value.Name;
            string bot = farmer.pantsItem.Value is null ? "": farmer.pantsItem.Value.Name;

            string[] clothes = { "", "", "" };
            foreach (KeyValuePair<string, Costume> entry in CostumeData)
            {
                if (hat == entry.Value.Hat)
                    clothes.SetValue(entry.Key, 0);
                if (top == entry.Value.Top)
                    clothes.SetValue(entry.Key, 1);
                if (bot == entry.Value.Bottom)
                    clothes.SetValue(entry.Key, 2);
            }
            if (verbose)
            {
                // Hat
                if (hat != "" && clothes[0] != "")
                    Log.Info(Helper.Translation.Get("commands.hat_costume").ToString().Replace("{hat}", hat).Replace("{costume}", clothes[0]));
                else if (hat != "")
                    Log.Info(Helper.Translation.Get("commands.hat_no-costume").ToString().Replace("{hat}", hat));
                else
                    Log.Info(Helper.Translation.Get("commands.no_hat"));
                // Top
                if (top != "" && clothes[1] != "")
                    Log.Info(Helper.Translation.Get("commands.top_costume").ToString().Replace("{top}", top).Replace("{costume}", clothes[1]));
                else if (top != "")
                    Log.Info(Helper.Translation.Get("commands.top_no-costume").ToString().Replace("{top}", top));
                else
                    Log.Info(Helper.Translation.Get("commands.no_top"));
                // Bottom
                if (bot != "" && clothes[2] != "")
                    Log.Info(Helper.Translation.Get("commands.bot_costume").ToString().Replace("{bot}", bot).Replace("{costume}", clothes[2]));
                else if (bot != "")
                    Log.Info(Helper.Translation.Get("commands.bot_no-costume").ToString().Replace("{bot}", bot));
                else
                    Log.Info(Helper.Translation.Get("commands.no_bot"));
            }

            string[] costumes_only = Array.Empty<string>();
            foreach (string i in clothes)
            {
                if (CostumeData.ContainsKey(i))
                    costumes_only = costumes_only.Append(i).ToArray();
            }

            var groups = costumes_only.GroupBy(v => v);
            string costume = null;
            foreach (var group in groups)
            {
                if (CostumeData[group.Key].NumPieces == group.Count())
                    costume = group.Key;
            }

            if (costume is not null && !Game1.player.activeDialogueEvents.ContainsKey(CostumeCT + costume.ToLower().Replace(' ', '_')))
            {
                Game1.player.modData[CostumeKey] = costume;
                if (verbose)
                    Log.Info(Helper.Translation.Get("commands.new_costume").ToString().Replace("{costume}", costume));
                else
                    Log.Trace("TaT: Now wearing costume " + costume);
                foreach (string key in Game1.player.activeDialogueEvents.Keys.Where(x => x.StartsWith(CostumeCT.ToLower())).ToArray()) {
                    Game1.player.activeDialogueEvents.Remove(key);
                }
                Game1.player.activeDialogueEvents.Add(CostumeCT + costume.ToLower().Replace(' ', '_'), 1);
                if (!Game1.player.activeDialogueEvents.ContainsKey(TreatCT))
                {
                    Game1.player.activeDialogueEvents.Add(TreatCT, 1);
                    /*
                    foreach (string name in NPCData.Keys)
                    {
                        var npc = Game1.getCharacterFromName(name);
                        npc.checkForNewCurrentDialogue(1);
                    }
                    */
                }
            }
            else if (costume is null)
            {
                if (verbose)
                    Log.Info(Helper.Translation.Get("commands.no_costume"));
                else
                    Log.Trace("TaT: Currently not wearing costume");
                foreach (string key in Game1.player.activeDialogueEvents.Keys.Where(x => x.StartsWith(CostumeCT.ToLower())).ToArray())
                    Game1.player.activeDialogueEvents.Remove(key);
                if (Game1.player.activeDialogueEvents.ContainsKey(TreatCT))
                    Game1.player.activeDialogueEvents.Remove(TreatCT);
            }
        }
    }

}
