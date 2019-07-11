//Copyright (c) 2019 Jahangmar

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public License
//along with this program. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

using System;
using StardewValley.BellsAndWhistles;
using System.Linq;

//TODO for multiplayer: only do this for the host

namespace CompostPestsCultivation
{
    public static class Pests {
        private static Config config;
        private static readonly List<Pest> pests = new List<Pest>();
        private static readonly Random rand = new Random();

        private static Texture2D pestTexture;
        private static Rectangle pestRectangle = new Rectangle(0, 96, 16, 16);

        public static void Init(Config conf)
        {
            config = conf;
            pestTexture = Game1.content.Load<Texture2D>("TileSheets\\debris");
        }

        public static void Load()
        {
            pests.Clear();

            List<Vector2> loadedInfestedCrops = ModEntry.GetHelper().Data.ReadSaveData<SaveData>(SaveData._InfestedCrops)?.InfestedCrops;
            if (loadedInfestedCrops != null)
                pests.AddRange(loadedInfestedCrops.Select((vec) => TryInfestCrop(vec, Game1.getFarm().terrainFeatures[vec] is HoeDirt hd ? hd : null)).Where((arg) => arg != null));
            ModEntry.GetMonitor().Log("Pests.Load() executed", LogLevel.Trace);
            ModEntry.GetMonitor().Log($"loaded {pests.Count} pests", LogLevel.Trace);
        }

        public static void Save()
        {
            SaveData dat = new SaveData()
            {
                InfestedCrops = pests.Select((pest) => pest.GetPos()).ToList()
            };

            ModEntry.GetHelper().Data.WriteSaveData<SaveData>(SaveData._InfestedCrops, dat);
            ModEntry.GetMonitor().Log("Pests.Save() executed", LogLevel.Trace);
        }

        public static void OnNewDay()
        {
            ModEntry.GetMonitor().Log($"There are currently {pests.Count} infested crops", LogLevel.Trace);

            //pests eat crops
            //ModEntry.GetMonitor().Log("nom", LogLevel.Trace);
            List<Pest> copy = new List<Pest>(pests);
            foreach (Pest pest in copy) {
                //ModEntry.GetMonitor().Log("nom on pest", LogLevel.Trace);
                if (pest.NomNom())
                    pests.Remove(pest);
            }

            //infest adjacent crops
            //ModEntry.GetMonitor().Log("adjacent", LogLevel.Trace);
            copy = new List<Pest>(pests);
            foreach (Pest pest in copy)
            {
                pests.AddRange(pest.InfestAdjacent(config.adjacent_infestation_chance));
            }

            //infest new crops
            //ModEntry.GetMonitor().Log("infest new", LogLevel.Trace);
            foreach (KeyValuePair<Vector2, TerrainFeature> kv in Game1.getFarm().terrainFeatures.Pairs)
            {
                if (kv.Key is Vector2 vec && kv.Value is HoeDirt hd)
                {
                    //ModEntry.GetMonitor().Log("found hoedirt", LogLevel.Trace);
                    if (CheckChance(config.pest_infestation_chance))
                    {
                        Pest pest = TryInfestCrop(vec, hd);
                        if (pest != null)
                        {
                            ModEntry.GetMonitor().Log("Infested crop at " + vec, LogLevel.Trace);
                            pests.Add(pest);
                        }
                    }
                }
            }

            ModEntry.GetMonitor().Log($"Now there are {pests.Count} infested crops", LogLevel.Trace);
        }

        public static void DrawPests(SpriteBatch spriteBatch)
        {
            if (Game1.currentLocation is Farm)
            foreach (Pest pest in pests)
            {
                pest.Draw(spriteBatch);
            }
        }

        public static Pest TryInfestCrop(Vector2 pos, HoeDirt hd)
        {
            if (hd != null && hd.crop == null || hd.crop.dead.Value || pests.Exists((Pest p) => p.HasPosition(pos)))
                return null;

            List<CropTrait> traits = Cultivation.GetTraits(hd.crop);
            switch (Cultivation.GetPestRes(pos, traits))
            {
                case 0:
                    break;
                case 1:
                    if (CheckChance(config.pest_resistance_i_chance))
                        return null;
                    break;
                case 2:
                    if (CheckChance(config.pest_resistance_ii_chance))
                        return null;
                    break;
                default:
                    ModEntry.GetMonitor().Log("Bug: GetPestRes returned wrong value", LogLevel.Error);
                    break;
            }

            Pest pest = new Pest(pos, hd);
            return pest;
        }

        public static double GetRandom()
        {
            return rand.NextDouble() * 100;
        }

        public static bool CheckChance(double chance)
        {
            double randd = GetRandom();
            //ModEntry.GetMonitor().Log("Got random number " + randd, LogLevel.Trace);
            //ModEntry.GetMonitor().Log("(chance <= GetRandom()) is " + (randd <= chance), LogLevel.Trace);
            return (randd <= chance);
        }

        public static Config GetConfig() => config;

        public static Texture2D GetPestTexture() => pestTexture;
        public static Rectangle GetPestRectangle() => pestRectangle;
    }

    public class Pest
    {
        class PestFly : Butterfly
        {
            private readonly Vector2 cropPos;

            public PestFly(Vector2 pos) : base(pos)
            {
                this.cropPos = AdjustPosition(pos * Game1.tileSize);
                this.position = this.cropPos;
                this.startingPosition = this.cropPos;
                           
                ModEntry.GetHelper().Reflection.GetField<bool>(this, "summerButterfly").SetValue(true);
                this.baseFrame = (128 + 4);
                this.sprite = new AnimatedSprite(Critter.critterTexture, baseFrame, 16, 16)
                {
                    loop = false
                };
            }

            private static Vector2 AdjustPosition(Vector2 p) => new Vector2(p.X, p.Y + (int)(Game1.tileSize * 1.9));


            public override bool update(GameTime time, GameLocation environment)
            {
                Vector2 oldPosition = ModEntry.GetHelper().Reflection.GetField<Vector2>(this, "position").GetValue();

                bool result = base.update(time, environment);

                float motionMultiplier = ModEntry.GetHelper().Reflection.GetField<float>(this, "motionMultiplier").GetValue();
                Vector2 motion = ModEntry.GetHelper().Reflection.GetField<Vector2>(this, "motion").GetValue();

                Vector2 newPos = oldPosition + motion * motionMultiplier;

                if (Math.Abs((int)newPos.X - (int)cropPos.X) < Game1.tileSize / 3 && Math.Abs((int)newPos.Y - (int)cropPos.Y) < Game1.tileSize / 2)
                    ModEntry.GetHelper().Reflection.GetField<Vector2>(this, "position").SetValue(newPos);
                else
                    ModEntry.GetHelper().Reflection.GetField<Vector2>(this, "position").SetValue(oldPosition);

                //ModEntry.GetMonitor().Log("update: pestfly pos is" + pos +", and p is "+p);
                return result;
            }

            public override void drawAboveFrontLayer(SpriteBatch b)
            {
                sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, position + new Vector2(16, -128f + yJumpOffset + yOffset)), position.Y / 10000f, 0, 0, Color.White, flip, 2f, 0f, false);
                //sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, position + new Vector2(-64f, -128f + yJumpOffset + yOffset)), position.Y / 10000f, 0, 0, Color.White, flip, 4f, 0f, false);
            }

            public void UpdateAndDraw(SpriteBatch b)
            {
                update(Game1.currentGameTime, Game1.getFarm());
                drawAboveFrontLayer(b);
            }

        }

        /// <summary>
        /// Tile of the infested crop on the farm map.
        /// </summary>
        private readonly Vector2 pos;

        /// <summary>
        /// Infested TerrainFeature (HoeDirt) with the infested crop.
        /// </summary>
        private readonly HoeDirt hd;

        private readonly List<Vector2> adjacentTiles = new List<Vector2>();

        private List<PestFly> bfList;

        public Pest(Vector2 pos, HoeDirt hd)
        {
            this.pos = pos;
            this.hd = hd;

            this.adjacentTiles.Add(new Vector2(pos.X - 1, pos.Y));
            this.adjacentTiles.Add(new Vector2(pos.X - 1, pos.Y - 1));
            this.adjacentTiles.Add(new Vector2(pos.X - 1, pos.Y + 1));
            this.adjacentTiles.Add(new Vector2(pos.X, pos.Y - 1));
            this.adjacentTiles.Add(new Vector2(pos.X, pos.Y + 1));
            this.adjacentTiles.Add(new Vector2(pos.X + 1, pos.Y));
            this.adjacentTiles.Add(new Vector2(pos.X + 1, pos.Y - 1));
            this.adjacentTiles.Add(new Vector2(pos.X + 1, pos.Y + 1));

            bfList = new List<PestFly>();
            bfList.Add(new PestFly(pos));
            if (Pests.GetRandom() >= 50)
                bfList.Add(new PestFly(pos));
            if (Pests.GetRandom() >= 50)
                bfList.Add(new PestFly(pos));
            //ModEntry.GetMonitor().Log("pos is " + pos + " and pestfly pos is" + pos * Game1.tileSize);
        }

        public Vector2 GetPos() => pos;

        public void Draw(SpriteBatch spriteBatch)
        {
            bfList.ForEach(bf => bf.UpdateAndDraw(spriteBatch));
        }

        public bool HasPosition(Vector2 pos)
        {
            return (Math.Abs(pos.X - this.pos.X) < 0.01) && (Math.Abs(pos.Y - this.pos.Y) < 0.01);
        }

        public List<Pest> InfestAdjacent(double chance)
        {
            List<Pest> pests = new List<Pest>();
            foreach (KeyValuePair<Vector2, TerrainFeature> pair in GetAdjacentCrops())
            {
                if (pair.Key is Vector2 vec && pair.Value is HoeDirt hd && Pests.CheckChance(chance))
                {
                    Pest pest = Pests.TryInfestCrop(vec, hd);
                    if (pest != null)
                    {
                        ModEntry.GetMonitor().Log("Infested adjacent crop at " + vec, LogLevel.Trace);
                        pests.Add(pest);
                    }

                }
            }
            return pests;
        }

        /// <summary>
        /// Pests eat the crop by decreasing its phase. If the phase reaches 0, the crop is set to dead.
        /// </summary>
        /// <returns><c>true</c>, if crop was killed or is non-existent, <c>false</c> otherwise.</returns>
        public bool NomNom()
        {
            if (hd.crop == null)
                return true;

            return Cultivation.UngrowCrop(hd.crop, pos, "Pest");
        }

        private List<KeyValuePair<Vector2, TerrainFeature>> GetAdjacentCrops()
        {
            List<KeyValuePair<Vector2, TerrainFeature>> pairs = new List<KeyValuePair<Vector2, TerrainFeature>>();

            foreach (Vector2 adjacent in adjacentTiles)
            {
                if (Game1.getFarm().terrainFeatures.ContainsKey(adjacent) && Game1.getFarm().terrainFeatures[adjacent] is HoeDirt hd && hd.crop != null)
                    pairs.Add(new KeyValuePair<Vector2, TerrainFeature>(adjacent, hd));
            }

            return pairs;
        }
           
    }
}
