/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/Junimatic
**
*************************************************/

using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;

namespace NermNermNerm.Junimatic
{
    /// <summary>
    ///   This represents all the game content related to enabling the Junimo that
    ///   works Kegs, Casks and JellyJamJar machines.
    /// </summary>
    public class UnlockCropMachines : ISimpleLog
    {
        private ModEntry mod = null!;

        public UnlockCropMachines() { }

        private const string GiantCropCelebrationEventId = "Junimatic.CropMachineHelper.GiantCropCelebration";
        private const string EventCustomConditionGiantCropIsGrowingOnFarm = "Junimatic.GiantCropIsGrowingOnFarm";
        private const string EventCustomCommandFocusOnGiantCrop = "Junimatic.FocusOnGiantCrop";
        private const string EventCustomCommandSpringJunimosFromCrop = "Junimatic.SpringJunimosFromCrop";
        public const string EventCustomCommandJunimosDisappear = "Junimatic.JunimosDisappear";

        public const string ConversationKeyBigCrops = "Junimatic.BigCrops";

        public void Entry(ModEntry mod)
        {
            mod.Helper.Events.Content.AssetRequested += this.OnAssetRequested;

            Event.RegisterPrecondition(EventCustomConditionGiantCropIsGrowingOnFarm, this.GiantCropIsGrowingOnFarm);
            Event.RegisterCommand(EventCustomCommandFocusOnGiantCrop, this.FocusOnGiantCrop);
            Event.RegisterCommand(EventCustomCommandSpringJunimosFromCrop, this.SpringJunimosFromCrop);
            Event.RegisterCommand(EventCustomCommandJunimosDisappear, this.JunimosDisappear);
        }

        public bool IsUnlocked => ModEntry.Config.EnableWithoutQuests || Game1.MasterPlayer.eventsSeen.Contains(GiantCropCelebrationEventId);

        private bool GiantCropIsGrowingOnFarm(GameLocation location, string eventId, string[] args)
            => location.resourceClumps.OfType<GiantCrop>().Any();

        private void FocusOnGiantCrop(Event @event, string[] split, EventContext context)
        {
            try
            {
                var crop = context.Location.resourceClumps.OfType<GiantCrop>().First();

                int xTile = (int)crop.Tile.X;
                int yTile = (int)crop.Tile.Y;
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

        private void SpringJunimosFromCrop(Event @event, string[] split, EventContext context)
        {
            try
            {
                var crop = context.Location.resourceClumps.OfType<GiantCrop>().First();
                // Let's see how we get on if we just jump any old place regardless of crap in the way.
                Vector2[] vectors = new Vector2[] { new Vector2(-2, 0), new Vector2(-2, -2), new Vector2(0, -2), new Vector2(2,-1), new Vector2(2,0), new Vector2(2,2), new Vector2(0,2), new Vector2(-1,2)};
                for (int i = 0; i < vectors.Length; ++i)
                {
                    var junimo = new EventJunimo(crop.Tile+new Vector2(1,1), vectors[i]);
                    @event.actors.Add(junimo);
                }
            }
            finally
            {
                @event.CurrentCommand++;
            }
        }

        private void JunimosDisappear(Event @event, string[] split, EventContext context)
        {
            try
            {
                foreach (var junimo in @event.actors.OfType<EventJunimo>())
                {
                    junimo.GoBack();
                }
            }
            finally
            {
                @event.CurrentCommand++;
            }
        }

        private void OnAssetRequested(object? sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/Farm"))
            {
                e.Edit(editor =>
                {
                    var d = editor.AsDictionary<string, string>().Data;
                    d[IF($"{GiantCropCelebrationEventId}/H/sawEvent {UnlockPortal.JunimoPortalDiscoveryEvent}/{EventCustomConditionGiantCropIsGrowingOnFarm}")] = SdvEvent($@"playful/
-1000 -1000/
farmer 8 24 0/
skippable/
{EventCustomCommandFocusOnGiantCrop}/
pause 2000/
{EventCustomCommandSpringJunimosFromCrop}/
pause 2000/
spriteText 4 ""We love giant crops!  Please keep growing them!""/
spriteText 4 ""One of us will come and help with your kegs, casks and preserves jars if you connect them to a portal!""/
{EventCustomCommandJunimosDisappear}/
spriteText 4 ""Thx!  Bai!!!""/
pause 2000/
end
");
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Caroline"))
            {
                e.Edit(editor =>
                {
                    var data = editor.AsDictionary<string, string>().Data;
                    data[ConversationKeyBigCrops] = L("Did you ever see the giant crops your Granddad used to crow?  Crazy big pumpkins.#$b#Abby campaigned for me to buy one every year.  I think she wanted me to buy one so she could carve out a house for herself.$1");
                    ConversationKeys.EditAssets(e.NameWithoutLocale, editor.AsDictionary<string, string>().Data);
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Pierre"))
            {
                e.Edit(editor =>
                {
                    var data = editor.AsDictionary<string, string>().Data;
                    data[ConversationKeyBigCrops] = L("Has Abby been needling you about growing giant pumpkins?$4#$b#Don't do it, really, there's no money in it.  Quantity is good, sure, but the quality isn't.$2#$b#Your grandpa grew them in his declining years in particular.  He said 'my helpers like them'.  Not sure what he meant by that, but he really enjoyed them that made it worth it to him!$1");
                    ConversationKeys.EditAssets(e.NameWithoutLocale, editor.AsDictionary<string, string>().Data);
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Maru"))
            {
                e.Edit(editor =>
                {
                    var data = editor.AsDictionary<string, string>().Data;
                    data[ConversationKeyBigCrops] = L("When you were a kid visiting the farm, did you ever see any of the giant cauliflowers that your grandpa grew?  I mean huge!  like way huge!$1#$b#I only got to see them once that I can recall when my Mom took me down there one afternoon.#$b#I climbed one like a tree and my Mom got all worried.#$b#Your grandad fished me down and told my mom not to worry because magical creatures would protect me.#$b#I don't know why, I guess it's because I was just a kid, but I felt really special for a long time after that day.");
                    ConversationKeys.EditAssets(e.NameWithoutLocale, editor.AsDictionary<string, string>().Data);
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Abigail"))
            {
                e.Edit(editor =>
                {
                    var data = editor.AsDictionary<string, string>().Data;
                    data[ConversationKeyBigCrops] = L("Your grandad used to grow these gigantic pumpkins.  I totally wanted to carve one of those things.$1#$b#But my mom wouldn't buy one and your grandad invented this cock&bull story about magical creatures.$5#$b#Didn't he know I was too old for stories like that?  Meh.  I guess I should cut him slack for being old and senile.$2#$b#So...  Do you want to hear about any more of my first-world problems?$2");
                    ConversationKeys.EditAssets(e.NameWithoutLocale, editor.AsDictionary<string, string>().Data);
                });
            }
            // Penny talking about melons?  Doesn't seem quite right somehow.
        }

        public void WriteToLog(string message, LogLevel level, bool isOnceOnly)
        {
            this.mod.WriteToLog(message, level, isOnceOnly);
        }
    }
}
