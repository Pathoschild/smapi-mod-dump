/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewArchipelago.Constants.Vanilla;
using StardewValley;
using StardewValley.Locations;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla.Relationship
{
    public class Friends
    {
        public const string PET_NAME = "Pet";

        private List<ArchipelagoFriend> _friends;
        private Dictionary<string, ArchipelagoFriend> _friendsByName;
        private int _lastRefreshDay;

        public Friends()
        {
            _lastRefreshDay = -1;
            RefreshFriends();
            _friendsByName = new Dictionary<string, ArchipelagoFriend>();
        }

        private void RefreshFriends()
        {
            if (Game1.stats.DaysPlayed == _lastRefreshDay)
            {
                return;
            }

            _friends = new List<ArchipelagoFriend>();
            var npcs = DataLoader.Characters(Game1.content);
            foreach (var (name, npcData) in npcs)
            {
                var gender = npcData.Gender;
                var datable = npcData.CanBeRomanced;
                var spawnLocation = npcData.HomeRegion;
                var spawnsOnIsland = IsIslandLocation(spawnLocation);
                var apName = NameAliases.NPCNameAliases.ContainsKey(name) ? NameAliases.NPCNameAliases[name] : name;

                var friend = new ArchipelagoFriend(name, apName, datable, false, spawnsOnIsland, name.Contains("Dwarf"), false);
                _friends.Add(friend);
            }

            _lastRefreshDay = (int)Game1.stats.DaysPlayed;
        }

        private bool IsIslandLocation(string spawnLocation)
        {
            var location = Game1.getLocationFromName(spawnLocation);
            if (location == null)
            {
                return false;
            }

            return location is IslandLocation;
        }

        public ArchipelagoFriend GetFriend(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            if (!_friendsByName.ContainsKey(name))
            {
                RefreshFriends();
                var friend = _friends.FirstOrDefault(x => x.StardewName == name || x.ArchipelagoName == name);
                if (friend == null)
                {
                    friend = TryFindKidWithThatName(name);
                    if (friend == null)
                    {
                        friend = GetFriend(name.Substring(0, name.Length - 1));
                        if (friend == null)
                        {
                            return null;
                        }
                    }
                }

                _friendsByName.Add(name, friend);
            }

            return _friendsByName[name];
        }

        private static ArchipelagoFriend TryFindKidWithThatName(string name)
        {
            var kids = Game1.player.getChildren();
            for (var kidIndex = 0; kidIndex < kids.Count; kidIndex++)
            {
                var kid = kids[kidIndex];
                if (kid.Name != name)
                {
                    continue;
                }

                var firstOrSecond = kidIndex % 2 == 0 ? "First" : "Second";
                return new ArchipelagoFriend(kid.Name, $"{firstOrSecond} Child", false, false, false,
                    false, true);
            }

            return null;
        }

        public ArchipelagoFriend GetFriend(Friendship friendship)
        {
            return GetFriend(GetNpcName(friendship));
        }

        private string GetNpcName(Friendship friendship)
        {
            var farmer = Game1.player;
            foreach (var name in farmer.friendshipData.Keys)
            {
                if (ReferenceEquals(farmer.friendshipData[name], friendship))
                {
                    return name;
                }
            }

            return null;
        }

        public void AddPet(string petName)
        {
            if (_friendsByName.ContainsKey(petName))
            {
                return;
            }

            var pet = new ArchipelagoFriend(petName, PET_NAME, false, true, false, false, false);
            _friends.Add(pet);
            _friendsByName.Add(petName, pet);
        }
    }
}
