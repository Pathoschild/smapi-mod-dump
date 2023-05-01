/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;

namespace StardewArchipelago.Locations.CodeInjections
{
    public static class FriendshipInjections
    {
        private const int POINTS_PER_HEART = 250;
        private const int POINTS_PER_PET_HEART = 200;
        private const string HEARTS_PATTERN = "{0}: 1 <3";
        private const string FRIENDSANITY_PATTERN = "Friendsanity: {0} {1} <3";

        private const string PET_NAME = "Pet";
        private static readonly string[] _bachelors = new[]
        {
            "Harvey", "Elliott", "Sam", "Alex", "Shane", "Sebastian",
            "Emily", "Haley", "Leah", "Abigail", "Penny", "Maru"
        };

        private static readonly string[]
            _notImmediatelyAccessible = new[] { "Leo", "Krobus", "Dwarf", "Sandy", "Kent" };

        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static Dictionary<string, double> _friendshipPoints = new();
        private static Texture2D _apLogoColor;
        private static Texture2D _apLogoBlue;
        private static Texture2D _apLogoWhite;
        private static Texture2D _apLogoBlack;

        private static string[] _hintedFriendshipLocations;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _helper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _apLogoColor = ArchipelagoTextures.GetColoredLogo(modHelper, 24, "color");
            _apLogoBlue = ArchipelagoTextures.GetColoredLogo(modHelper, 24, "blue");
            _apLogoWhite = ArchipelagoTextures.GetColoredLogo(modHelper, 24, "white");
            _apLogoBlack = ArchipelagoTextures.GetColoredLogo(modHelper, 24, "black");
            _hintedFriendshipLocations = Array.Empty<string>();
        }

        public static Dictionary<string, int> GetArchipelagoFriendshipPoints()
        {
            return _friendshipPoints.ToDictionary(x => x.Key, x => (int)Math.Round(x.Value));
        }

        public static string GetArchipelagoFriendshipPointsForPrinting(string characterName)
        {
            var points = GetFriendshipPoints(characterName);
            if (points <= 0)
            {
                return $"You have never met someone named {characterName}";
            }
            return $"{characterName}: {points} ({GetHearts(points)} <)";
        }

        public static void SetArchipelagoFriendshipPoints(Dictionary<string, int> values)
        {
            if (values == null)
            {
                _friendshipPoints = new Dictionary<string, double>();
                return;
            }

            _friendshipPoints = values.ToDictionary(x => x.Key, x => (double)x.Value);
        }

        public static void ResetArchipelagoFriendshipPoints()
        {
            _friendshipPoints = new Dictionary<string, double>();
        }

        public static bool GetPoints_ArchipelagoHearts_Prefix(Friendship __instance, ref int __result)
        {
            try
            {
                var name = GetNpcName(__instance);
                if (name == null)
                {
                    return true; // run original logic
                }

                var archipelagoHearts =
                    _archipelago.GetReceivedItemCount(string.Format(HEARTS_PATTERN, name));
                var maxShuffled = ShuffledUpTo(name);

                var friendshipPoints = archipelagoHearts * POINTS_PER_HEART;
                friendshipPoints = GetBoundedToCurrentRelationState(friendshipPoints, name);
                if (archipelagoHearts >= maxShuffled)
                {
                    var earnedPoints = (int)GetFriendshipPoints(name);
                    var earnedPointsAboveMaxShuffled = Math.Max(0, earnedPoints - (maxShuffled * POINTS_PER_HEART));
                    friendshipPoints += earnedPointsAboveMaxShuffled;
                }

                __result = friendshipPoints;
                var pointsField = _helper.Reflection.GetField<NetInt>(__instance, "points");
                pointsField.GetValue().Value = friendshipPoints;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetPoints_ArchipelagoHearts_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public SocialPage(int x, int y, int width, int height)
        public static void SocialPageCtor_CheckHints_Postfix(SocialPage __instance, int x, int y, int width, int height)
        {
            try
            {
                var hints = _archipelago.GetHints().Where(x => !x.Found && _archipelago.GetPlayerName(x.FindingPlayer) == _archipelago.SlotData.SlotName);
                var hintedLocationNames = hints.Select(hint => _archipelago.GetLocationName(hint.LocationId)).Where(hint => hint.StartsWith($"Friendsanity: ")).ToArray();
                _hintedFriendshipLocations = hintedLocationNames;
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(DrawNPCSlot_DrawEarnedHearts_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // private void drawNPCSlot(SpriteBatch b, int i)
        public static void DrawNPCSlot_DrawEarnedHearts_Postfix(SocialPage __instance, SpriteBatch b, int i)
        {
            try
            {
                var name = __instance.names[i] as string;
                //var hints = _archipelago.GetHints().Where(x => !x.Found && _archipelago.GetPlayerName(x.FindingPlayer) == _archipelago.SlotData.SlotName);
                //var hintedLocationNames = hints.Select(hint => _archipelago.GetLocationName(hint.LocationId)).Where(hint => hint.StartsWith($"Friendsanity: {name}"));
                var apPoints = (int)GetFriendshipPoints(name);
                var maxShuffled = ShuffledUpTo(name);
                var maxHeartForCurrentRelation = GetMaximumHeartsWithRelationState(name);
                var apHearts = apPoints / POINTS_PER_HEART;
                var spritesField = _helper.Reflection.GetField<List<ClickableTextureComponent>>(__instance, "sprites");
                var sprites = spritesField.GetValue();
                for (var index = 0; index < maxShuffled; ++index)
                {
                    var positionX = __instance.xPositionOnScreen + 320 - 2 + index * 32;
                    var smallHeartOffset = 28;
                    var positionY = sprites[i].bounds.Y + 64 - 28;
                    var position = new Vector2(positionX, positionY - smallHeartOffset);
                    var sourceRectangle = new Rectangle(0, 0, 24, 24);
                    var color = Color.White;
                    var texture = _apLogoColor;
                    if (index >= 10)
                    {
                        var reverseX = 9 - (index - 10);
                        positionX = __instance.xPositionOnScreen + 320 - 2 + reverseX * 32;
                        position = new Vector2(positionX, positionY + smallHeartOffset); // position = new Vector2(__instance.xPositionOnScreen + 320 - 4 + (index - 10) * 32, sprites[i].bounds.Y + 64);
                    }

                    if (index >= apHearts)
                    {
                        if (_hintedFriendshipLocations.Any(x => x.Contains($"{name} {index + 1} ")))
                        {
                            texture = _apLogoBlue;
                        }
                        else if (index < maxHeartForCurrentRelation)
                        {
                            texture = _apLogoWhite;
                            color = new Color(191, 191, 191);
                        }
                        else
                        {
                            texture = _apLogoBlack;
                        }
                    }

                    b.Draw(texture, position, sourceRectangle, color, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.88f);
                }
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(DrawNPCSlot_DrawEarnedHearts_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static int GetBoundedToCurrentRelationState(double friendshipPoints, string npcName)
        {
            return GetBoundedToCurrentRelationState((int)friendshipPoints, npcName);
        }

        private static int GetBoundedToCurrentRelationState(int friendshipPoints, string npcName)
        {
            var npc = Game1.getCharacterFromName(npcName);
            return Math.Max(0, Math.Min(friendshipPoints, (Utility.GetMaximumHeartsForCharacter(npc) + 1) * POINTS_PER_HEART - 1));
        }

        private static int GetMaximumHeartsWithRelationState(string npcName)
        {
            var npc = Game1.getCharacterFromName(npcName);
            return Utility.GetMaximumHeartsForCharacter(npc);
        }

        public static bool DayUpdate_ArchipelagoPoints_Prefix(Pet __instance, int dayOfMonth)
        {
            try
            {
                if (_archipelago.SlotData.Friendsanity is Friendsanity.None or Friendsanity.Bachelors)
                {
                    return true; // run original logic;
                }
                if (__instance.currentLocation is FarmHouse)
                {
                    __instance.setAtFarmPosition();
                }

                var wasPet = __instance.grantedFriendshipForPet.Value;
                var farm = (__instance.currentLocation as Farm);
                var wasWatered = farm?.petBowlWatered?.Value ?? false;
                var pointIncrease = (wasPet ? 12 : 0) + (wasWatered ? 6 : 0);
                var multipliedPointIncrease = GetMultipliedFriendship(pointIncrease);

                var petName = Game1.player.getPetName();
                var newApPoints = GetFriendshipPoints(petName) + multipliedPointIncrease;
                SetFriendshipPoints(petName, Math.Min(1000, newApPoints));
                for (var i = 1; i < newApPoints / POINTS_PER_PET_HEART; i++)
                {
                    _locationChecker.AddCheckedLocation(string.Format(FRIENDSANITY_PATTERN, PET_NAME, i));
                }
                farm?.petBowlWatered?.Set(false);

                var archipelagoHearts = _archipelago.GetReceivedItemCount(string.Format(HEARTS_PATTERN, PET_NAME));
                __instance.friendshipTowardFarmer.Set(Math.Min(1000, archipelagoHearts * POINTS_PER_PET_HEART));
                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(DayUpdate_ArchipelagoPoints_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool ChangeFriendship_ArchipelagoPoints_Prefix(Farmer __instance, int amount, NPC n)
        {
            try
            {
                var isValidTarget = n != null && (n is Child || n.isVillager());
                var canCommunicateWithNpc = !n.Name.Equals("Dwarf") || __instance.canUnderstandDwarves;
                if (!isValidTarget || (amount > 0 && !canCommunicateWithNpc))
                {
                    return false; // don't run original logic
                }

                if (__instance.friendshipData.ContainsKey(n.Name))
                {
                    if (n.isDivorcedFrom(__instance) && amount > 0)
                    {
                        return false; // don't run original logic
                    }

                    var pointDifference = amount;
                    var multipliedPointDifference = GetMultipliedFriendship(pointDifference);
                    var apPoints = GetFriendshipPoints(n.Name);
                    var newApPoints = apPoints + multipliedPointDifference;
                    newApPoints = GetBoundedToCurrentRelationState(newApPoints, n.Name);
                    SetFriendshipPoints(n.Name, newApPoints);
                    var earnedHearts = (int)newApPoints / POINTS_PER_HEART;
                    for (var i = 1; i <= earnedHearts; i++)
                    {
                        _locationChecker.AddCheckedLocation(string.Format(FRIENDSANITY_PATTERN, n.Name, i));
                    }

                    if (n.datable.Value && __instance.friendshipData[n.Name].Points >= 2000 && !__instance.hasOrWillReceiveMail("Bouquet"))
                    {
                        Game1.addMailForTomorrow("Bouquet");
                    }

                    if (n.datable.Value && __instance.friendshipData[n.Name].Points >= 2500 && !__instance.hasOrWillReceiveMail("SeaAmulet"))
                    {
                        Game1.addMailForTomorrow("SeaAmulet");
                    }
                }
                else
                {
                    Game1.debugOutput = "Tried to change friendship for a friend that wasn't there.";
                }

                Game1.stats.checkForFriendshipAchievements();

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ChangeFriendship_ArchipelagoPoints_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static string GetNpcName(Friendship __instance)
        {
            var farmer = Game1.player;
            foreach (var name in farmer.friendshipData.Keys)
            {
                if (ReferenceEquals(farmer.friendshipData[name], __instance))
                {
                    return name;
                }
            }

            return null;
        }

        private static int ShuffledUpTo(string name)
        {
            if (_archipelago.SlotData.ExcludeGingerIsland && name == "Leo")
            {
                return 0;
            }

            var isBachelor = _bachelors.Contains(name);
            switch (_archipelago.SlotData.Friendsanity)
            {
                case Friendsanity.None:
                    return 0;
                case Friendsanity.Bachelors:
                    return isBachelor ? 8 : 0;
                case Friendsanity.StartingNpcs:
                    if (name == PET_NAME)
                    {
                        return 5;
                    }

                    return _notImmediatelyAccessible.Contains(name) ? 0 : (isBachelor ? 8 : 10);
                case Friendsanity.All:
                    if (name == PET_NAME)
                    {
                        return 5;
                    }

                    return isBachelor ? 8 : 10;
                case Friendsanity.AllWithMarriage:
                    if (name == PET_NAME)
                    {
                        return 5;
                    }

                    return isBachelor ? 14 : 10;
            }

            return 0;
        }

        private static double GetMultipliedFriendship(int amount)
        {
            return amount * _archipelago.SlotData.FriendshipMultiplier;
        }

        private static int GetHearts(double friendshipPoints)
        {
            return (int)Math.Floor(friendshipPoints) / 250;
        }

        private static double GetFriendshipPoints(string npc)
        {
            if (!_friendshipPoints.ContainsKey(npc))
            {
                _friendshipPoints.Add(npc, 0);
            }

            return _friendshipPoints[npc];
        }

        private static void SetFriendshipPoints(string npc, double points)
        {
            if (!_friendshipPoints.ContainsKey(npc))
            {
                _friendshipPoints.Add(npc, 0);
            }

            if (points < 0)
            {
                points = 0;
            }

            _friendshipPoints[npc] = points;
        }
    }
}
