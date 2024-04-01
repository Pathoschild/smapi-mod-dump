/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Monsters;
using System;
using xTile;

namespace MushroomRancher
{
    public enum HutchType
    {
        normal = 0,
        slimy = 1,
        mine = 2,
        volcano = 3
    }

    public class MushroomRancher : Mod
    {
        internal const string magmaCapId = "(O)851";
        internal const string redMushroomId = "(O)420";
        internal const string purpleMushroomId = "(O)422";

        private HutchType currentHutchType = HutchType.normal;
        private readonly Map[] maps = new Map[4];
        private readonly Texture2D[] waterTextures = new Texture2D[4];

        internal static MushroomRancher Mod { get; private set; }
        internal static MushroomRancherConfig Config { get; private set; }
        internal string MushroomIncubatorAssetPath { get; private set; }

        public override void Entry(IModHelper helper)
        {
            Mod = this;
            Config = Helper.ReadConfig<MushroomRancherConfig>();

            MushroomRancherConfig.VerifyConfigValues(Config, this);

            Helper.Events.GameLoop.GameLaunched += delegate { MushroomRancherConfig.SetUpModConfigMenu(Config, this); };
            Helper.Events.GameLoop.SaveLoaded += RespawnLivingMushrooms;
            Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            Helper.Events.Player.Warped += Player_Warped;
            Helper.Events.Content.AssetRequested += OnAssetRequested;

            Patcher.PatchAll(this);

            MushroomIncubatorAssetPath = Helper.ModContent.GetInternalAssetName("assets/mushroomIncubator.png").BaseName;

            foreach (var item in Enum.GetValues(typeof(HutchType)))
            {
                maps[(int)item] = (int)item == 0 ? null : Helper.ModContent.Load<Map>($"assets/{item}_hutch.tmx");
                waterTextures[(int)item] = Helper.ModContent.Load<Texture2D>($"assets/water_spots_{item}.png");
            }
        }

        /// <summary>
        /// Small helper method to log to the console because I keep forgetting the signature
        /// </summary>
        /// <param name="o">the object I want to log as a string</param>
        public void DebugLog(object o)
        {
            Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
        }

        /// <summary>
        /// Small helper method to log an error to the console because I keep forgetting the signature
        /// </summary>
        /// <param name="o">the object I want to log as a string</param>
        /// <param name="e">an optional error message to log additionally</param>
        public void ErrorLog(object o, Exception e = null)
        {
            string baseMessage = o == null ? "null" : o.ToString();

            string errorMessage = e == null ? string.Empty : $"\n{e.Message}\n{e.StackTrace}";

            Monitor.Log(baseMessage + errorMessage, LogLevel.Error);
        }

        public static DustSpirit CreateFriendlyLivingMushroom(Vector2 vector)
        {
            var monster = new DustSpirit(vector, false);
            monster.Speed /= 2;
            monster.DamageToFarmer = 0;
            monster.farmerPassesThrough = true;
            monster.objectsToDrop.Clear();
            monster.objectsToDrop.Add(MushroomRancher.redMushroomId);

            string dangerous_texture_name = monster.Sprite.textureName.Value + "_dangerous";
            if (Game1.content.Load<Texture2D>(dangerous_texture_name) != null)
            {
                monster.Sprite.LoadTexture(dangerous_texture_name);
            }

            return monster;
        }

        public static RockCrab CreateFriendlyMagmaCap(Vector2 vector)
        {
            var monster = new RockCrab(vector, "False Magma Cap");
            monster.Speed /= 2;
            monster.DamageToFarmer = 0;
            monster.farmerPassesThrough = true;
            monster.objectsToDrop.Clear();
            monster.objectsToDrop.Add(MushroomRancher.magmaCapId);
            monster.moveTowardPlayerThreshold.Value = 2;

            return monster;
        }

        private void RespawnLivingMushrooms(object sender, SaveLoadedEventArgs e)
        {
            Utility.ForEachBuilding(delegate (Building building)
            {
                if (building?.indoors?.Value is SlimeHutch hutch)
                {
                    for (int i = 0; i < hutch.characters.Count; i++)
                    {
                        if (hutch.characters[i] is DustSpirit)
                        {
                            var pos = hutch.characters[i].Position;
                            hutch.characters[i] = CreateFriendlyLivingMushroom(pos);
                        }
                        else if (hutch.characters[i] is RockCrab && hutch.characters[i].Name.Equals("False Magma Cap"))
                        {
                            var pos = hutch.characters[i].Position;
                            hutch.characters[i] = CreateFriendlyMagmaCap(pos);
                        }
                    }
                }
                return true;
            });
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Config.RandomizeMonsterPositionInHutch)
            {
                return;
            }

            Utility.ForEachBuilding(delegate (Building building)
            {
                if (building?.indoors?.Value is SlimeHutch hutch)
                {
                    foreach (var monster in hutch.characters)
                    {
                        if (Config.RandomizeMonsterPositionOnlyAffectsLivingMushrooms && !(monster is DustSpirit or RockCrab))
                        {
                            continue;
                        }

                        int tries = 50;
                        Vector2 tile = hutch.getRandomTile();
                        while ((!hutch.CanItemBePlacedHere(tile, false, CollisionMask.All, ~CollisionMask.Objects, false, false) || tile.Y >= 12f) && tries > 0)
                        {
                            tile = hutch.getRandomTile();
                            tries--;
                        }

                        tile *= 64;

                        if (tries > 0)
                        {
                            monster.Position = tile;
                        }
                    }
                }
                return true;
            });
        }

        private void Player_Warped(object sender, WarpedEventArgs e)
        {
            if (e?.NewLocation is SlimeHutch hutch)
            {
                if (Config.HutchInterior == 1)
                {
                    int slimes = 0;
                    int mushrooms = 0;
                    int magmaCaps = 0;

                    foreach (var monster in hutch.characters)
                    {
                        if (monster is GreenSlime)
                        {
                            slimes++;
                        }
                        else if (monster is DustSpirit)
                        {
                            mushrooms++;
                        }
                        else if (monster is RockCrab && monster.Name.Equals("False Magma Cap"))
                        {
                            magmaCaps++;
                        }
                    }

                    if (slimes >= mushrooms && slimes >= magmaCaps)
                    {
                        currentHutchType = HutchType.slimy;
                    }
                    else if (magmaCaps > slimes && magmaCaps >= mushrooms)
                    {
                        currentHutchType = HutchType.volcano;
                    }
                    else if (mushrooms > slimes && mushrooms > magmaCaps)
                    {
                        currentHutchType = HutchType.mine;
                    }

                    if (mushrooms + slimes + magmaCaps == 0)
                    {
                        currentHutchType = HutchType.normal;
                    }
                }

                Helper.GameContent.InvalidateCacheAndLocalized("Maps/SlimeHutch");
                Helper.GameContent.InvalidateCacheAndLocalized("Maps/townInterior");
            }
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (!(Config.HutchInterior > 0 && Config.HutchInterior < MushroomRancherConfig.InteriorChoices.Length))
            {
                return;
            }

            var option = Config.HutchInterior == 1 ? (int)currentHutchType : Config.HutchInterior - 1;

            if (e.NameWithoutLocale.IsEquivalentTo("Maps/SlimeHutch"))
            {
                e.Edit((asset) =>
                {
                    var editor = asset.AsMap();

                    var newVal = maps[option];

                    if (newVal != null)
                    {
                        editor.ReplaceWith(newVal);
                    }
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Maps/townInterior"))
            {
                e.Edit((asset) =>
                {
                    var editor = asset.AsImage();

                    var newVal = waterTextures[option];

                    if (newVal != null)
                    {
                        editor.PatchImage(newVal, targetArea: new Rectangle(352, 1056, 32, 16));
                    }
                });
            }

            MushroomIncubator.AddIncubatorAssetChanges(e);
        }
    }

    /// <summary>
    /// Extension methods for IGameContentHelper.
    /// </summary>
    public static class GameContentHelperExtensions
    {
        /// <summary>
        /// Invalidates both an asset and the locale-specific version of an asset.
        /// </summary>
        /// <param name="helper">The game content helper.</param>
        /// <param name="assetName">The (string) asset to invalidate.</param>
        /// <returns>if something was invalidated.</returns>
        public static bool InvalidateCacheAndLocalized(this IGameContentHelper helper, string assetName)
            => helper.InvalidateCache(assetName)
                | (helper.CurrentLocaleConstant != LocalizedContentManager.LanguageCode.en && helper.InvalidateCache(assetName + "." + helper.CurrentLocale));
    }
}