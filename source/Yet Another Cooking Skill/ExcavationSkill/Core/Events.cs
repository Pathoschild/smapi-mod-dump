/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ArchaeologySkill.Objects.Water_Shifter;
using ArchaeologySkill.Objects.Restoration_Table;
using BirbCore.Attributes;
using Microsoft.Xna.Framework.Graphics;
using MoonShared.APIs;
using Newtonsoft.Json;
using SpaceCore;
using SpaceCore.Interface;
using SpaceShared.APIs;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Objects;
using static BirbCore.Attributes.SMod;
using static SpaceCore.Skills;
using Object = StardewValley.Object;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace ArchaeologySkill.Core
{
    [SEvent]
    public class Events
    {

        [SEvent.GameLaunchedLate]
        private static void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            
            var sc = ModEntry.Instance.Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore");
            sc.RegisterSerializerType(typeof(WaterShifter));
            sc.RegisterSerializerType(typeof(RestorationTable));
            ArchaeologySkill.Objects.Water_Shifter.Patches.Patch(ModEntry.Instance.Helper);
            ArchaeologySkill.Objects.Restoration_Table.Patches.Patch(ModEntry.Instance.Helper);

            ModEntry.ItemDefinitions = ModEntry.Assets.ItemDefinitions;


            Log.Trace("Archaeology: Trying to Register skill.");
            SpaceCore.Skills.RegisterSkill(new Archaeology_Skill());

            foreach (string entry in ModEntry.ItemDefinitions["extra_loot_table"])
            {
                Log.Trace("Archaeology: Adding " + entry + " to the bonus loot table");
                ModEntry.BonusLootTable.Add(entry);
            }
            foreach (string entry in ModEntry.ItemDefinitions["waterShifter_loot_table"])
            {
                Log.Trace("Archaeology: Adding " + entry + " to the water shifter loot table");
                ModEntry.WaterSifterLootTable.Add(entry);
            }
            foreach (string entry in ModEntry.ItemDefinitions["artifact_loot_table"])
            {
                Log.Trace("Archaeology: Adding " + entry + " to the artifact loot table");
                ModEntry.ArtifactLootTable.Add(entry);
            }


            Log.Trace("Archaeology: Do I see XP display?");
            if (ModEntry.XPDisplayLoaded)
            {
                Log.Trace("Archaeology: I do see XP display, Registering API.");
                ModEntry.XpAPI = ModEntry.Instance.Helper.ModRegistry.GetApi<IXPDisplayApi>("Shockah.XPDisplay");
            }
        }

        [SEvent.DayStarted]
        private void DayStarted(object sender, DayStartedEventArgs e)
        {

            if (Game1.IsMasterGame)
            {
                int extraArtifactSpot = 0;

                foreach (Farmer farmer in Game1.getOnlineFarmers())
                {
                    Log.Trace("Archaeology: Does a player have Pioneer Profession?");
                    var player = Game1.getFarmer(farmer.UniqueMultiplayerID);
                    if (player.isActive() && player.HasCustomProfession(Archaeology_Skill.Archaeology5a))
                    {
                        Log.Trace("Archaeology: They do have Pioneer profession, spawn extra artifact spots.");
                        extraArtifactSpot += 2;
                        Log.Trace("Archaeology: extra artifact spot chance increased by: " + extraArtifactSpot.ToString());
                    }
                }

                if (extraArtifactSpot != 0)
                {
                    SpawnDiggingSpots(extraArtifactSpot);
                }
            }
        }



        [SEvent.SaveLoaded]
        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (ModEntry.XpAPI is not null)
            {
                Log.Trace("Archaeology: XP display found, Marking Hoe and Pan as Skill tools");
                ModEntry.XpAPI.RegisterToolSkillMatcher(ModEntry.Instance.ToolSkillMatchers[0]);
                ModEntry.XpAPI.RegisterToolSkillMatcher(ModEntry.Instance.ToolSkillMatchers[1]);
            }

            string Id = "moonslime.Archaeology";
            int skillLevel = Game1.player.GetCustomSkillLevel(Id);
            if (skillLevel == 0)
            {
                return;
            }

            if (skillLevel >= 5 && !(Game1.player.HasCustomProfession(Archaeology_Skill.Archaeology5a) ||
                                     Game1.player.HasCustomProfession(Archaeology_Skill.Archaeology5b)))
            {
                Game1.endOfNightMenus.Push(new SkillLevelUpMenu(Id, 5));
            }

            if (skillLevel >= 10 && !(Game1.player.HasCustomProfession(Archaeology_Skill.Archaeology10a1) ||
                                      Game1.player.HasCustomProfession(Archaeology_Skill.Archaeology10a2) ||
                                      Game1.player.HasCustomProfession(Archaeology_Skill.Archaeology10b1) ||
                                      Game1.player.HasCustomProfession(Archaeology_Skill.Archaeology10b2)))
            {
                Game1.endOfNightMenus.Push(new SkillLevelUpMenu(Id, 10));
            }

            foreach (KeyValuePair<string, string> recipePair in DataLoader.CraftingRecipes(Game1.content))
            {
                string conditions = ArgUtility.Get(recipePair.Value.Split('/'), 4, "");
                if (!conditions.Contains(Id))
                {
                    continue;
                }
                if (conditions.Split(" ").Length < 2)
                {
                    continue;
                }

                int level = int.Parse(conditions.Split(" ")[1]);

                if (skillLevel < level)
                {
                    continue;
                }

                Game1.player.craftingRecipes.TryAdd(recipePair.Key, 0);
            }

            foreach (KeyValuePair<string, string> recipePair in DataLoader.CookingRecipes(Game1.content))
            {
                string conditions = ArgUtility.Get(recipePair.Value.Split('/'), 3, "");
                if (!conditions.Contains(Id))
                {
                    continue;
                }
                if (conditions.Split(" ").Length < 2)
                {
                    continue;
                }

                int level = int.Parse(conditions.Split(" ")[1]);

                if (skillLevel < level)
                {
                    continue;
                }

                if (Game1.player.cookingRecipes.TryAdd(recipePair.Key, 0) &&
                    !Game1.player.hasOrWillReceiveMail("robinKitchenLetter"))
                {
                    Game1.mailbox.Add("robinKitchenLetter");
                }
            }
        }

        private static void SpawnDiggingSpots(int spawn)
        {
            List<Tuple<string, Vector2>> locations;
            locations = new List<Tuple<string, Vector2>>();

            int maxspawn = 0;

            foreach (GameLocation loc in Game1.locations)
            {

                if (loc.IsFarm || !loc.IsOutdoors)
                    continue;
                if (maxspawn >= spawn)
                    break;

                for (int z = 0; z < spawn; z++)
                {

                    int i = Game1.random.Next(loc.Map.DisplayWidth / Game1.tileSize);
                    int j = Game1.random.Next(loc.Map.DisplayHeight / Game1.tileSize);
                    GameLocation gameLocation = loc;
                    Vector2 vector = new Vector2(i, j);
                    if (gameLocation.CanItemBePlacedHere(vector) && !gameLocation.IsTileOccupiedBy(vector) && gameLocation.getTileIndexAt(i, j, "AlwaysFront") == -1 && gameLocation.getTileIndexAt(i, j, "Front") == -1 && !gameLocation.isBehindBush(vector) && (gameLocation.doesTileHaveProperty(i, j, "Diggable", "Back") != null || (gameLocation.GetSeason() == Season.Winter && gameLocation.doesTileHaveProperty(i, j, "Type", "Back") != null && gameLocation.doesTileHaveProperty(i, j, "Type", "Back").Equals("Grass"))))
                    {
                        if (loc.Name.Equals("Forest") && i >= 93 && j <= 22)
                        {
                            continue;
                        }

                        gameLocation.objects.Add(vector, ItemRegistry.Create<Object>("(O)590"));
                    }
                    locations.Add(new Tuple<string, Vector2>(loc.Name, vector));
                    Log.Trace($"Location Name: {loc.Name}, IsFarm: {loc.IsFarm}, IsOutDoors: {loc.IsOutdoors}, X: {vector.X}, Y: {vector.Y}");
                    maxspawn++;
                }
            }
        }
    }
}
