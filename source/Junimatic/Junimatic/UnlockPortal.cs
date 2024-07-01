/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/Junimatic
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Objects;
using StardewValley.TerrainFeatures;

using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;

namespace NermNermNerm.Junimatic
{
    /// <summary>
    ///   Everything related to getting the portals.
    /// </summary>
    public class UnlockPortal : ISimpleLog
    {
        private ModEntry mod = null!; // set in Entry

        private const string JunimoPortalRecipe = "Junimatic.JunimoPortalRecipe";
        private const string OldJunimoPortalQiid = "(O)Junimatic.OldJunimoPortal";
        private const string OldJunimoPortalQuest = "Junimatic.OldJunimoPortalQuest";

        public const string JunimoPortal = "Junimatic.JunimoPortal";
        public const string JunimoPortalDiscoveryEvent = "Junimatic.JunimoPortalDiscoveryEvent"; // Public because event code can depend on this.

        /// <summary>
        ///   The key to the value that tells us whether the portal was put down somewhere.
        ///   (It may have been picked up since then.)  The value stored is really just a
        ///   boolean, but for debugging, it's the tile coordinates it's placed at.
        /// </summary>
        private const string ModDataKey_PlacedOldPortal = "Junimatic.OldJunimoPortalPlaced";

        /// <summary>
        ///   Whether we've given the player the clue that they could look for it.
        ///   The value is just a bool, but we store the day we gave the alert.
        /// </summary>
        private const string ModDataKey_AlertedPlayer = "Junimatic.AlertedPlayer";

        public void Entry(ModEntry mod)
        {
            this.mod = mod;
            this.mod.Helper.Events.Content.AssetRequested += this.OnAssetRequested;
            this.mod.Helper.Events.Player.InventoryChanged += this.Player_InventoryChanged;
            this.mod.Helper.Events.GameLoop.DayEnding += this.GameLoop_DayEnding;
            this.mod.Helper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;

            mod.PetFindsThings.AddObjectFinder(OldJunimoPortalQiid, .02);
        }

        public bool IsUnlocked => Game1.MasterPlayer.eventsSeen.Contains(JunimoPortalDiscoveryEvent);

        private void Player_InventoryChanged(object? sender, InventoryChangedEventArgs e)
        {
            if (e.Added.Any(i => i.QualifiedItemId == OldJunimoPortalQiid))
            {
                if (!e.Player.IsMainPlayer)
                {
                    Game1.addHUDMessage(new HUDMessage(L("Give the strange little structure to the host player - only the host can advance this quest.  (Put it in a chest for them.)")) { noIcon = true });
                }
                else if (!this.IsUnlocked && !e.Player.questLog.Any(q => q.id.Value == OldJunimoPortalQuest))
                {
                    e.Player.addQuest(OldJunimoPortalQuest);
                }
                else
                {
                    this.LogWarning($"Player received a {OldJunimoPortalQiid} when they've already got or have completed the quest");
                }
            }
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/BigCraftables"))
            {
                e.Edit(editor =>
                {
                    this.EditBigCraftableData(editor.AsDictionary<string, BigCraftableData>().Data);
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit(editor =>
                {
                    this.EditObjectData(editor.AsDictionary<string, ObjectData>().Data);
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                e.Edit(editor =>
                {
                    IDictionary<string, string> recipes = editor.AsDictionary<string, string>().Data;
                    recipes[JunimoPortalRecipe] = IF($"{StardewValley.Object.woodID} 20 {"92" /* sap*/} 30 {-777 /*wild seeds any*/} 5/Field/{JunimoPortal}/true/None/");
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/WizardHouse"))
            {
                e.Edit(editor =>
                {
                    this.EditWizardHouseEvents(editor.AsDictionary<string, string>().Data);
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Quests"))
            {
                e.Edit(editor =>
                {
                    IDictionary<string, string> data = editor.AsDictionary<string, string>().Data;
                    data[OldJunimoPortalQuest] = SdvQuest("Basic/The Strange Little Structure/You found the remnants of what looks like a little building.  It smells like it has some Forest Magic in it./Bring the remnants of the strange little structure to the wizard's tower./null/-1/0/-1/false");
                });
            }
        }

        private void EditBigCraftableData(IDictionary<string, BigCraftableData> objects)
        {
            objects[JunimoPortal] = new BigCraftableData()
            {
                Name = JunimoPortal,
                SpriteIndex = 0,
                CanBePlacedIndoors = true,
                CanBePlacedOutdoors = true,
                Description = L("A portal through which Junimos who want to help out on the farm can appear.  Place pathways next to these when placing them outdoors so the Junimos will know where to go."),
                DisplayName = L("Junimo Portal"),
                Texture = ModEntry.BigCraftablesSpritesPseudoPath,
            };
        }

        private void EditObjectData(IDictionary<string, ObjectData> objects)
        {
            ModEntry.AddQuestItem(
                objects,
                OldJunimoPortalQiid,
                L("a strange little structure"),
                L("At first it looked like a woody weed, but a closer look makes it like a little structure, and it smells sorta like the Wizard's forest-magic potion."),
                0);
        }

        private void EditWizardHouseEvents(IDictionary<string, string> eventData)
        {
            string commonPart1 = SdvEvent($@"WizardSong/
-1000 -1000/
farmer 8 24 0 Wizard 10 15 2 Junimo -2000 -2000 2/
removeQuest {OldJunimoPortalQuest}/
addConversationTopic {ConversationKeys.JunimosLastTripToMine} 200/
addConversationTopic {UnlockCropMachines.ConversationKeyBigCrops} 200/
setSkipActions MarkCraftingRecipeKnown All {JunimoPortalRecipe}#removeItem {OldJunimoPortalQiid}/
skippable/
showFrame Wizard 20/
viewport 8 18 true/
move farmer 0 -3 0/
pause 2000/
speak Wizard ""Ah... Come in.""/
pause 800/
animate Wizard false false 100 20 21 22 0/
playSound dwop/
pause 1000/
stopAnimation Wizard/
move Wizard -2 0 3/
move Wizard 0 2 2/
pause 1500/
speak Wizard ""You have something to show me?  Well, bring it to me!""/
move farmer -1 0 3/
move farmer 0 -4 0/
faceDirection farmer 1/
itemAboveHead {OldJunimoPortalQiid}/ 
playSound dwop/
faceDirection farmer 1/
pause 1000/
faceDirection Wizard 3/
speak Wizard ""Ah I see why you thought I should see this...#$b#I believe I recognize the magical traces, but let me consult my vast reference library to be certain...""/
");

            string stockPart2 = SdvEvent($@"
move Wizard 0 -2 0/
faceDirection Wizard 2/
faceDirection farmer 0/
speak Wizard ""Come along then!""/
move Wizard 0 -10 0 farmer 0 -10 0/
move Wizard 1 0 1/
faceDirection Wizard 0/
emote Wizard 40/
pause 1000/
move Wizard -3 0 3/
faceDirection Wizard 0/
emote Wizard 40/
pause 1000/
faceDirection Wizard 2/
speak Wizard ""Yes.  I was right...#$b#As always.""/
move Wizard 2 0 1/
move Wizard 0 2 2/
");
            string svePart2 = SdvEvent($@"
move Wizard 0 4 2
faceDirection Wizard 0
faceDirection farmer 2
speak Wizard ""Come along then!""
move Wizard 0 5 2 true
move farmer 0 5 2 true
fade
viewport -1000 -1000
waitForAllStationary

changeLocation Custom_WizardBasement
warp Wizard 12 13
warp farmer 8 13
faceDirection farmer 1
faceDirection Wizard 3
viewport 8 13
fade unfade

pause 500
move Wizard 1 0 1
faceDirection Wizard 0
emote Wizard 40
pause 1000
move Wizard -3 0 3
faceDirection Wizard 0
emote Wizard 40
pause 1000
faceDirection Wizard 2
speak Wizard ""Yes.  I was right...#$b#As always.""
").Replace("\r", "").Replace("\n", "/");

            string commonPart3 = SdvEvent($@"
faceDirection Wizard 3/
faceDirection farmer 1/
speak Wizard ""This is a sort of a crude portal, made by your Grandfather to allow Junimos to easily travel between their world and ours.#$b#It's an easy thing to construct, even the greenest apprentice could do it.  Here, let me teach it to you.""/
removeItem {OldJunimoPortalQiid}/
pause 500/
itemAboveHead/
playsound getNewSpecialItem/
addCraftingRecipe {JunimoPortalRecipe}/
pause 3300/
message ""I learned how to craft a 'Junimo Portal'""/
playMusic none/
shake Wizard 1500/
speak Wizard ""Enticing a Junimo to *use* it, well, that's up to the Junimo...""/
globalFade/
viewport -1000 -1000/
message ""Usage: After completing quests to get junimo helpers, you can place Junimo Portals either in buildings or outside.  If outside, place walkways between the hut and any chests or machines you want the Junimo to automate.  If in a building, you can place walkways or just leave a clear path.""/
end warpOut");

            eventData[IF($"{JunimoPortalDiscoveryEvent}/H/i {OldJunimoPortalQiid}")]
                = commonPart1 + (this.mod.IsRunningSve ? svePart2 : stockPart2) + commonPart3;
        }

        private void GameLoop_DayEnding(object? sender, DayEndingEventArgs e)
        {
            if (Game1.isRaining
                && Game1.IsMasterGame
                && Game1.Date.TotalDays >= 7
                && !Game1.MasterPlayer.modData.ContainsKey(ModDataKey_PlacedOldPortal)
                && !this.IsUnlocked
                && !Game1.player.questLog.Any(q => q.id.Value == OldJunimoPortalQuest))
            {
                this.PlacePortalRemains();
            }
        }

        private void GameLoop_DayStarted(object? sender, DayStartedEventArgs e)
        {
            if (ModEntry.Config.EnableWithoutQuests
                && Game1.player.IsMainPlayer
                && !Game1.player.questLog.Any(q => q.id.Value == OldJunimoPortalQuest)
                && !this.IsUnlocked)
            {
                Game1.player.addItemToInventory(ItemRegistry.Create(OldJunimoPortalQiid));
            }
            else if (Game1.MasterPlayer.modData.ContainsKey(ModDataKey_PlacedOldPortal)
                && !Game1.MasterPlayer.modData.ContainsKey(ModDataKey_AlertedPlayer)
                && !Game1.isRaining)
            {
                Game1.addHUDMessage(new HUDMessage(L("That was some storm!  I wonder if the rain washed the mud off of any of Grandpa's old stuff!")) { noIcon = true });
                Game1.MasterPlayer.modData[ModDataKey_AlertedPlayer] = Game1.Date.TotalDays.ToString();
            }
        }

        public void PlacePortalRemains()
        {
            var farm = Game1.getFarm();
            var existing = farm.objects.Values.FirstOrDefault(o => o.QualifiedItemId == OldJunimoPortalQiid);
            if (existing is not null)
            {
                // Perhaps this could happen if the save is passed to somebody else?
                this.LogError($"{OldJunimoPortalQiid} is already placed at {existing.TileLocation.X},{existing.TileLocation.Y}");
                Game1.MasterPlayer.modData[ModDataKey_PlacedOldPortal] = existing.TileLocation.ToString();
                return;
            }

            bool isObscured(Vector2 tile) => farm.isBehindTree(tile) || farm.isBehindBush(tile); // << TODO: behind building

            var weedLocations = farm.objects.Pairs.Where(pair => pair.Value.QualifiedItemId == "(O)784" /* weed*/ && !isObscured(pair.Value.TileLocation) && farm.isTilePassable(pair.Value.TileLocation)).Select(pair => pair.Key).ToArray();
            Vector2 position;
            if (weedLocations.Any())
            {
                position = Game1.random.Choose(weedLocations);
            }
            else
            {
                var visibleGrassPlots = farm.terrainFeatures.Values.OfType<Grass>().Where(grass => !isObscured(grass.Tile) && farm.isTilePassable(grass.Tile)).ToList();
                if (!visibleGrassPlots.Any())
                {
                    // TODO: Try and find some clear ground or just pick a random spot.
                    this.LogWarning($"No weeds or grass on farm, can't place the old junimo portal");
                    return;
                }

                position = visibleGrassPlots[Game1.random.Next(visibleGrassPlots.Count)].Tile;
                farm.terrainFeatures.Remove(position);
            }

            var o = ItemRegistry.Create<StardewValley.Object>(OldJunimoPortalQiid);
            o.questItem.Value = true;
            o.Location = Game1.getFarm();
            o.TileLocation = position;
            this.LogInfoOnce($"{OldJunimoPortalQiid} placed at {position.X},{position.Y}");
            o.IsSpawnedObject = true;
            farm.objects[o.TileLocation] = o;

            Game1.MasterPlayer.modData[ModDataKey_PlacedOldPortal] = position.ToString();
        }

        public void WriteToLog(string message, LogLevel level, bool isOnceOnly)
        {
            this.mod.WriteToLog(message, level, isOnceOnly);
        }
    }
}
