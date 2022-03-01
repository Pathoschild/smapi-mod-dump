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
using StardewValley;
using StardewValley.Monsters;
using System;
using xTile;
using StardewObject = StardewValley.Object;

namespace MushroomRancher
{
    public enum HutchType
    {
        normal = 0,
        slimy = 1,
        mine = 2,
        volcano = 3
    }

    public class MushroomRancher : Mod, IAssetEditor
    {
        internal const int magmaCapId = 851;
        internal const int redMushroomId = 420;
        internal const int purpleMushroomId = 422;

        private HutchType currentHutchType = HutchType.normal;
        private readonly Map[] maps = new Map[4];
        private readonly Texture2D[] waterTextures = new Texture2D[4];

        private MushroomRancherConfig config;

        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<MushroomRancherConfig>();

            MushroomRancherConfig.VerifyConfigValues(config, this);

            Helper.Events.GameLoop.GameLaunched += delegate { MushroomRancherConfig.SetUpModConfigMenu(config, this); };
            Helper.Events.GameLoop.SaveLoaded += RespawnLivingMushrooms;
            Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            Helper.Events.Player.Warped += Player_Warped;

            Patcher.PatchAll(this);

            foreach (var item in Enum.GetValues(typeof(HutchType)))
            {
                maps[(int)item] = Helper.Content.Load<Map>($"assets/{item}_hutch.tmx");
                waterTextures[(int)item] = Helper.Content.Load<Texture2D>($"assets/water_spots_{item}.png");
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
            monster.DamageToFarmer = 0;
            monster.Speed /= 2;
            monster.farmerPassesThrough = true;
            monster.coinsToDrop.Value = 0;
            monster.objectsToDrop.Clear();
            monster.objectsToDrop.Add(MushroomRancher.redMushroomId);

            string dangerous_texture_name = monster.Sprite.textureName.Value + "_dangerous";
            if (Game1.content.Load<Texture2D>(dangerous_texture_name) != null)
            {
                monster.Sprite.LoadTexture(dangerous_texture_name);
            }

            return monster;
        }

        public static RockCrab CreateFriendlyMagmaCap(Vector2 vector, MushroomRancher mod)
        {
            var monster = new RockCrab(vector, "False Magma Cap");
            monster.DamageToFarmer = 0;
            monster.Speed /= 2;
            monster.farmerPassesThrough = true;
            monster.coinsToDrop.Value = 0;
            monster.objectsToDrop.Clear();
            monster.objectsToDrop.Add(MushroomRancher.magmaCapId);
            monster.moveTowardPlayerThreshold.Value = 2;

            return monster;
        }

        private void RespawnLivingMushrooms(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            foreach (var item in Game1.getFarm().buildings)
            {
                if (item?.indoors?.Value is SlimeHutch hutch)
                {
                    if (hutch.Objects.TryGetValue(new Vector2(1, 4), out StardewObject incubator))
                    {
                        if (incubator != null && incubator.Name.Equals("Slime Incubator"))
                        {
                            incubator.Fragility = config.RemovableSlimeHutchIncubator ? StardewObject.fragility_Removable : StardewObject.fragility_Indestructable;
                        }
                    }

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
                            hutch.characters[i] = CreateFriendlyMagmaCap(pos, this);
                        }
                    }
                }
            }
        }

        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            if (!config.RandomizeMonsterPositionInHutch)
            {
                return;
            }

            foreach (var item in Game1.getFarm().buildings)
            {
                if (item?.indoors?.Value is SlimeHutch hutch)
                {
                    foreach (var monster in hutch.characters)
                    {
                        int tries = 50;
                        Vector2 tile = hutch.getRandomTile();
                        while ((!hutch.isTileLocationTotallyClearAndPlaceable(tile) || tile.Y >= 12f) && tries > 0)
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
            }
        }

        private void Player_Warped(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            if (e?.NewLocation is SlimeHutch hutch)
            {
                if (config.HutchInterior == 1)
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
                        else if (monster is RockCrab crab && monster.Name.Equals("False Magma Cap"))
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

                Helper.Content.InvalidateCache("Maps/SlimeHutch");
                Helper.Content.InvalidateCache("Maps/townInterior");
            }
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return Context.IsWorldReady && (asset.AssetNameEquals("Maps/SlimeHutch") || asset.AssetNameEquals("Maps/townInterior")) && config.HutchInterior > 0 && config.HutchInterior < MushroomRancherConfig.InteriorChoices.Length;
        }

        public void Edit<T>(IAssetData asset)
        {
            var option = config.HutchInterior == 1 ? (int)currentHutchType : config.HutchInterior - 1;

            if (asset.AssetNameEquals("Maps/SlimeHutch"))
            {
                var editor = asset.AsMap();

                var newVal = maps[option];

                if (newVal != null)
                {
                    editor.ReplaceWith(newVal);
                }
            }

            if (asset.AssetNameEquals("Maps/townInterior"))
            {
                var editor = asset.AsImage();

                var newVal = waterTextures[option];

                if (newVal != null)
                {
                    editor.PatchImage(newVal, targetArea: new Rectangle(352, 1056, 32, 16));
                }
            }
        }
    }
}