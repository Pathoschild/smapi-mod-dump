using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Linq;

namespace DVR
{
    public class Fish
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Times { get; set; }
        public string[] Locations { get; set; }
        public int MinLevel { get; set; }
        public int Difficulty { get; set; }
        public string Weather { get; set; }
        public int NumSeasons { get; set; }

        public bool NeedForCC { get; set; }
        public bool NeedForCollection { get; set; }

        public IEnumerable<string> GetTvText(ITranslationHelper t)
        {
            yield return $"{Name}{(NeedForCC?"=":"")}{(NeedForCollection ? "$" : "")}! {Description}";
            var or = t.Get("fish_or");
            var locs = string.Join(or, Locations.Select(x =>
            {
                var trans = t.Get($"loc_{x}");
                return trans.HasValue() ? trans.ToString() : t.Get("fish_default_preposition").ToString() + x;
            }));

            yield return $"{Name} {t.Get("fish_catch")} {locs} {t.Get("fish_from")} {string.Join(or, Times)}.";
        }
    }
    public static class Fishies
    {
        public static IList<Fish> GetFishForToday()
        {
            var fish = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            var neededItems = new List<int>();
            CommunityCenter cc = Game1.locations.OfType<CommunityCenter>().SingleOrDefault();
            

            // get fish still needed for cc bundles
            if (cc != null && !Game1.MasterPlayer.mailReceived.Contains("JojaMember") && !cc.areAllAreasComplete())
            {
                foreach (var b in Game1.content.Load<Dictionary<string, string>>("Data\\Bundles"))
                {
                    var bid = int.Parse(b.Key.Split('/')[1]);
                    var items = b.Value.Split('/')[2].Split(' ').GroupsOf(3).Select(y => y.First()).ToList();
                    for(var i = 0; i< items.Count; i++)
                    {
                        if (cc.bundles[bid][i]) continue;
                        var itemId = int.Parse(items[i]);
                        var objectToAdd = new StardewValley.Object(Vector2.Zero, itemId, 1);
                        if (objectToAdd.Name.Contains("rror") || objectToAdd.Category != -4)
                        {
                            continue;
                        }
                        neededItems.Add(itemId);
                    }
                }
            }

            var locations = Game1.content.Load<Dictionary<string, string>>("Data\\Locations")
                .Where(x => x.Key != "fishingGame" && x.Key != "Temp" && x.Key != "Backwoods");
            var fishField = 4 + Utility.getSeasonNumber(Game1.currentSeason);
            

            return fish.Where(x => !x.Value.Contains("/trap/"))
            .Select(x =>
            {
                var parts = x.Value.Split('/');
                var o = new StardewValley.Object(x.Key, 1);
                return new Fish
                {
                    ID = x.Key,
                    Name = parts[0],
                    Difficulty = int.Parse(parts[1]),
                    Description = o.getDescription().Replace("\r\n", ""),
                    Times = parts[5].Split(' ').GroupsOf(2).Select(y => string.Join("-", y)).ToArray(),
                    MinLevel = int.Parse(parts[12]),
                    Weather = parts[7],
                    NumSeasons = parts[6].Split(' ').Length,
                    NeedForCC = neededItems.Contains(x.Key),
                    NeedForCollection = !Game1.player.fishCaught.ContainsKey(x.Key),
                    Locations = locations.Where(y => {
                        var fishes = y.Value.Split('/')[fishField];
                        return fishes.Contains(x.Key.ToString());
                    }).Select(y => y.Key).ToArray(),
                };
            })
            // filter for today's weather
            .Where(x => x.Weather == "both" || (x.Weather == "rainy") == (Game1.isRaining))
            // filter for those with valid locations this season
            .Where(x => x.Locations.Length > 0)

            .OrderByDescending(x => x.NeedForCC)        // all cc fish first
            .ThenByDescending(x => x.NeedForCollection || x.NeedForCC) // then unmet collection requirements
            .ThenByDescending(x => x.Weather == "rainy") // rainy day fish higher on rainy days get highest priority
            .ThenBy(x => x.NumSeasons)                  // fewer seasons makes it more urgent
            .ThenBy(x => x.Difficulty)                  // easiest first
            .ToList();
        }

        private static IEnumerable<IEnumerable<T>> GroupsOf<T>(this IEnumerable<T> l, int n)
        {
            var idx = 0;
            return l.GroupBy(x => idx++ / n);
        }
    }
}
