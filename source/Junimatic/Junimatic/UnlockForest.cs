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
using System.Data;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;

namespace NermNermNerm.Junimatic
{
    public class UnlockForest
    {
        private const string LightCampfireEventCommand = "Junimatic.LightCampFires";
        private const string FocusOnMysticTreeEventCommand = "Junimatic.FocusOnMysticTree";
        private const string JunimosSpringFromTreeEventCommand = "Junimatic.JunimosSpringFromTree";
        private const string IsMysticTreeGrownOnFarmEventCondition = "IsMysticTreeGrownOnFarm";
        private const string GrowMysticTreeQuest = "Junmatic.GrowMysticTree";
        private const string MeetLinusMailKey = "Junimatic.MeetLinus";
        private const string MeetLinusInWoodsQuestKey = "Junimatic.CookOutWithLinus";
        private const string LinusCampingEvent = "Junimatic.LinusCamping";
        private const string MysticTreeCelebrationEvent = "Junimatic.MysticTreeCelebration";
        private const string TempMarker = "Junimatic.TemporaryEventMarker";

        private ModEntry mod = null!;

        public void Entry(ModEntry mod)
        {
            this.mod = mod;

            mod.Helper.Events.Content.AssetRequested += this.OnAssetRequested;
            mod.Helper.Events.Player.Warped += this.Player_Warped;
            mod.Helper.Events.GameLoop.DayEnding += this.OnDayEnding;

            Event.RegisterCommand(LightCampfireEventCommand, this.LightCampfire);
            Event.RegisterCommand(FocusOnMysticTreeEventCommand, this.FocusOnMysticTree);
            Event.RegisterCommand(JunimosSpringFromTreeEventCommand, this.JunimosSpringFromTree);
            Event.RegisterPrecondition(IsMysticTreeGrownOnFarmEventCondition, this.IsMysticTreeGrownOnFarm);
        }

        public bool IsUnlocked => ModEntry.Config.EnableWithoutQuests || Game1.MasterPlayer.eventsSeen.Contains(MysticTreeCelebrationEvent);

        private void JunimosSpringFromTree(Event @event, string[] args, EventContext context)
        {
            try
            {
                var tree = context.Location.terrainFeatures.Values.OfType<Tree>().Where(t => t.growthStage.Value >= Tree.treeStage && t.treeType.Value == Tree.mysticTree).First();
                Vector2[] vectors = new Vector2[] { new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
                for (int i = 0; i < vectors.Length; ++i)
                {
                    var junimo = new EventJunimo(tree.Tile, vectors[i]);
                    @event.actors.Add(junimo);
                }
            }
            finally
            {
                @event.CurrentCommand++;
            }
        }

        private void FocusOnMysticTree(Event @event, string[] split, EventContext context)
        {
            try
            {
                var mysticTree = context.Location.terrainFeatures.Values.OfType<Tree>().First(t => t.growthStage.Value >= Tree.treeStage && t.treeType.Value == Tree.mysticTree);

                int xTile = (int)mysticTree.Tile.X;
                int yTile = (int)mysticTree.Tile.Y;
                Game1.viewportFreeze = true;
                Game1.viewport.X = xTile * 64 + 32 - Game1.viewport.Width / 2;
                Game1.viewport.Y = yTile * 64 + 32 - Game1.viewport.Height / 2;
                if (Game1.viewport.X > 0 && Game1.viewport.Width > Game1.currentLocation.Map.DisplayWidth)
                {
                    Game1.viewport.X = (Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width) / 2;
                }

                if (Game1.viewport.Y > 0 && Game1.viewport.Height > Game1.currentLocation.Map.DisplayHeight)
                {
                    Game1.viewport.Y = (Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height) / 2;
                }
            }
            finally
            {
                @event.CurrentCommand++;
            }
        }

        private bool IsMysticTreeGrownOnFarm(GameLocation location, string eventId, string[] args)
        {
            bool hasMysticTree = location.terrainFeatures.Values.OfType<Tree>().Any(t => t.growthStage.Value >= Tree.treeStage && t.treeType.Value == Tree.mysticTree);
            int numMahogonies = location.terrainFeatures.Values.OfType<Tree>().Count(t => t.growthStage.Value >= Tree.treeStage && t.treeType.Value == Tree.mahoganyTree);
            return hasMysticTree && numMahogonies >= 2;
        }

        private void OnDayEnding(object? sender, DayEndingEventArgs e)
        {
            if (Game1.IsMasterGame && this.mod.UnlockPortalQuest.IsUnlocked && Game1.player.foragingLevel.Value >= 6 && Game1.player.getFriendshipHeartLevelForNPC("Linus") >= 6 && !Game1.player.hasOrWillReceiveMail(MeetLinusMailKey))
            {
                Game1.player.mailForTomorrow.Add(MeetLinusMailKey);
            }
        }

        private void Player_Warped(object? sender, WarpedEventArgs e)
        {
            if (e.OldLocation.Name == I("Woods"))
            {
                var oldTorchLocations = e.OldLocation.Objects.Values.Where(o => o.modData.ContainsKey(TempMarker)).Select(t => t.TileLocation).ToList();
                foreach (var oldCampfire in oldTorchLocations)
                {
                    e.OldLocation.Objects.Remove(oldCampfire);
                }
            }
        }

        private void LightCampfire(Event @event, string[] args, EventContext context)
        {
            if (!ArgUtility.TryGetVector2(args, 1, out var atTile, out string? error, integerOnly: true))
            {
                context.LogErrorAndSkip(error);
                return;
            }

            string? campfireId = args.Length > 3 ? args[3] : null;

            try
            {
                if (campfireId is null)
                {
                    // Clean up the old fire.
                    if (context.Location.Objects.TryGetValue(atTile, out var objectAtTile) && objectAtTile is Torch)
                    {
                        context.Location.Objects.Remove(atTile);
                    }
                }
                else
                {
                    var campfire = new Torch(campfireId, true);
                    context.Location.objects[atTile] = campfire;
                    campfire.TileLocation = @event.OffsetTile(atTile);
                    campfire.initializeLightSource(campfire.TileLocation);
                    campfire.modData["Junimatic.TempMarker"] = true.ToString();
                }
            }
            finally
            {
                @event.CurrentCommand++;
                @event.Update(context.Location, context.Time);
            }
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/Farm"))
            {
                e.Edit(editor => this.EditFarmEvents(editor.AsDictionary<string, string>().Data));
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/Woods"))
            {
                e.Edit(editor => this.EditWoodsEvents(editor.AsDictionary<string, string>().Data));
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Quests"))
            {
                e.Edit(editor =>
                {
                    IDictionary<string, string> data = editor.AsDictionary<string, string>().Data;
                    data[GrowMysticTreeQuest] = SdvQuest("Basic/Plant The Mystic Seed/Linus gave you some tree seeds...  Do the Junimos talk to him too?/Grow a Mystic Tree and 2 Mahogony Trees to adulthood on your farm./null/-1/0/-1/false");
                    data[MeetLinusInWoodsQuestKey] = SdvQuest("Basic/Meet Linus In The Secret Woods/Have dinner with Linus in the Secret Woods/Enter the secret woods on a sunny day between 6 and 11pm./null/-1/0/-1/false");
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Mail"))
            {
                e.Edit(editor =>
                {
                    IDictionary<string, string> data = editor.AsDictionary<string, string>().Data;
                    data[MeetLinusMailKey] = SdvMail($"@,^how are you doing?  I've decided to spend a night or two in the deep woods, west of Marnie's ranch.  Would you care to share a meal in the wild with me? ^   -Linus%item quest {MeetLinusInWoodsQuestKey}%%[#]Meet at Linus' camp in the woods");
                });
            }
        }

        private void EditFarmEvents(IDictionary<string, string> eventData)
        {
            eventData[IF($"{MysticTreeCelebrationEvent}/H/sawEvent {LinusCampingEvent}/{IsMysticTreeGrownOnFarmEventCondition}")] = SdvEvent($@"playful
-1000 -1000
farmer 8 24 0
removeQuest {GrowMysticTreeQuest}
skippable
{FocusOnMysticTreeEventCommand}
pause 2000
{JunimosSpringFromTreeEventCommand}
pause 2000
spriteText 4 ""The giant trees were cut down to make ships and buildings and other wonderful things...""
spriteText 4 ""...but they cut like there would always be more, until they cut down the last one.""
spriteText 4 ""You will be wiser.  We will help.""
{UnlockCropMachines.EventCustomCommandJunimosDisappear}
spriteText 4 ""Thx!  Bai!!!""
pause 2000
end
").Replace("\r", "").Replace("\n", "/");
        }

        private void EditWoodsEvents(IDictionary<string, string> eventData)
        {
            (int modDeltaX,int modDeltaY) = this.mod.IsRunningSve ? (40, 15) : (0,0);

            eventData[IF($"{LinusCampingEvent}/H/w sunny/t 1800 2300/n {MeetLinusMailKey}")] = SdvEvent(@$"nightTime
-1000 -1000
farmer {40+modDeltaX} {14+modDeltaY} 3 Linus {29+modDeltaX} {13+modDeltaY} 1
removeQuest {MeetLinusInWoodsQuestKey}

makeInvisible {28+modDeltaX} {7+modDeltaY} 4 9
viewport {27+modDeltaX} {12+modDeltaY} true

temporaryAnimatedSprite ""LooseSprites\Cursors_1_6"" 48 208 64 48 999999 1 0  {28+modDeltaX} {9+modDeltaY}  false false 10 0 1 0 0 0/
temporaryAnimatedSprite ""LooseSprites\Cursors_1_6"" 0 192 48 64  999999 1 0  {29+modDeltaX} {7+modDeltaY}  false false 11 0 1 0 0 0/
Junimatic.LightCampFires {30+modDeltaX} {13+modDeltaY} 278
setSkipActions addItem (O)MysticTreeSeed 1#addItem (O)292 2#addQuest {GrowMysticTreeQuest}
skippable

textAboveHead Linus ""Welcome!  You're just in time!""
move farmer -7 0 3
pause 2000
textAboveHead Linus ""I hiked the back-woods trail today.""

animate Linus false true 250 20 21
pause 500
playSound clank
pause 500
stopAnimation Linus

textAboveHead Linus ""It can be a bit treacherous at my age...""
move farmer -2 0 3
pause 1000

animate Linus false true 250 20 21
pause 500
playSound clank
pause 500
stopAnimation Linus

textAboveHead Linus ""But the view of the desert is breathtaking.""
pause 2000

animate Linus false true 250 20 21
pause 500
playSound clank
pause 500
stopAnimation Linus

textAboveHead Linus ""I picked a few wild mushrooms and herbs along the way.""
pause 1000

animate Linus false true 250 20 21
pause 500
playSound clank
pause 500
stopAnimation Linus

pause 2000
Junimatic.LightCampFires 30 13 146
playSound clank

textAboveHead Linus ""Dinner is served!""
pause 1500
move Linus 0 1 2
move Linus 1 0 1
animate Linus false true 250 20 21
pause 250
move Linus -1 0 1
pause 2000
farmerEat 199
move Linus 0 -1 0
faceDirection Linus 1
pause 1000
textAboveHead Linus ""What do you think?""
pause 2500
emote farmer 32
pause 1500

textAboveHead Linus ""I'm glad you like it.""
move farmer 0 -1 0
faceDirection farmer 3
pause 2000

speak Linus ""You know, when people see you talking to the trees, well...  They tend to think you're crazy.$2""
pause 500
eyes 1 500
pause 1000
speak Linus ""But when you hear the trees talking *to* *you*, then... well...#$b#Then maybe they're right.$2""
playSound owl
pause 500
eyes 1 500
pause 1000

move Linus -2 0 1
move Linus 0 -1 0
pause 3000
faceDirection Linus 1

speak Linus ""Last night I dreamt I saw what these trees looked like before they were cut...""
move Linus 0 1 1
move Linus 2 0 1

speak Linus ""Then, the vision pivoted to your farm; your farm when those big stumps and logs were standing trees...""

pause 1000
move Linus 0 -1 0
move Linus 1 0 1
move Linus 0 -1 0
playsound leafrustle
pause 1000
move Linus 0 1 2
move Linus 1 0 1
faceDirection Linus 2

speak Linus ""I found these seeds in my tent as I was stepping out this morning.#$b#I guess I tracked them in last night.  They're usually not so easy to find.  Here.  I think they're meant for you.""
addItem (O)MysticTreeSeed 1
addItem (O)292 2/ -- mahogony seed
setSkipActions addQuest {GrowMysticTreeQuest}
pause 2000
move Linus -2 0 3
playSound owl
move Linus 0 1 2
faceDirection Linus 1
pause 4000
speak Linus ""I'm glad you're the sort of friend that doesn't mind listening to the ravings of a crazy old man.""
pause 3000
eyes 1 500
pause 2000
eyes 1 500
pause 2000
speak Linus ""Well...  It's getting late.  You should get back to your farm and I should crawl into my tent.#$b#I plan on breaking camp in the morning and making the hike up to the mountain.""
pause 1000
playSound owl
move farmer 0 1 2
move farmer 3 0 1
speak Linus ""Do the trees talk to you too?""
faceDirection farmer 3
speak Linus ""Hm.  Perhaps it's best not to answer that one...""
pause 1500
eyes 1 500
pause 1000
move farmer 5 0 1 true
pause 1000
fade
viewport -1000 -1000 true
Junimatic.LightCampFires 30 13
addQuest {GrowMysticTreeQuest}
end warpOut
").Replace("\r", "").Replace("\n", "/");
        }
    }
}
