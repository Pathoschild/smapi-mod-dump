/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/Junimatic
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Extensions;

using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;

namespace NermNermNerm.Junimatic
{
    /// <summary>
    ///   A modlet that makes it so the pet will sometimes hang out near quest-started items.
    /// </summary>
    public class PetFindsThings
        : ISimpleLog
    {
        private ModEntry mod = null!;

        private readonly List<Func<IEnumerable<(Point tileLocation, double chance)>>> finders = new();

        // Using a distinct mod key, in the event this gets split out
        private static readonly string PetSawItemConversationKey = "PetFindsThings.PetSightedAnObject";

        private record IdAndPoint(string Id, Point Point)
        {
            public override string ToString() => FormattableString.Invariant($"{this.Point.X},{this.Point.Y},{this.Id}");

            public static IdAndPoint? FromString(string serialized)
            {
                string[] splits = serialized.Split(",", 3);
                if (splits.Length == 3 && int.TryParse(splits[0], out int x) && int.TryParse(splits[1], out int y))
                {
                    return new IdAndPoint(splits[2], new Point(x, y));
                }
                else
                {
                    return null;
                }
            }
        }

        public PetFindsThings() { }

        /// <summary>
        ///   Called during Entry, this adds a new function for pointing out places that the pet could go to.
        /// </summary>
        public void AddObjectFinder(Func<IEnumerable<(Point tileLocation, double chance)>> newFinder)
        {
            this.finders.Add(newFinder);
        }

        public void AddObjectFinder(string hiddenObjectItemId, double chance)
        {
            this.AddObjectFinder(() =>
                Game1.currentLocation.objects.Values
                    .Where(o => o.ItemId == hiddenObjectItemId || o.QualifiedItemId == hiddenObjectItemId)
                    .Select<StardewValley.Object, (Point tileLocation, double chance)>(o => new() { tileLocation = o.TileLocation.ToPoint(), chance = chance }));
        }

        public void Entry(ModEntry mod)
        {
            this.mod = mod;
            mod.Helper.Events.Player.Warped += this.Player_Warped;
            mod.Helper.Events.Content.AssetRequested += this.Content_AssetRequested;
        }

        private void Content_AssetRequested(object? sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Marnie"))
            {
                e.Edit(editor =>
                {
                    IDictionary<string, string> data = editor.AsDictionary<string, string>().Data;
                    data[PetSawItemConversationKey] = L("Pets sometimes have an uncanny ability to spot missing things!$0#$b#Just last week I lost my favorite milking bucket.  I came across it a few days later and my cat, Muffin, was sleeping in it.$1#$b#Well, I guess she didn't exactly find it for me, but at least she knew where it was!$0");
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Linus"))
            {
                e.Edit(editor =>
                {
                    IDictionary<string, string> data = editor.AsDictionary<string, string>().Data;
                    data["winter_Sun4"] = L(@"Do I miss my ""normal"" life on days like this?$2#$b#No, not really.  Except for Jeremy Clarkson...$1#$b#...My pet Schnauzer.  He had an uncanny ability to fetch the thing I wanted before I even knew I wanted it.$0");
                });
            }
        }

        private void Player_Warped(object? sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            var petInScene = e.NewLocation.characters.OfType<Pet>().FirstOrDefault();
            if (petInScene is null
                || Game1.hudMessages.Any() // <- can protect against duplicate versions of this code trying to do the same thing
                || Game1.getOnlineFarmers().Any(f => f != e.Player && f.currentLocation == e.NewLocation))
            {
                return;
            }

            double chance = Game1.random.NextDouble();
            List<Point> interestingPoints = new();
            foreach (var finder in this.finders)
            {
                interestingPoints.AddRange(finder().Where(result => chance < result.chance).Select(result => result.tileLocation));
            }

            if (!interestingPoints.Any())
            {
                return;
            }

            Point find = Game1.random.Choose(interestingPoints.ToArray());
            bool isObscured(Vector2 tile) => e.NewLocation.isBehindTree(tile) || e.NewLocation.isBehindBush(tile); // << TODO: behind building

            var openTiles = new List<Vector2>();
            for (int deltaX = -2; deltaX < 3; ++deltaX)
            {
                for (int deltaY = -2; deltaY < 3; ++deltaY)
                {
                    var tile = new Vector2(find.X + deltaX, find.Y + deltaY);
                    if (e.NewLocation.CanItemBePlacedHere(tile) && e.NewLocation.getObjectAt((int)tile.X, (int)tile.Y) is null && !e.NewLocation.terrainFeatures.ContainsKey(tile))
                    {
                        openTiles.Add(tile);
                    }
                }
            }

            if (!openTiles.Any())
            {
                this.LogWarning($"Can't put pet at {find} because the area is too crowded.");
                return;
            }

            var nonObscuredTiles = openTiles.Where(t => !isObscured(t)).ToArray();
            Vector2 landingTile = nonObscuredTiles.Any() ? Game1.random.Choose(nonObscuredTiles) : Game1.random.Choose(openTiles.ToArray());
            petInScene.Position = landingTile * 64;

            Game1.addHUDMessage(new HUDMessage(LF($"I wonder what {petInScene.Name} has been up to...")) { noIcon = true });
            Game1.player.activeDialogueEvents[PetSawItemConversationKey] = 30;
        }

        public void WriteToLog(string message, LogLevel level, bool isOnceOnly)
            => this.mod.WriteToLog(message, level, isOnceOnly);
    }
}
