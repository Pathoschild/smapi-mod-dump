/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData.Locations;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.ItemTypeDefinitions;
using Fishnets.Data;
using StardewModdingAPI;
using StardewValley.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Fishnets
{
    public static class Statics
    {
        private const int SpriteFrames = 8;

        internal static List<string> ExcludedFish = [];

        private static readonly int[] Qualities = [Object.lowQuality, Object.medQuality, Object.highQuality, Object.bestQuality];

        private static readonly Texture2D FishNetTexture = ModEntry.IHelper.ModContent.Load<Texture2D>("assets/FishNet.png");

        private static readonly Rectangle FishNetSourceRect = new(0, 0, 16, 16);

        private static readonly Dictionary<Vector2, Vector2> OffsetMap = [];

        internal static readonly List<string> WeedsIds = ["(O)152", "(O)153", "(O)157"];

        internal static readonly List<string> JellyIds = ["(O)RiverJelly", "(O)SeaJelly", "(O)CaveJelly"];

        internal static bool TryGetRandomFishForLocation(string? bait, Farmer who, string locationName, [NotNullWhen(true)] out Object? fish)
        {
            fish = null;

            string itemId = "";
            bool flag2 = bait == "908";
            string locationKey = locationName;
            GameLocation location = Game1.getLocationFromName(locationKey);

            if (location.GetData() is not LocationData data)
                return false;
            List<string> keys = [.. data.Fish.SelectMany(x => x.Id.Split('|'))]; //Unpack fish item ids into list
            Utility.Shuffle(Game1.random, keys); //Shuffle ids
            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                if (!CanCatchThisFish(key, location, who, flag2)) //Validate the fish can be caught and it's included in Data\\Fish
                    continue;

                double chance = TryGetCatchChance(key); //Generate random chance based on internal fish catch chance and farmer fishing level
                chance += (who.FishingLevel / 50);
                chance = Math.Min(chance, 0.899999976158142);
                if (Game1.random.NextDouble() <= chance)
                {
                    itemId = key;
                    break;
                }
            }

            int stackSize = GetStackSizeFromBait(bait); //bait == "774" && Game1.random.NextDouble() <= .15 ? 2 : 1; //Try set stacksize for Wild bait \\Replaced by getStackSizeFromBait
            if (string.IsNullOrWhiteSpace(itemId))
            {
                itemId = $"(O){Game1.random.Next(167, 173)}";
                stackSize = 1;
            }
            if (Game1.random.NextDouble() <= .15 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
                itemId = "(O)890";
            fish = ItemRegistry.Create<Object>(itemId, stackSize);
            return true;
        }

        internal static bool CanCatchThisFish(string itemId, GameLocation location, Farmer? who = null, bool isMagicBait = false)
        {
            Dictionary<string, string> fishData = ModEntry.IHelper.GameContent.Load<Dictionary<string, string>>("Data\\Fish");

            if (location.GetData() is not LocationData data || (!fishData.TryGetValue(UnQualifyItemId(itemId), out string? fishStr) && !JellyIds.Contains(itemId) && !WeedsIds.Contains(itemId)))
                return false;
            var fishKeys = data.Fish.SelectMany(x => x.Id.Split('|'));

            if (JellyIds.Contains(itemId) && fishKeys.Any(x => x == itemId))
                return true;
            if (WeedsIds.Contains(itemId) && fishKeys.Any(x => x == itemId))
                return true;

            if (string.IsNullOrWhiteSpace(fishStr))
                return false;

            foreach (var fish in data.Fish)
            {
                if (fish.Id.Contains(itemId))
                {
                    if (fishStr.Contains("trap"))
                        return false;
                    string[] fishStrSplit = fishStr.Split('/');
                    bool flag1 = fish.Season is null || fish.Season == Game1.season; //Do seasons match?
                    bool flag2 = ItemRegistry.Create(itemId).TypeDefinitionId == "(O)"; //Is Object type?
                    bool flag3 = string.IsNullOrWhiteSpace(fish.Condition) || GameStateQuery.CheckConditions(fish.Condition, location, who, ignoreQueryKeys: isMagicBait ? GameStateQuery.MagicBaitIgnoreQueryKeys : null); //Do GSQ conditions exist and if they do, do they match current conditions?
                    bool flag4 = !ExcludedFish.Contains(fishStrSplit[0]); //Is the fish not excluded from possible catches?
                    bool flag5 = true; //Do weather conditions match?
                    bool flag6 = (fish.RequireMagicBait && isMagicBait) || !fish.RequireMagicBait; //Does the fish not require magic bait, or does it and is magic bait used
                    bool flag7 = (who ?? Game1.player).FishingLevel >= Convert.ToInt32(fishStrSplit[12]); //Is the fishing level higher than or equal to the minimum catch level?
                    if (fishStrSplit[7] != "both")
                    {
                        if (fishStrSplit[7] == "rainy" && !Game1.IsRainingHere(location))
                            flag5 = false;
                        else if (fishStrSplit[7] == "sunny" && Game1.IsRainingHere(location))
                            flag5 = false;
                    }

                    return flag2 && flag3 && flag4 && flag6 && flag7 && (isMagicBait || (flag1 && flag5));
                }
            }
            return false;
        }

        internal static void Draw(Object fishNet, SpriteBatch b, int x, int y, float alpha)
        {
            int tileIndex = 0;
            Vector2 directionOffset = Vector2.Zero;
            Vector2 tile = fishNet.TileLocation;
            if (fishNet.Location is not null)
            {
                if (!OffsetMap.ContainsKey(tile))
                    OnPlace(fishNet.Location, fishNet.TileLocation); //Re-Place to avoid key errors if needed
                directionOffset = OffsetMap[tile];
                if (TryGetTileIndexData(fishNet, out var tileIndexData))
                    tileIndex = tileIndexData.tileIndex == 7 ? 5 : tileIndexData.tileIndex; //Forced intermediate frame, might remove, needs testing \\Looks alright, I'm keeping it until I get complaints
            }
            if (fishNet.heldObject.Value is not null)
                tileIndex = 4;
            float yBob = (float)Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 500.0f + (x * 64f)) * 8.0f + 8.0f;
            if (fishNet.Location is null)
                yBob = 0;

            if (ModEntry.HasAlternativeTextures)
            {
                Rectangle sourceRect = Rectangle.Empty;
                Texture2D? texture = ModEntry.IAlternativeTexturesApi?.GetTextureForObject(fishNet, out sourceRect);
                if (texture is not null && sourceRect != Rectangle.Empty)
                    b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, directionOffset + new Vector2(x * 64f, y * 64f + yBob)), sourceRect, Color.White * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, ((y * 64f) + directionOffset.Y + (x % 4)) / 10000.0f);
                else
                    b.Draw(FishNetTexture, Game1.GlobalToLocal(Game1.viewport, directionOffset + new Vector2(x * 64f, y * 64f + yBob)), GetSourceRectAtTileIndex(tileIndex), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((y * 64f) + directionOffset.Y + (x % 4)) / 10000.0f);
            }
            else
                b.Draw(FishNetTexture, Game1.GlobalToLocal(Game1.viewport, directionOffset + new Vector2(x * 64f, y * 64f + yBob)), GetSourceRectAtTileIndex(tileIndex), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((y * 64f) + directionOffset.Y + (x % 4)) / 10000.0f);
            if (fishNet.Location?.waterTiles != null && x < fishNet.Location.waterTiles.waterTiles.GetLength(0) && y < fishNet.Location.waterTiles.waterTiles.GetLength(1) && fishNet.Location.waterTiles.waterTiles[x, y].isWater && tileIndex <= 4)
            {
                if (fishNet.Location.waterTiles.waterTiles[x, y].isVisible)
                    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, directionOffset + new Vector2((x * 64 + 4), y * 64 + 48)), new(fishNet.Location.waterAnimationIndex * 64, 2112 + ((x + y) % 2 == 0 ? (fishNet.Location.waterTileFlip ? 128 : 0) : (fishNet.Location.waterTileFlip ? 0 : 128)), 56, 16 + (int)yBob), fishNet.Location.waterColor.Value, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, ((y * 64) + directionOffset.Y + (x % 4)) / 9999);
                else
                {
                    Color color = Utility.MultiplyColor(new Color(135, 135, 135, 215), fishNet.Location.waterColor.Value);
                    b.Draw(Game1.staminaRect, Game1.GlobalToLocal(Game1.viewport, directionOffset + new Vector2(x * 64 + 4, y * 64 + 48)), new(), color, 0.0f, Vector2.Zero, new Vector2(56f, 16 + yBob), SpriteEffects.None, ((y * 64) + directionOffset.Y + x % 4) / 9999);
                }
            }
            if (!fishNet.readyForHarvest.Value || fishNet.heldObject.Value is null)
                return;
            float num = 4.0f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, directionOffset + new Vector2(x * 64f - 8, y * 64f - 112 + num)), new Rectangle(141, 465, 20, 24), Color.White * .75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float)((y + 1) * 64f / 10000.0f + 9.99999997475243E-07 + (fishNet.TileLocation.X) / 10000.0f));
            DrawHeldObject(b, fishNet, x, y, num, directionOffset);

            if (fishNet.heldObject.Value.Stack > 1)
                NumberSprite.draw(fishNet.heldObject.Value.Stack, b, Game1.GlobalToLocal(Game1.viewport, directionOffset + new Vector2(x * 64f + 32 + 20, y * 64f - 72 + num + 20)), Color.White, .5f, (float)((y + 1) * 64f / 10000.0f + 9.99999974737875E-06 + fishNet.TileLocation.X / 10000.0f) + 0.001f, 1f, 0);
            if (fishNet.heldObject.Value.Quality > 0)
            {
                float num2 = fishNet.heldObject.Value.Quality < 4 ? 0.0f : (float)((Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.0500000007450581);
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, directionOffset + new Vector2(x * 64f + 32 - 20, y * 64f - 72 + num + 20)), GetSourceRectForQuality(fishNet.heldObject.Value.Quality), Color.White, 0.0f, new(4f), (float)(2.0 * 1.0 * (1.0 + num2)), SpriteEffects.None, (float)((y + 1) * 64f / 10000.0f + 9.99999974737875E-06 + fishNet.TileLocation.X / 10000.0f) + 0.001f);
            }
        }

        internal static void DoDayUpdate(Object fishNet)
        {
            try
            {
                GameLocation location = fishNet.Location;
                Vector2 tile = fishNet.TileLocation;
                Farmer owner = Game1.getFarmerMaybeOffline(fishNet.owner.Value);
                if (!OffsetMap.ContainsKey(tile))
                    OnPlace(fishNet.Location, fishNet.TileLocation); //Backup fail-safe, don't understand why it's needed, but I've seen enough red for today
                Object? bait = null;
                if (TryParseModData(fishNet, out var baitData))
                    bait = ItemRegistry.Create<Object>(baitData.Key, quality: baitData.Value, allowNull: true);

                if (fishNet.owner.Value <= 0)
                    fishNet.owner.Value = Game1.player.UniqueMultiplayerID;
                bool flag1 = owner != null && owner.professions.Contains(11);
                bool flag2 = owner != null && owner.professions.Contains(10);
                if (owner is null && Game1.player.professions.Contains(11))
                    flag2 = true;
                owner ??= Game1.player;
                if ((bait is null && !flag1) || fishNet.heldObject.Value is not null)
                    return;
                fishNet.readyForHarvest.Value = true;
                Random r = new();
                Dictionary<string, string> fishData = ModEntry.IHelper.GameContent.Load<Dictionary<string, string>>("Data\\Fish");
                List<string> ids = [];
                if (!location.TryGetFishAreaForTile(tile, out _, out var data))
                    data = null;
                double chance = (flag2 || ModEntry.IConfig.LessTrash) ? 0 : (data?.CrabPotJunkChance ?? .2);
                if (!r.NextBool(chance))
                {
                    foreach (var item in fishData)
                    {
                        if (item.Value.Contains("trap"))
                            continue;
                        if (CanCatchThisFish(item.Key, location, owner, isMagicBait: bait?.ItemId == "908"))
                            ids.Add(item.Key);
                        if (r.NextDouble() > .15 || !TryGetRandomFishForLocation(bait?.ItemId, owner, location.Name, out var o) || (o.Category == Object.junkCategory && ModEntry.IConfig.LessTrash))
                            continue;
                        AddBaitEffects(o, bait!, flag1, r);
                        fishNet.heldObject.Value = o;
                        break;
                    }
                }
                if (fishNet.heldObject.Value is not null && !ShouldReRoll(fishNet.heldObject.Value))
                    return;
                if ((flag2 || ShouldReRoll(fishNet.heldObject.Value)) && ids.Count > 0)
                {
                    Object o = new(ids[r.Next(ids.Count)], 1);
                    AddBaitEffects(o, bait!, flag1, r);
                    o.Stack = GetStackSizeFromBait(bait?.ItemId);
                    fishNet.heldObject.Value = o;
                }
                else
                    fishNet.heldObject.Value = new(r.Next(168, 173).ToString(), 1);
            }
            catch (Exception ex)
            {
                ModEntry.IMonitor.Log($"Failed trying to perform Object.DayUpdate patch", LogLevel.Error);
                ModEntry.IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
            }
        }

        internal static Rectangle GetSourceRectAtTileIndex(int tileIndex) => new(FishNetSourceRect.X + (FishNetSourceRect.Width * (tileIndex + 1)), FishNetSourceRect.Y + (FishNetSourceRect.Height * ModEntry.IConfig.TextureVariant), FishNetSourceRect.Width, FishNetSourceRect.Height);

        internal static int GetSpriteIndexFromConfig() => 0 + (SpriteFrames * ModEntry.IConfig.TextureVariant);

        [Obsolete($"Will be removed in next non-bugfix release, alongside {nameof(FishNetSerializable)}")]
        internal static Object? GetObjectFromSerializable(FishNetSerializable serializable)
        {
            Object? o = null;
            string id = serializable.ObjectId;
            if (serializable.IsJAObject)
            {
                id = ModEntry.IJsonAssetsApi.GetObjectId(serializable.ObjectName);
                if (string.IsNullOrWhiteSpace(id))
                    id = serializable.ObjectId;
            }
            else if (serializable.IsDGAObject)
            {
                ModEntry.IMonitor.Log($"DGA object found! As of Stardew 1.6 DGA is seen as deprecated, as of Fishnets 2.0.0+ all DGA support has been dropped", LogLevel.Warn);
                return null;
            }
            o = (Object?)ItemRegistry.Create(id, serializable.ObjectStack, serializable.ObjectQuality, true);
            return o;
        }

        public static void SetTileIndexData(Object fishNet, bool complete, int tileIndex, int timer) => fishNet.modData[ModEntry.ModDataTileIndexKey] = $"{(complete ? 1 : 0)},{tileIndex},{timer}";

        public static bool TryGetTileIndexData(Object fishNet, out (bool complete, int tileIndex, int timer) data)
        {
            data = (false, 0, 60);
            if (!fishNet.modData.TryGetValue(ModEntry.ModDataTileIndexKey, out string tileIndexData))
                return false;
            string[] opts = tileIndexData.Split(',');
            data = (opts[0] == "1", int.Parse(opts[1]), int.Parse(opts[2]));
            return true;
        }

        public static void ClearTileIndexData(Object fishNet) => fishNet.modData.Remove(ModEntry.ModDataTileIndexKey);

        public static void SetModData(Object fishNet, KeyValuePair<string, int> bait) => fishNet.modData[ModEntry.ModDataKey] = new StringBuilder().Append(bait.Key).Append('|').Append(bait.Value).ToString();

        public static bool HasModData(Object fishNet) => fishNet.modData.ContainsKey(ModEntry.ModDataKey);

        public static bool TryParseModData(Object fishNet, out KeyValuePair<string, int> modData)
        {
            modData = default;
            if (!fishNet.modData.TryGetValue(ModEntry.ModDataKey, out var data))
                return false;
            string[] split = data.Split('|');
            int quality = int.Parse(split[1]);
            modData = new(split[0], quality);
            return true;
        }

        public static bool RemoveModData(Object fishNet) => fishNet.modData.Remove(ModEntry.ModDataKey);

        /// <summary>
        /// Clear the offset map when the current location changes
        /// </summary>
        internal static void ClearOffsetMap() => OffsetMap.Clear();

        internal static void OnPlace(GameLocation location, Vector2 tile)
        {
            OffsetMap[tile] = SetDirectionOffset(location, tile);
            AddOverlayTiles(location, tile, OffsetMap[tile]);
        }

        internal static void OnRemove(GameLocation location, Vector2 tile)
        {
            var offset = OffsetMap[tile];
            OffsetMap.Remove(tile);
            RemoveOverlayTiles(location, tile, offset);
        }

        internal static Vector2 SetDirectionOffset(GameLocation location, Vector2 tileLocation)
        {
            Vector2 zero = Vector2.Zero;
            if (CheckLocation(location, tileLocation.X - 1f, tileLocation.Y))
                zero += new Vector2(32f, 0f);
            if (CheckLocation(location, tileLocation.X + 1f, tileLocation.Y))
                zero += new Vector2(-32f, 0f);
            if (zero.X != 0.0f && CheckLocation(location, tileLocation.X + Math.Sign(zero.X), tileLocation.Y + 1f))
                zero += new Vector2(0.0f, -42f);
            if (CheckLocation(location, tileLocation.X, tileLocation.Y - 1f))
                zero += new Vector2(0.0f, 32f);
            if (CheckLocation(location, tileLocation.X, tileLocation.Y + 1f))
                zero += new Vector2(0.0f, -42f);
            return zero;
        }

        internal static void AddOverlayTiles(GameLocation location, Vector2 tile, Vector2 directionOffset)
        {
            if (location != Game1.currentLocation)
                return;
            foreach (Vector2 overlayTile in GetOverlayTiles(location, tile, directionOffset))
            {
                if (!Game1.crabPotOverlayTiles.ContainsKey(overlayTile))
                    Game1.crabPotOverlayTiles[overlayTile] = 0;
                Game1.crabPotOverlayTiles[overlayTile]++;
            }
        }

        internal static void RemoveOverlayTiles(GameLocation location, Vector2 tile, Vector2 directionOffset)
        {
            if (location != Game1.currentLocation)
                return;
            foreach (Vector2 overlayTile in GetOverlayTiles(location, tile, directionOffset))
            {
                if (Game1.crabPotOverlayTiles.ContainsKey(overlayTile))
                {
                    Game1.crabPotOverlayTiles[overlayTile]--;
                    if (Game1.crabPotOverlayTiles[overlayTile] <= 0)
                        Game1.crabPotOverlayTiles.Remove(overlayTile);
                }
            }
        }

        private static bool CheckLocation(GameLocation location, float x, float y) => location.doesTileHaveProperty((int)x, (int)y, "Water", "Back") == null || location.doesTileHaveProperty((int)x, (int)y, "Passable", "Buildings") != null;

        private static List<Vector2> GetOverlayTiles(GameLocation location, Vector2 tileLocation, Vector2 directionOffset)
        {
            List<Vector2> tiles = [];
            if (directionOffset.Y < 0f)
                AddOverlayTilesIfNecessary(location, (int)tileLocation.X, (int)tileLocation.Y, tiles);
            AddOverlayTilesIfNecessary(location, (int)tileLocation.X, (int)tileLocation.Y + 1, tiles);
            if (directionOffset.X < 0f)
                AddOverlayTilesIfNecessary(location, (int)tileLocation.X - 1, (int)tileLocation.Y + 1, tiles);
            if (directionOffset.X > 0f)
                AddOverlayTilesIfNecessary(location, (int)tileLocation.X + 1, (int)tileLocation.Y + 1, tiles);
            return tiles;
        }

        private static void AddOverlayTilesIfNecessary(GameLocation location, int x, int y, List<Vector2> tiles)
        {
            if (location != Game1.currentLocation || location.getTileIndexAt(x, y, "Buildings") < 0 || location.doesTileHaveProperty(x, y + 1, "Back", "Water") != null)
                return;
            tiles.Add(new(x, y));
        }

        private static Rectangle GetSourceRectForQuality(int quality)
        {
            return quality switch
            {
                Object.medQuality => new(338, 400, 8, 8),
                Object.highQuality => new(346, 400, 8, 8),
                Object.bestQuality => new(346, 392, 8, 8),
                _ => new(338, 392, 8, 8)
            };
        }

        private static void DrawHeldObject(SpriteBatch b, Object fishNet, int x, int y, float yOffset, Vector2 directionOffset)
        {
            ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(fishNet.heldObject.Value.QualifiedItemId);
            b.Draw(dataOrErrorItem.GetTexture(), Game1.GlobalToLocal(Game1.viewport, directionOffset + new Vector2(x * 64f + 32f, y * 64f - 72f + yOffset)), dataOrErrorItem.GetSourceRect(0, dataOrErrorItem.SpriteIndex), Color.White * .75f, 0f, new(8f), 4f, SpriteEffects.None, (float)((y + 1) * 64f / 10000.0f + 9.99999974737875E-06 + fishNet.TileLocation.X / 10000.0f));
        }
        private static string UnQualifyItemId(string id) => ItemRegistry.IsQualifiedItemId(id) ? id.Split(')')[1] : id;

        private static int GetStackSizeFromBait(string? baitId)
        {
            return baitId switch
            {
                "774" => Game1.random.NextDouble() <= .15 ? 2 : 1,
                "ChallengeBait" => Game1.random.NextDouble() <= .05 ? 3 : (Game1.random.NextDouble() <= .15 ? 2 : 1),
                _ => 1
            };
        }

        private static void AddBaitEffects(Object heldObject, Object bait, bool flag1, Random r)
        {
            if (bait?.ItemId == "DeluxeBait")
                heldObject.Quality = r.NextDouble() <= .15 ? Object.medQuality : Object.lowQuality;
            if (ModEntry.HasQualityBait)
                heldObject.Quality = ModEntry.IQualityBaitApi.GetQuality(heldObject.Quality, bait?.Quality ?? (flag1 ? Qualities[Game1.random.Next(4)] : Object.lowQuality));
            if (heldObject.Category == Object.junkCategory || WeedsIds.Contains(heldObject.QualifiedItemId) || JellyIds.Contains(heldObject.QualifiedItemId)) //I just got gold star glasses when that shouldn't be possible, so I'm adding this and praying
                heldObject.Quality = Object.lowQuality;
        }

        private static bool ShouldReRoll(Object? current)
        {
            if (current is null)
                return true;
            if (ModEntry.IConfig.LessTrash && current.Category == Object.junkCategory)
                return true;
            if (ModEntry.IConfig.LessWeeds && WeedsIds.Contains(current.QualifiedItemId))
                return true;
            if (ModEntry.IConfig.LessJelly && JellyIds.Contains(current.QualifiedItemId))
                return true;
            return false;
        }

        private static double TryGetCatchChance(string itemId)
        {
            if (JellyIds.Contains(itemId))
                return ModEntry.IConfig.LessJelly ? 0.05 : 0.15;
            if (WeedsIds.Contains(itemId))
                return ModEntry.IConfig.LessWeeds ? 0.05 : 0.15;

            Dictionary<string, string> fishData = DataLoader.Fish(Game1.content);

            if (!fishData.TryGetValue(UnQualifyItemId(itemId), out string fishStr))
                return 0.0;
            string[] fishStrSplit = fishStr.Split('/');

            return Convert.ToDouble(fishStrSplit[10]);
        }
    }
}
