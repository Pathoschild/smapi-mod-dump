using System;
using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using StardewModdingAPI.Events;
using StardewValley;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public class GiftPreferencesService
    {
        #region Constants

        private const string UniversalLove = "Universal_Love";
        private const string UniversalLike = "Universal_Like";
        private const string UniversalNeutral = "Universal_Neutral";
        private const string UniversalDislike = "Universal_Dislike";
        private const string UniversalHate = "Universal_Hate";
        private static readonly string[] UniversalPreferences = { UniversalDislike, UniversalHate, UniversalLike, UniversalNeutral, UniversalLove };

        #endregion

        #region Private Data

        private readonly List<GiftPreferences> _addedPreferences = new List<GiftPreferences>();

        private readonly List<GiftPreferences> _removedPreferences = new List<GiftPreferences>();

        private List<GiftPreferences> GiftPreferenceses { get; } = new List<GiftPreferences>();

        #endregion

        #region Singleton Access

        private GiftPreferencesService()
        {
            GameEvents.LoadContent += OnLoadContent;
        }

        private static GiftPreferencesService _instance;

        public static GiftPreferencesService Instance => _instance ?? (_instance = new GiftPreferencesService());

        #endregion

        #region	Public Methods

        public void AddGiftPreferences(GiftPreferences preferences)
        {
            _addedPreferences.Add(preferences);
        }

        public void RemoveGiftPreferences(GiftPreferences preferences)
        {
            _removedPreferences.Add(preferences);
        }

        #endregion

        #region	Auxiliary Methods

        private void OnLoadContent(object sender, EventArgs e)
        {
            GiftPreferenceses.Add(new GiftPreferences
            {
                Loved = GiftPreferences.PartToIDs(Game1.NPCGiftTastes[UniversalLove]),
                Liked = GiftPreferences.PartToIDs(Game1.NPCGiftTastes[UniversalLike]),
                Disliked = GiftPreferences.PartToIDs(Game1.NPCGiftTastes[UniversalDislike]),
                Hated = GiftPreferences.PartToIDs(Game1.NPCGiftTastes[UniversalHate]),
                Neutral = GiftPreferences.PartToIDs(Game1.NPCGiftTastes[UniversalNeutral])
            });

            foreach (var kv in Game1.NPCGiftTastes)
            {
                if (UniversalPreferences.Contains(kv.Key)) continue;
                GiftPreferenceses.Add(CharacterGiftPreferences.Parse(kv.Value, kv.Key));
            }

            // TODO.
            foreach (var addedPreference in _addedPreferences)
            {
                var preference = GiftPreferenceses.First(p => p.Name == addedPreference.Name);
                preference.Disliked.AddRange(addedPreference.Disliked ?? Empty);
                preference.Hated.AddRange(addedPreference.Hated ?? Empty);
                preference.Loved.AddRange(addedPreference.Loved ?? Empty);
                preference.Liked.AddRange(addedPreference.Liked ?? Empty);
                preference.Neutral.AddRange(addedPreference.Neutral ?? Empty);
                Game1.NPCGiftTastes[preference.Name.ToString()] = preference.ToString();
            }
        }

        private static IEnumerable<DynamicID<ItemID, CategoryID>> Empty => Enumerable.Empty<DynamicID<ItemID, CategoryID>>();

        #endregion
    }
}
