using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using StardewValley;
using StardewValley.Tools;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class GiveAGiftChore : BaseChore
    {
        private NPC _todayBirthdayNpc;
        private readonly string _giftType;
        private readonly int _maxGifts;
        private readonly bool _enableUniversal;
        private readonly double _chanceForLove;
        private readonly List<int> _universalLoves = new List<int>();
        private readonly List<int> _universalLikes = new List<int>();
        private Dictionary<int, string> _items = new Dictionary<int, string>();
        private int _giftsGiven;

        public GiveAGiftChore(ChoreData choreData) : base(choreData)
        {
            ChoreData.Config.TryGetValue("GiftType", out var giftType);
            ChoreData.Config.TryGetValue("MaxGifts", out var maxGifts);
            ChoreData.Config.TryGetValue("EnableUniversal", out var enableUniversal);
            ChoreData.Config.TryGetValue("ChanceForLove", out var chanceForLove);
            
            _giftType = giftType is string s && !string.IsNullOrWhiteSpace(s) ? s : "Birthday";
            _maxGifts = maxGifts is int n ? n : 1;

            if (!_giftType.Equals("Birthday", StringComparison.CurrentCultureIgnoreCase))
                return;

            _enableUniversal = enableUniversal is bool b && b;
            _chanceForLove = chanceForLove is double d ? d : 0.1;

            if (Game1.NPCGiftTastes.TryGetValue("Universal_Love", out var universalLoves))
                _universalLoves.AddRange(
                    from a in universalLoves.Split(' ')
                    select Convert.ToInt32(a, CultureInfo.InvariantCulture));

            if (Game1.NPCGiftTastes.TryGetValue("Universal_Like", out var universalLikes))
                _universalLikes.AddRange(
                    from a in universalLikes.Split(' ')
                    select Convert.ToInt32(a, CultureInfo.InvariantCulture));
        }

        public override bool CanDoIt(bool today = true)
        {
            _items.Clear();
            _giftsGiven = _maxGifts > 1 ? Game1.random.Next(1, _maxGifts) : 1;

            if (!_giftType.Equals("Birthday", StringComparison.CurrentCultureIgnoreCase))
                return true;

            if (today)
                _todayBirthdayNpc = Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth);
            else if (Game1.dayOfMonth < 28)
                _todayBirthdayNpc = Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth + 1);
            else
            {
                string nextSeason;
                switch (Game1.currentSeason)
                {
                    case "spring":
                        nextSeason = "summer";
                        break;
                    case "summer":
                        nextSeason = "fall";
                        break;
                    case "fall":
                        nextSeason = "winter";
                        break;
                    case "winter":
                        nextSeason = "spring";
                        break;
                    default:
                        nextSeason = Game1.currentSeason;
                        break;
                }
                _todayBirthdayNpc = Utility.getTodaysBirthdayNPC(nextSeason, 1);
            }

            return _todayBirthdayNpc != null && !_todayBirthdayNpc.getName().Equals(Game1.player.getSpouse().getName(), StringComparison.CurrentCultureIgnoreCase);
        }

        public override bool DoIt()
        {
            var r = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2 +
                               (int) Game1.player.UniqueMultiplayerID);
            
            var itemIds = new List<int>();

            if (!_giftType.Equals("Birthday", StringComparison.CurrentCultureIgnoreCase))
            { 
                itemIds.AddRange(
                    from itemId in _giftType.Split(' ')
                    select Convert.ToInt32(itemId, CultureInfo.InvariantCulture));
            }
            else
            {
                Game1.NPCGiftTastes.TryGetValue(_todayBirthdayNpc.getName(), out var personalGifts);

                var personalData = personalGifts?.Split('/');

                if (personalData == null)
                    return false;

                if (r.NextDouble() < _chanceForLove)
                {
                    itemIds.AddRange(
                        from s in personalData[1].Split(' ')
                        select Convert.ToInt32(s, CultureInfo.InvariantCulture));
                    if (_enableUniversal)
                        itemIds.AddRange(_universalLoves);
                }
                else
                {
                    itemIds.AddRange(
                        from s in personalData[3].Split(' ')
                        select Convert.ToInt32(s, CultureInfo.InvariantCulture));
                    if (_enableUniversal)
                        itemIds.AddRange(_universalLikes);
                }
            }

            var itemCats = itemIds.Where(itemId => itemId < 0).ToList();

            // Get objects by category
            var objectsFromCats = (
                from objectInfo in Game1.objectInformation.Select(objectInfo =>
                    new KeyValuePair<int, string[]>(objectInfo.Key, objectInfo.Value.Split('/')[3].Split(' ')))
                where objectInfo.Value.Length == 2 &&
                      itemCats.Contains(Convert.ToInt32(objectInfo.Value[1], CultureInfo.InvariantCulture))
                select objectInfo.Key).ToList();

            // Get objects by id
            var objectsFromIds = (
                from objectInfo in Game1.objectInformation
                where itemIds.Contains(objectInfo.Key)
                select objectInfo.Key).ToList();

            // Get unique objects from both lists
            var objects = (
                from objectInfo in Game1.objectInformation
                where objectsFromCats.Contains(objectInfo.Key) ||
                      objectsFromIds.Contains(objectInfo.Key)
                select objectInfo).ToList();

            // Store items to give to player
            foreach (var item in objects.Shuffle().Take(_giftsGiven))
            {
                _items.Add(item.Key, item.Value.Split('/')[0]);
            }

            return _items.Any();
        }

        public override IDictionary<string, Func<string>> GetTokens()
        {
            var tokens = base.GetTokens();
            tokens.Add("ItemName", GetItemName);
            tokens.Add("ItemId", GetItemId);
            tokens.Add("Birthday", GetBirthdayName);
            tokens.Add("BirthdayGender", GetBirthdayGender);
            tokens.Add("GiftsGiven", GetGiftsGiven);
            tokens.Add("WorkDone", GetGiftsGiven);
            tokens.Add("WorkNeeded", GetWorkNeeded);
            return tokens;
        }

        private string GetItemName() =>
            string.Join(", ", _items.Values);

        private string GetItemId() =>
            "[" + string.Join("][", _items.Keys) + "]";

        private static string GetBirthdayName() =>
            Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth)?.getName();

        private static string GetBirthdayGender() =>
            Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth)?.Gender == 1 ? "Female" : "Male";

        private string GetGiftsGiven() =>
            _items?.Count.ToString(CultureInfo.InvariantCulture);

        private string GetWorkNeeded() =>
            _giftsGiven.ToString(CultureInfo.InvariantCulture);
    }
}