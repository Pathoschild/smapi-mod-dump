/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jpparajeles/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.TerrainFeatures;
using Netcode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WildFlowersReimagined
{
    public class FlowerGrass : Grass
    {
        private readonly HoeDirt fakeDirt = new();

        private readonly NetRef<Crop> netCrop = new();
        
        private List<Action<GameLocation, Vector2>> queuedActions = new();

        public Crop Crop
        {
            get
            {
                return netCrop.Value;
            }
            set
            {
                netCrop.Value = value;
            }
        }

        public FlowerGrassConfig FlowerGrassConfig { get; set; }

        public override void initNetFields()
        {
            base.initNetFields();
            base.NetFields.AddField(netCrop, "netCrop");
            netCrop.Interpolated(interpolate: false, wait: false);
            netCrop.OnConflictResolve += delegate (Crop rejected, Crop accepted)
            {
                if (Game1.IsMasterGame && rejected != null && rejected.netSeedIndex.Value != null)
                {
                    queuedActions.Add(delegate (GameLocation gLocation, Vector2 tileLocation)
                    {
                        Vector2 vector = tileLocation * 64f;
                        gLocation.debris.Add(new Debris(rejected.netSeedIndex, vector, vector));
                    });
                    base.NeedsUpdate = true;
                }
            };

        }

        public FlowerGrass() : base()
        {
            Location = Game1.currentLocation;
            FlowerGrassConfig = new FlowerGrassConfig();
        }

        public FlowerGrass(int which, int numberOfWeeds, Crop crop, FlowerGrassConfig flowerGrassConfig) : this()
        {
            grassType.Value = (byte)which;
            loadSprite();
            this.numberOfWeeds.Value = numberOfWeeds;
            this.Crop = crop;
            this.FlowerGrassConfig = flowerGrassConfig;

        }

        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);
            Vector2 tile = Tile;
            if (this.Crop != null)
            {
                Crop.draw(spriteBatch, tile, Color.White, shakeRotation);
            }
        }

        private void Harvest(Vector2 tileLocation)
        {
            this.Crop.harvest((int)tileLocation.X, (int)tileLocation.Y, fakeDirt);
            this.Crop = null;
        }

        public override bool performUseAction(Vector2 tileLocation)
        {
            if (this.Crop != null && !this.FlowerGrassConfig.UseScythe)
            {
                Harvest(tileLocation);
            }

            return false;
        }

        public override bool performToolAction(Tool tool, int damage, Vector2 tileLocation)
        {
            if (this.Crop != null && this.FlowerGrassConfig.UseScythe && tool != null && tool.isScythe())
            {
                Harvest(tileLocation);
            }
            return base.performToolAction(tool, damage, tileLocation);
        }





    }
}
