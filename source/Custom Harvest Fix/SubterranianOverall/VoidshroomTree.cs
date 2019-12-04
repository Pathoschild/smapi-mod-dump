using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Dimensions;

namespace SubterranianOverhaul
{   
    public class VoidshroomTree :  TerrainFeature
    {   
        public static Microsoft.Xna.Framework.Rectangle treeTopSourceRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 48, 96);
        public static Microsoft.Xna.Framework.Rectangle stumpSourceRect = new Microsoft.Xna.Framework.Rectangle(32, 96, 16, 32);
        public static Microsoft.Xna.Framework.Rectangle shadowSourceRect = new Microsoft.Xna.Framework.Rectangle(663, 1011, 41, 30);
        [XmlElement("growthStage")]
        public readonly NetInt growthStage = new NetInt();
        [XmlElement("treeType")]
        public readonly NetInt treeType = new NetInt();
        [XmlElement("health")]
        public readonly NetFloat health = new NetFloat();
        [XmlElement("flipped")]
        public readonly NetBool flipped = new NetBool();
        [XmlElement("stump")]
        public readonly NetBool stump = new NetBool();
        [XmlElement("tapped")]
        public readonly NetBool tapped = new NetBool();
        [XmlElement("hasSeed")]
        public readonly NetBool hasSeed = new NetBool();
        [XmlElement("shakeLeft")]
        public readonly NetBool shakeLeft = new NetBool().Interpolated(false, false);
        [XmlElement("falling")]
        private readonly NetBool falling = new NetBool();
        [XmlElement("destroy")]
        private readonly NetBool destroy = new NetBool();
        private float alpha = 1f;
        private List<Leaf> leaves = new List<Leaf>();
        [XmlElement("lastPlayerToHit")]
        private readonly NetLong lastPlayerToHit = new NetLong();
        public float chanceForDailySeed = 0.05f;
        public float chanceForDailySpread = 0.05f;
        public const float shakeRate = 0.01570796f;
        public const float shakeDecayRate = 0.003067962f;
        public const int minWoodDebrisForFallenTree = 12;
        public const int minWoodDebrisForStump = 5;
        public const int startingHealth = 10;
        public const int leafFallRate = 3;
        public const int bushyTree = 1;
        public const int leafyTree = 2;
        public const int pineTree = 3;
        public const int winterTree1 = 4;
        public const int winterTree2 = 5;
        public const int palmTree = 6;
        public const int mushroomTree = 7;
        public const int seedStage = 0;
        public const int sproutStage = 1;
        public const int saplingStage = 2;
        public const int bushStage = 3;
        public const int treeStage = 5;
        private Lazy<Texture2D> texture;
        private string season;
        private float shakeRotation;
        private float maxShake;
        private float shakeTimer;

        private static IModHelper helper;
        private static IMonitor monitor;

        public VoidshroomTree() : base(true)
        {
            this.resetTexture();
            this.NetFields.AddFields((INetSerializable)this.growthStage, (INetSerializable)this.treeType, (INetSerializable)this.health, (INetSerializable)this.flipped, (INetSerializable)this.stump, (INetSerializable)this.tapped, (INetSerializable)this.hasSeed, (INetSerializable)this.shakeLeft, (INetSerializable)this.falling, (INetSerializable)this.destroy, (INetSerializable)this.lastPlayerToHit);
            this.treeType.fieldChangeVisibleEvent += (NetFieldBase<int, NetInt>.FieldChange)((a, b, c) => this.resetTexture());
        }

        public VoidshroomTree(int growthStage) : this()
        {
            this.growthStage.Value = growthStage;
            this.flipped.Value = Game1.random.NextDouble() < 0.5;
            this.health.Value = 10f;
        }

        public VoidshroomTree(VoidshroomtreeSaveData savedData) : this()
        {
            this.growthStage.Value = savedData.growthStage;
            this.hasSeed.Value = savedData.hasSeed;
            this.health.Value = savedData.health;
            this.stump.Value = savedData.stump;
            this.flipped.Value = savedData.flipped;
            this.tapped.Value = savedData.tapped;
        }

        protected void resetTexture()
        {
            this.texture = new Lazy<Texture2D>(new Func<Texture2D>(this.loadTexture));
        }

        protected Texture2D loadTexture()
        {
            return TextureSet.voidShroomTree;
        }

        public override Microsoft.Xna.Framework.Rectangle getBoundingBox(Vector2 tileLocation)
        {
            return new Microsoft.Xna.Framework.Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64);
        }

        public override Microsoft.Xna.Framework.Rectangle getRenderBounds(Vector2 tileLocation)
        {
            if (this.stump.Value || this.growthStage.Value < 5)
                return new Microsoft.Xna.Framework.Rectangle((int)((double)tileLocation.X - 0.0) * 64, (int)((double)tileLocation.Y - 1.0) * 64, 64, 128);
            return new Microsoft.Xna.Framework.Rectangle((int)((double)tileLocation.X - 1.0) * 64, (int)((double)tileLocation.Y - 5.0) * 64, 192, 448);
        }

        public override bool performUseAction(Vector2 tileLocation, GameLocation location)
        {
            if (!this.tapped.Value)
            {
                if ((double)this.maxShake == 0.0 && !this.stump.Value && this.growthStage.Value >= 3)
                    location.localSound("leafrustle");
                this.shake(tileLocation, false);
            }
            return false;
        }

        private int extraWoodCalculator(Vector2 tileLocation)
        {
            Random random = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + (int)tileLocation.X * 7 + (int)tileLocation.Y * 11);
            int num = 0;
            if (random.NextDouble() < Game1.player.DailyLuck)
                ++num;
            if (random.NextDouble() < (double)Game1.player.ForagingLevel / 12.5)
                ++num;
            if (random.NextDouble() < (double)Game1.player.ForagingLevel / 12.5)
                ++num;
            if (random.NextDouble() < (double)Game1.player.LuckLevel / 25.0)
                ++num;
            return num;
        }

        public override bool tickUpdate(GameTime time, Vector2 tileLocation, GameLocation location)
        {
            if (this.season != Game1.currentSeason)
            {
                this.resetTexture();
                this.season = Game1.currentSeason;
            }
            if ((double)this.shakeTimer > 0.0)
                this.shakeTimer -= (float)time.ElapsedGameTime.Milliseconds;
            if (this.destroy.Value)
                return true;
            this.alpha = Math.Min(1f, this.alpha + 0.05f);
            if (this.growthStage.Value >= 5 && !this.falling.Value && !this.stump.Value && Game1.player.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle(64 * ((int)tileLocation.X - 1), 64 * ((int)tileLocation.Y - 5), 192, 288)))
                this.alpha = Math.Max(0.4f, this.alpha - 0.09f);
            if (!this.falling.Value)
            {
                if ((double)Math.Abs(this.shakeRotation) > Math.PI / 2.0 && this.leaves.Count <= 0 && (double)this.health.Value <= 0.0)
                    return true;
                if ((double)this.maxShake > 0.0)
                {
                    if (this.shakeLeft.Value)
                    {
                        this.shakeRotation -= this.growthStage.Value >= 5 ? (float)Math.PI / 600f : (float)Math.PI / 200f;
                        if ((double)this.shakeRotation <= -(double)this.maxShake)
                            this.shakeLeft.Value = false;
                    }
                    else
                    {
                        this.shakeRotation += this.growthStage.Value >= 5 ? (float)Math.PI / 600f : (float)Math.PI / 200f;
                        if ((double)this.shakeRotation >= (double)this.maxShake)
                            this.shakeLeft.Value = true;
                    }
                }
                if ((double)this.maxShake > 0.0)
                    this.maxShake = Math.Max(0.0f, this.maxShake - (this.growthStage.Value >= 5 ? 0.001022654f : 0.003067962f));
            }
            else
            {
                this.shakeRotation += this.shakeLeft.Value ? (float)-((double)this.maxShake * (double)this.maxShake) : this.maxShake * this.maxShake;
                this.maxShake += 0.001533981f;                
                if ((double)Math.Abs(this.shakeRotation) > Math.PI / 2.0)
                {
                    this.falling.Value = false;
                    this.maxShake = 0.0f;
                    location.localSound("treethud");
                    int num = Game1.random.Next(90, 120);
                    if (Game1.currentLocation.Objects.ContainsKey(tileLocation))
                        Game1.currentLocation.Objects.Remove(tileLocation);
                    for (int index = 0; index < num; ++index)
                        this.leaves.Add(new Leaf(new Vector2((float)(Game1.random.Next((int)((double)tileLocation.X * 64.0), (int)((double)tileLocation.X * 64.0 + 192.0)) + (this.shakeLeft.Value ? -320 : 256)), (float)((double)tileLocation.Y * 64.0 - 64.0)), (float)Game1.random.Next(-10, 10) / 100f, Game1.random.Next(4), (float)Game1.random.Next(10, 40) / 10f));

                    Game1.createRadialDebris(Game1.currentLocation, 12, (int)tileLocation.X + (((NetFieldBase<bool, NetBool>)this.shakeLeft).Value ? -4 : 4), (int)tileLocation.Y, 12 + this.extraWoodCalculator(tileLocation), true, -1, false, -1);

                    int number = 0;
                    if (Game1.getFarmer(this.lastPlayerToHit.Value) != null)
                    {
                        while (Game1.getFarmer(this.lastPlayerToHit.Value).professions.Contains(14) && Game1.random.NextDouble() < 0.4)
                            ++number;
                    }
                    if (number > 0)
                        Game1.createMultipleObjectDebris(709, (int)tileLocation.X + ((bool)((NetFieldBase<bool, NetBool>)this.shakeLeft).Value ? -4 : 4), (int)tileLocation.Y, number);
                    //seed dropping code. Revisit if I get spores working.
                    int seedIndex = VoidshroomSpore.getIndex();
                    if (this.lastPlayerToHit.Value != 0L && Game1.getFarmer(this.lastPlayerToHit.Value).getEffectiveSkillLevel(2) >= 1 && (Game1.random.NextDouble() < 0.75 && ((NetFieldBase<int, NetInt>)this.treeType).Value < 4) && seedIndex != -1)
                        Game1.createMultipleObjectDebris(seedIndex, (int)tileLocation.X + (((NetFieldBase<bool, NetBool>)this.shakeLeft).Value ? -4 : 4), (int)tileLocation.Y, Game1.random.Next(1, 3));
                    if ((double)this.health.Value == -100.0)
                        return true;
                    if ((double)this.health.Value <= 0.0)
                        this.health.Value = -100f;
                }
            }
            for (int index = this.leaves.Count - 1; index >= 0; --index)
            {
                this.leaves.ElementAt<Leaf>(index).position.Y -= this.leaves.ElementAt<Leaf>(index).yVelocity - 3f;
                this.leaves.ElementAt<Leaf>(index).yVelocity = Math.Max(0.0f, this.leaves.ElementAt<Leaf>(index).yVelocity - 0.01f);
                this.leaves.ElementAt<Leaf>(index).rotation += this.leaves.ElementAt<Leaf>(index).rotationRate;
                if ((double)this.leaves.ElementAt<Leaf>(index).position.Y >= (double)tileLocation.Y * 64.0 + 64.0)
                    this.leaves.RemoveAt(index);
            }
            return false;
        }

        private void shake(Vector2 tileLocation, bool doEvenIfStillShaking)
        {
            if ((double)this.maxShake == 0.0 | doEvenIfStillShaking && this.growthStage.Value >= 3 && !this.stump.Value)
            {
                this.shakeLeft.Value = (double)Game1.player.getTileLocation().X > (double)tileLocation.X || (double)Game1.player.getTileLocation().X == (double)tileLocation.X && Game1.random.NextDouble() < 0.5;
                this.maxShake = this.growthStage.Value >= 5 ? (float)Math.PI / 128f : (float)Math.PI / 64f;
                if (this.growthStage.Value >= 5)
                {
                    
                    if (Game1.random.NextDouble() < 0.66)
                    {
                        int num = Game1.random.Next(1, 6);
                        for (int index = 0; index < num; ++index)
                            this.leaves.Add(new Leaf(new Vector2((float)Game1.random.Next((int)((double)tileLocation.X * 64.0 - 64.0), (int)((double)tileLocation.X * 64.0 + 128.0)), (float)Game1.random.Next((int)((double)tileLocation.Y * 64.0 - 256.0), (int)((double)tileLocation.Y * 64.0 - 192.0))), (float)Game1.random.Next(-10, 10) / 100f, Game1.random.Next(4), (float)Game1.random.Next(5) / 10f));
                    }
                    if (Game1.random.NextDouble() < 0.01 && (Game1.currentSeason.Equals("spring") || Game1.currentSeason.Equals("summer")))
                    {
                        while (Game1.random.NextDouble() < 0.8)
                            Game1.currentLocation.addCritter((Critter)new Butterfly(new Vector2(tileLocation.X + (float)Game1.random.Next(1, 3), tileLocation.Y - 2f + (float)Game1.random.Next(-1, 2))));
                    }
                    if (!this.hasSeed.Value || !Game1.IsMultiplayer && Game1.player.ForagingLevel < 1)
                        return;

                     //Add this back in once we've added the seed object for these Voidshroom Trees
                    int objectIndex = VoidshroomSpore.getIndex();
                    if (objectIndex != -1)
                        Game1.createObjectDebris(objectIndex, (int)tileLocation.X, (int)tileLocation.Y - 3, ((int)tileLocation.Y + 1) * 64, 0, 1f, (GameLocation)null);
                    this.hasSeed.Value = false;
                    
                }
                else
                {
                    if (Game1.random.NextDouble() >= 0.66)
                        return;
                    int num = Game1.random.Next(1, 3);
                    for (int index = 0; index < num; ++index)
                        this.leaves.Add(new Leaf(new Vector2((float)Game1.random.Next((int)((double)tileLocation.X * 64.0), (int)((double)tileLocation.X * 64.0 + 48.0)), (float)((double)tileLocation.Y * 64.0 - 32.0)), (float)Game1.random.Next(-10, 10) / 100f, Game1.random.Next(4), (float)Game1.random.Next(30) / 10f));
                }
            }
            else
            {
                if (!this.stump.Value)
                    return;
                this.shakeTimer = 100f;
            }
        }

        public override bool isPassable(Character c = null)
        {
            return (double)this.health.Value <= -99.0 || this.growthStage.Value == 0;
        }

        public override void dayUpdate(GameLocation environment, Vector2 tileLocation)
        {
            Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int)(((double)tileLocation.X - 1.0) * 64.0), (int)(((double)tileLocation.Y - 1.0) * 64.0), 192, 192);
            if ((double)this.health.Value <= -100.0)
                this.destroy.Value = true;

            
            string str = environment.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "NoSpawn", "Back");
            if (str != null && (str.Equals("All") || str.Equals(nameof(Tree)) || str.Equals("True")))
                return;
            if (this.growthStage.Value == 4)
            {
                foreach (KeyValuePair<Vector2, TerrainFeature> pair in environment.terrainFeatures.Pairs)
                {
                    if (pair.Value is Tree && !pair.Value.Equals((object)this) && (((Tree)pair.Value).growthStage.Value) >= 5 && pair.Value.getBoundingBox(pair.Key).Intersects(rectangle))
                        return;
                }
            }
            else if (this.growthStage.Value == 0 && environment.objects.ContainsKey(tileLocation))
                return;
            if (Game1.random.NextDouble() < 0.2)
                ++this.growthStage.Value;
            

            if (this.growthStage.Value >= 5 && environment is FarmCave && Game1.random.NextDouble() < chanceForDailySpread)
            {
                int xTile = Game1.random.Next(-3, 4) + (int)tileLocation.X;
                int yTile = Game1.random.Next(-3, 4) + (int)tileLocation.Y;
                Vector2 vector2 = new Vector2((float)xTile, (float)yTile);
                string str2 = environment.doesTileHaveProperty(xTile, yTile, "NoSpawn", "Back");
                if ((str2 == null || !str2.Equals(nameof(Tree)) && !str2.Equals("All") && !str2.Equals("True")) && (environment.isTileLocationOpen(new Location(xTile * 64, yTile * 64)) && !environment.isTileOccupied(vector2, "") && (environment.doesTileHaveProperty(xTile, yTile, "Water", "Back") == null && environment.isTileOnMap(vector2))))
                {
                    environment.terrainFeatures.Add(vector2, (TerrainFeature)new VoidshroomTree(0));
                }   
            }

            this.hasSeed.Value = false;
            if (this.growthStage.Value < 5 || Game1.random.NextDouble() >= chanceForDailySeed)
                return;
            this.hasSeed.Value = true;
        }

        public override bool seasonUpdate(bool onLoad)
        {
            this.loadSprite();
            return false;
        }

        public override bool isActionable()
        {
            if (!this.tapped.Value)
                return this.growthStage.Value >= 3;
            return false;
        }

        public override bool performToolAction(
        Tool t,
        int explosion,
        Vector2 tileLocation,
        GameLocation location)
        {
            if (location == null)
                location = Game1.currentLocation;
            if (explosion > 0)
                this.tapped.Value = false;
            if (this.tapped.Value)
                return false;
            Console.WriteLine("TREE: IsClient:" + Game1.IsClient.ToString() + " randomOutput: " + (object)Game1.recentMultiplayerRandom.Next(9999));
            if ((double)this.health.Value <= -99.0)
                return false;
            if (this.growthStage.Value >= 5)
            {
                if (t != null && t is Axe)
                {
                    location.playSound("axchop");
                    location.debris.Add(new Debris(12, Game1.random.Next(1, 3), t.getLastFarmerToUse().GetToolLocation(false) + new Vector2(16f, 0.0f), t.getLastFarmerToUse().Position, 0, -1));
                    this.lastPlayerToHit.Value = t.getLastFarmerToUse().UniqueMultiplayerID;
                    if (!this.stump.Value && t.getLastFarmerToUse() != null && (t.getLastFarmerToUse().hasMagnifyingGlass && Game1.random.NextDouble() < 0.005))
                    {
                        StardewValley.Object unseenSecretNote = location.tryToCreateUnseenSecretNote(t.getLastFarmerToUse());
                        if (unseenSecretNote != null)
                            Game1.createItemDebris((Item)unseenSecretNote, new Vector2(tileLocation.X, tileLocation.Y - 3f) * 64f, -1, (GameLocation)null, Game1.player.getStandingY() - 32);
                    }
                }
                else if (explosion <= 0)
                    return false;
                this.shake(tileLocation, true);
                float num = 1f;
                if (explosion > 0)
                {
                    num = (float)explosion;
                }
                else
                {
                    if (t == null)
                        return false;
                    switch (t.UpgradeLevel)
                    {
                        case 0:
                            num = 1f;
                            break;
                        case 1:
                            num = 1.25f;
                            break;
                        case 2:
                            num = 1.67f;
                            break;
                        case 3:
                            num = 2.5f;
                            break;
                        case 4:
                            num = 5f;
                            break;
                    }
                }
                this.health.Value -= num;
                if ((double)this.health.Value <= 0.0)
                {
                    if (!this.stump.Value)
                    {
                        if (t != null || explosion > 0)
                            location.playSound("treecrack");
                        this.stump.Value = true;
                        this.health.Value = 5f;
                        this.falling.Value = true;
                        if (t != null && t.getLastFarmerToUse().IsLocalPlayer)
                        {
                            t?.getLastFarmerToUse().gainExperience(2, 12);
                            if (t == null || t.getLastFarmerToUse() == null)
                                this.shakeLeft.Value = true;
                            else
                                this.shakeLeft.Value = (double)t.getLastFarmerToUse().getTileLocation().X > (double)tileLocation.X || (double)t.getLastFarmerToUse().getTileLocation().Y < (double)tileLocation.Y && (double)tileLocation.X % 2.0 == 0.0;
                        }
                    }
                    else
                    {
                        if (t != null && (double)this.health.Value != -100.0 && (t.getLastFarmerToUse().IsLocalPlayer && t != null))
                            t.getLastFarmerToUse().gainExperience(2, 1);
                        this.health.Value = -100f;
                        Game1.createRadialDebris(location, 12, (int)tileLocation.X, (int)tileLocation.Y, Game1.random.Next(30, 40), false, -1, false, -1);

                        //this produces eitehr sap or mushrooms currently.  Tweak perhaps to drop voidshroom bark to process into wood? Just drop wood directly?
                        int index = (double)tileLocation.X % 7.0 != 0.0 ? 388 : 709;
                        if (Game1.IsMultiplayer)
                        {
                            Game1.recentMultiplayerRandom = new Random((int)tileLocation.X * 2000 + (int)tileLocation.Y);
                            Random multiplayerRandom = Game1.recentMultiplayerRandom;
                        }
                        else
                        {
                            Random random = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + (int)tileLocation.X * 7 + (int)tileLocation.Y * 11);
                        }
                        if (t == null || t.getLastFarmerToUse() == null)
                        {
                            if (location.Equals(Game1.currentLocation))
                            {
                                Game1.createMultipleObjectDebris(92, (int)tileLocation.X, (int)tileLocation.Y, 2);
                            }
                            else
                            {
                                Game1.createItemDebris((Item)new StardewValley.Object(92, 1, false, -1, 0), tileLocation * 64f, 2, location, -1);
                                Game1.createItemDebris((Item)new StardewValley.Object(92, 1, false, -1, 0), tileLocation * 64f, 2, location, -1);
                            }
                        }
                        else if (Game1.IsMultiplayer)
                        {
                            Game1.createMultipleObjectDebris(index, (int)tileLocation.X, (int)tileLocation.Y, 1, this.lastPlayerToHit.Value);
                            
                             Game1.createRadialDebris(location, 12, (int)tileLocation.X, (int)tileLocation.Y, 4, true, -1, false, -1);
                        }
                        else
                        {   
                            Game1.createRadialDebris(location, 12, (int)tileLocation.X, (int)tileLocation.Y, 5 + this.extraWoodCalculator(tileLocation), true, -1, false, -1);
                            Game1.createMultipleObjectDebris(index, (int)tileLocation.X, (int)tileLocation.Y, 1);
                        }
                        location.playSound("treethud");
                        if (!this.falling.Value)
                            return true;
                    }
                }
            }
            else if (this.growthStage.Value >= 3)
            {
                if (t != null && t.BaseName.Contains("Ax"))
                {
                    location.playSound("axchop");
                    location.debris.Add(new Debris(12, Game1.random.Next(t.UpgradeLevel * 2, t.UpgradeLevel * 4), t.getLastFarmerToUse().GetToolLocation(false) + new Vector2(16f, 0.0f), new Vector2((float)t.getLastFarmerToUse().GetBoundingBox().Center.X, (float)t.getLastFarmerToUse().GetBoundingBox().Center.Y), 0, -1));
                }
                else if (explosion <= 0)
                    return false;
                this.shake(tileLocation, true);
                float num = 1f;
                if (Game1.IsMultiplayer)
                {
                    Random multiplayerRandom = Game1.recentMultiplayerRandom;
                }
                else
                {
                    Random random = new Random((int)((double)Game1.uniqueIDForThisGame + (double)tileLocation.X * 7.0 + (double)tileLocation.Y * 11.0 + (double)Game1.stats.DaysPlayed + (double)this.health.Value));
                }
                if (explosion > 0)
                {
                    num = (float)explosion;
                }
                else
                {
                    switch (t.UpgradeLevel)
                    {
                        case 0:
                            num = 2f;
                            break;
                        case 1:
                            num = 2.5f;
                            break;
                        case 2:
                            num = 3.34f;
                            break;
                        case 3:
                            num = 5f;
                            break;
                        case 4:
                            num = 10f;
                            break;
                    }
                }
                this.health.Value -= num;
                if ((double)this.health.Value <= 0.0)
                {
                    Game1.createDebris(12, (int)tileLocation.X, (int)tileLocation.Y, 4, (GameLocation)null);
                    Game1.createRadialDebris(location, 12, (int)tileLocation.X, (int)tileLocation.Y, Game1.random.Next(20, 30), false, -1, false, -1);
                    return true;
                }
            }
            else if (this.growthStage.Value >= 1)
            {
                if (explosion > 0)
                    return true;
                location.playSound("cut");
                if (t != null && t.BaseName.Contains("Axe"))
                {
                    location.playSound("axchop");
                    Game1.createRadialDebris(location, 12, (int)tileLocation.X, (int)tileLocation.Y, Game1.random.Next(10, 20), false, -1, false, -1);
                }
                if (t is Axe || t is Pickaxe || (t is Hoe || t is MeleeWeapon))
                {
                    Game1.createRadialDebris(location, 12, (int)tileLocation.X, (int)tileLocation.Y, Game1.random.Next(10, 20), false, -1, false, -1);
                    if (t.BaseName.Contains("Axe") && Game1.recentMultiplayerRandom.NextDouble() < (double)t.getLastFarmerToUse().ForagingLevel / 10.0)
                        Game1.createDebris(12, (int)tileLocation.X, (int)tileLocation.Y, 1, (GameLocation)null);
                    //TODO: come back and figure out this sprite broadcasting thing
                    //Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(17, tileLocation * 64f, Color.White, 8, false, 100f, 0, -1, -1f, -1, 0));
                    return true;
                }
            }
            else
            {
                if (explosion > 0)
                    return true;
                if (t.BaseName.Contains("Axe") || t.BaseName.Contains("Pick") || t.BaseName.Contains("Hoe"))
                {
                    location.playSound("woodyHit");
                    location.playSound("axchop");
                    //TODO: come back and figure out this sprite broadcasting thing
                    //Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(17, tileLocation * 64f, Color.White, 8, false, 100f, 0, -1, -1f, -1, 0));
                    int seedIndex = VoidshroomSpore.getIndex();
                    if ((long)this.lastPlayerToHit.Value != 0L && Game1.getFarmer((long)this.lastPlayerToHit.Value).getEffectiveSkillLevel(2) >= 1 && seedIndex != -1)
                    {
                        Game1.createMultipleObjectDebris(seedIndex, (int)tileLocation.X, (int)tileLocation.Y, 1, t.getLastFarmerToUse().UniqueMultiplayerID, location);
                    } else if (Game1.player.getEffectiveSkillLevel(2) >= 1 && seedIndex != -1)
                    {   
                        Game1.createMultipleObjectDebris(seedIndex, (int)tileLocation.X, (int)tileLocation.Y, 1, (long)t.getLastFarmerToUse().UniqueMultiplayerID, location);
                    }

                    return true;
                }
            }
            return false;
        }

        public override void drawInMenu(
            SpriteBatch spriteBatch,
            Vector2 positionOnScreen,
            Vector2 tileLocation,
            float scale,
            float layerDepth)
        {
            layerDepth += positionOnScreen.X / 100000f;
            if (this.growthStage.Value < 5)
            {
                Microsoft.Xna.Framework.Rectangle rectangle = Microsoft.Xna.Framework.Rectangle.Empty;
                switch (this.growthStage.Value)
                {
                    case 0:
                        rectangle = new Microsoft.Xna.Framework.Rectangle(32, 128, 16, 16);
                        break;
                    case 1:
                        rectangle = new Microsoft.Xna.Framework.Rectangle(0, 128, 16, 16);
                        break;
                    case 2:
                        rectangle = new Microsoft.Xna.Framework.Rectangle(16, 128, 16, 16);
                        break;
                    default:
                        rectangle = new Microsoft.Xna.Framework.Rectangle(0, 96, 16, 32);
                        break;
                }
                spriteBatch.Draw(this.texture.Value, positionOnScreen - new Vector2(0.0f, (float)rectangle.Height * scale), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, scale, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + (float)(((double)positionOnScreen.Y + (double)rectangle.Height * (double)scale) / 20000.0));
            }
            else
            {
                if (!this.falling.Value)
                    spriteBatch.Draw(this.texture.Value, positionOnScreen + new Vector2(0.0f, -64f * scale), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(32, 96, 16, 32)), Color.White, 0.0f, Vector2.Zero, scale, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + (float)(((double)positionOnScreen.Y + 448.0 * (double)scale - 1.0) / 20000.0));
                if (this.stump.Value && !this.falling.Value)
                    return;
                spriteBatch.Draw(this.texture.Value, positionOnScreen + new Vector2(-64f * scale, -320f * scale), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 48, 96)), Color.White, this.shakeRotation, Vector2.Zero, scale, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + (float)(((double)positionOnScreen.Y + 448.0 * (double)scale) / 20000.0));
            }
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            Microsoft.Xna.Framework.Rectangle boundingBox;
            if (this.growthStage.Value < 5)
            {
                Microsoft.Xna.Framework.Rectangle rectangle = Microsoft.Xna.Framework.Rectangle.Empty;
                switch (this.growthStage.Value)
                {
                    case 0:
                        rectangle = new Microsoft.Xna.Framework.Rectangle(32, 128, 16, 16);
                        break;
                    case 1:
                        rectangle = new Microsoft.Xna.Framework.Rectangle(0, 128, 16, 16);
                        break;
                    case 2:
                        rectangle = new Microsoft.Xna.Framework.Rectangle(16, 128, 16, 16);
                        break;
                    default:
                        rectangle = new Microsoft.Xna.Framework.Rectangle(0, 96, 16, 32);
                        break;
                }
                SpriteBatch spriteBatch1 = spriteBatch;
                Texture2D texture = this.texture.Value;
                Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((double)tileLocation.X * 64.0 + 32.0), (float)((double)tileLocation.Y * 64.0 - (double)(rectangle.Height * 4 - 64) + (this.growthStage.Value >= 3 ? 128.0 : 64.0))));
                Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(rectangle);
                Color white = Color.White;
                double shakeRotation = (double)this.shakeRotation;
                Vector2 origin = new Vector2(8f, this.growthStage.Value >= 3 ? 32f : 16f);
                int num1 = this.flipped.Value ? 1 : 0;
                double num2;
                if (this.growthStage.Value != 0)
                {
                    boundingBox = this.getBoundingBox(tileLocation);
                    num2 = (double)boundingBox.Bottom / 10000.0;
                }
                else
                    num2 = 9.99999974737875E-05;
                spriteBatch1.Draw(texture, local, sourceRectangle, white, (float)shakeRotation, origin, 4f, (SpriteEffects)num1, (float)num2);
            }
            else
            {
                if (!this.stump.Value || this.falling.Value)
                {
                    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((double)tileLocation.X * 64.0 - 51.0), (float)((double)tileLocation.Y * 64.0 - 16.0))), new Microsoft.Xna.Framework.Rectangle?(Tree.shadowSourceRect), Color.White * (1.570796f - Math.Abs(this.shakeRotation)), 0.0f, Vector2.Zero, 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1E-06f);
                    SpriteBatch spriteBatch1 = spriteBatch;
                    Texture2D texture = this.texture.Value;
                    Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((double)tileLocation.X * 64.0 + 32.0), (float)((double)tileLocation.Y * 64.0 + 64.0)));
                    Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Tree.treeTopSourceRect);
                    Color color = Color.White * this.alpha;
                    double shakeRotation = (double)this.shakeRotation;
                    Vector2 origin = new Vector2(24f, 96f);
                    int num1 = this.flipped.Value ? 1 : 0;
                    boundingBox = this.getBoundingBox(tileLocation);
                    double num2 = (double)(boundingBox.Bottom + 2) / 10000.0 - (double)tileLocation.X / 1000000.0;
                    spriteBatch1.Draw(texture, local, sourceRectangle, color, (float)shakeRotation, origin, 4f, (SpriteEffects)num1, (float)num2);
                }
                if ((double)this.health.Value >= 1.0 || !this.falling.Value && (double)this.health.Value > -99.0)
                {
                    SpriteBatch spriteBatch1 = spriteBatch;
                    Texture2D texture = this.texture.Value;
                    Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((double)tileLocation.X * 64.0 + ((double)this.shakeTimer > 0.0 ? Math.Sin(2.0 * Math.PI / (double)this.shakeTimer) * 3.0 : 0.0)), (float)((double)tileLocation.Y * 64.0 - 64.0)));
                    Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Tree.stumpSourceRect);
                    Color color = Color.White * this.alpha;
                    Vector2 zero = Vector2.Zero;
                    int num1 = this.flipped.Value ? 1 : 0;
                    boundingBox = this.getBoundingBox(tileLocation);
                    double num2 = (double)boundingBox.Bottom / 10000.0;
                    spriteBatch1.Draw(texture, local, sourceRectangle, color, 0.0f, zero, 4f, (SpriteEffects)num1, (float)num2);
                }
                if (this.stump.Value && (double)this.health.Value < 4.0 && (double)this.health.Value > -99.0)
                {
                    SpriteBatch spriteBatch1 = spriteBatch;
                    Texture2D texture = this.texture.Value;
                    Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((double)tileLocation.X * 64.0 + ((double)this.shakeTimer > 0.0 ? Math.Sin(2.0 * Math.PI / (double)this.shakeTimer) * 3.0 : 0.0)), tileLocation.Y * 64f));
                    Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(Math.Min(2, (int)(3.0 - (double)this.health.Value)) * 16, 144, 16, 16));
                    Color color = Color.White * this.alpha;
                    Vector2 zero = Vector2.Zero;
                    int num1 = this.flipped.Value ? 1 : 0;
                    boundingBox = this.getBoundingBox(tileLocation);
                    double num2 = (double)(boundingBox.Bottom + 1) / 10000.0;
                    spriteBatch1.Draw(texture, local, sourceRectangle, color, 0.0f, zero, 4f, (SpriteEffects)num1, (float)num2);
                }
            }
            foreach (Leaf leaf in this.leaves)
            {
                SpriteBatch spriteBatch1 = spriteBatch;
                Texture2D texture = this.texture.Value;
                Vector2 local = Game1.GlobalToLocal(Game1.viewport, leaf.position);
                Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(16 + leaf.type % 2 * 8, 112 + leaf.type / 2 * 8, 8, 8));
                Color white = Color.White;
                double rotation = (double)leaf.rotation;
                Vector2 zero = Vector2.Zero;
                boundingBox = this.getBoundingBox(tileLocation);
                double num = (double)boundingBox.Bottom / 10000.0 + 0.00999999977648258;
                spriteBatch1.Draw(texture, local, sourceRectangle, white, (float)rotation, zero, 4f, SpriteEffects.None, (float)num);
            }
        }

        public static void SetHelper()
        {
            if(VoidshroomTree.helper == null)
            {
                VoidshroomTree.helper = ModEntry.GetHelper();
            }
        }

        public static void SetMonitor()
        {
            if (VoidshroomTree.monitor == null)
            {
                VoidshroomTree.monitor = ModEntry.GetMonitor();
            }
        }

        public VoidshroomtreeSaveData GetSaveData()
        {
            return new VoidshroomtreeSaveData(this);
        }

        //stealing (or at least borrowing heavily) this remove/readd method from Deep Woods pretty much whole cloth.

        private static HashSet<GameLocation> processedLocations = new HashSet<GameLocation>();

        public static void RemovalAll()
        {
            SetHelper();
            SetMonitor();

            if (!Game1.IsMasterGame)
                return;

            monitor.Log("VoidshroomTree.RemovalAll()", StardewModdingAPI.LogLevel.Trace);
            ModState.voidshroomTreeLocations.Clear();

            processedLocations.Clear();

            foreach (GameLocation location in Game1.locations)
            {
                ProcessLocation(location, ProcessingMethod.Remove);
            }

            processedLocations.Clear();
        }

        public static void ReplaceAll()
        {
            SetHelper();
            SetMonitor();

            if (!Game1.IsMasterGame)
                return;

            monitor.Log("VoidshroomTree.ReplaceAll()", StardewModdingAPI.LogLevel.Trace);
            //ModState.voidshroomTreeLocations.Clear();

            processedLocations.Clear();

            foreach (GameLocation location in Game1.locations)
            {
                ProcessLocation(location, ProcessingMethod.Restore);
            }

            processedLocations.Clear();
        }

        public static void ProcessLocation(GameLocation location, ProcessingMethod method)
        {
            if (location == null)
                return;

            monitor.Log("VoidshroomTree.ProcessLocation(" + location.Name + ", " + method + ")", StardewModdingAPI.LogLevel.Trace);

            if (processedLocations.Contains(location))
            {
                monitor.Log("VoidshroomTree.ProcessLocation(" + location.Name + ", " + method + "): Already processed this location (infinite recursion?), aborting!", StardewModdingAPI.LogLevel.Warn);
                return;
            }

            processedLocations.Add(location);

            bool itemsToRemove = false;

            if (VoidshroomSpore.IsValidLocation(location))
            {
                String locationName = location.Name.ToString();
                if (method.Equals(ProcessingMethod.Remove))
                {

                    foreach (Vector2 featureSpot in location.terrainFeatures.Keys)
                    {
                        if (location.terrainFeatures[featureSpot] is VoidshroomTree)
                        {

                            StringWriter writer = new StringWriter();

                            if (!ModState.voidshroomTreeLocations.ContainsKey(locationName))
                            {
                                ModState.voidshroomTreeLocations.Add(locationName, new Dictionary<Vector2, VoidshroomtreeSaveData>());
                            }

                            VoidshroomTree tree = (VoidshroomTree)location.terrainFeatures[featureSpot];

                            ModState.voidshroomTreeLocations[locationName].Add(featureSpot, tree.GetSaveData());
                            itemsToRemove = true;
                        }
                    }

                    //data is stored, but if we're removing we need to actually clear stuff out of the list now.
                    if (itemsToRemove)
                    {
                        foreach (Vector2 locationData in ModState.voidshroomTreeLocations[location.Name.ToString()].Keys)
                        {
                            location.terrainFeatures.Remove(locationData);
                        }
                    }
                }
                else if (method.Equals(ProcessingMethod.Restore))
                {
                    if (ModState.voidshroomTreeLocations.ContainsKey(location.Name.ToString()))
                    {
                        foreach (KeyValuePair<Vector2, VoidshroomtreeSaveData> locationData in ModState.voidshroomTreeLocations[location.Name.ToString()])
                        {
                            location.terrainFeatures.Add(locationData.Key, new VoidshroomTree(locationData.Value));
                        }
                    }
                }
            }
        }
    }
}
