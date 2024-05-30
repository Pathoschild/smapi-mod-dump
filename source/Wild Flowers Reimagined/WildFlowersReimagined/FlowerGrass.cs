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
using System.Xml.Serialization;
using StardewValley.Tools;
using System;

namespace WildFlowersReimagined
{
    [XmlType("Mods_jppWildFlowersReimagined_FlowerGrass")]
    public class FlowerGrass : Grass
    {
        [XmlIgnore]
        private readonly HoeDirt fakeDirt = new();

        public readonly NetRef<Crop> netCrop = new(new());

        [XmlIgnore]
        private List<Action<GameLocation, Vector2>> queuedActions = new();

        [XmlIgnore]
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

        [XmlIgnore]
        public override GameLocation Location
        {
            get
            {
                return base.Location;
            }
            set
            {
                base.Location = value;
                if (netCrop.Value != null)
                {
                    netCrop.Value.currentLocation = value;
                }
            }
        }


        [XmlElement("tile")]
        public readonly NetVector2 netTile = new NetVector2();

        public override Vector2 Tile
        {
            get
            {
                return this.netTile.Value;
            }
            set
            {
                this.netTile.Value = value;
                if (netCrop.Value != null)
                {
                    netCrop.Value.tilePosition = value;
                }
            }
        }

        public FlowerGrassConfig FlowerGrassConfig { get; set; }

        public override void initNetFields()
        {
            base.initNetFields();
            base.NetFields.AddField(netCrop, "netCrop").AddField(netTile, "netTile");
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

            netCrop.fieldChangeVisibleEvent += delegate
            {
                if (netCrop.Value != null)
                {
                    netCrop.Value.Dirt = fakeDirt;
                    netCrop.Value.currentLocation = Location;
                    netCrop.Value.updateDrawMath(Tile);
                }
            };

        }

        public FlowerGrass() : base()
        {
            Location = Game1.currentLocation;
            FlowerGrassConfig = ModEntry.ConfigLoadedFlowerConfig();
        }

        public FlowerGrass(int which, int numberOfWeeds, Crop crop, FlowerGrassConfig flowerGrassConfig) : this()
        {
            //Location = Game1.currentLocation;
            grassType.Value = (byte)which;
            this.numberOfWeeds.Value = numberOfWeeds;
            this.Crop = crop;
            loadSprite();
            this.FlowerGrassConfig = flowerGrassConfig;

        }

        public override void loadSprite()
        {
            base.loadSprite();
            Crop?.updateDrawMath(Tile);
        }

        public override void performPlayerEntryAction()
        {
            base.performPlayerEntryAction();
            Crop?.updateDrawMath(Tile);
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

        public override void dayUpdate()
        {
            base.dayUpdate();
            if (this.Crop != null)
            {
                Crop.newDay(0);
            }
            
            // kill at the end of the season???
            //GameLocation location = Location;
            //if ((bool)location.isOutdoors && location.GetSeason() == Season.Winter && crop != null && !crop.isWildSeedCrop() && !crop.IsInSeason(location))
            //{
            //    destroyCrop(showAnimation: false);
            //}

        }

        /// <summary>
        /// Method for harvesting the flowers, 
        /// </summary>
        /// <param name="tileLocation"> tile location </param>
        /// <param name="useScythe">if it was scythe or hand. The game code has different behaviors for this</param>
        private void Harvest(Vector2 tileLocation, bool useScythe)
        {
            var successful = this.Crop.harvest((int)tileLocation.X, (int)tileLocation.Y, fakeDirt, isForcedScytheHarvest: useScythe);
            if (successful)
            {
                this.Crop = null;
            }
            else
            {
                // handle special case of re-growable flowers
                var cropData = this.Crop.GetData();
                if (this.Crop.dayOfCurrentPhase.Value > 0 && cropData.RegrowDays >= 0)
                {
                    // this means we are on a crop with regrow
                    // If the player doesn't want to keep the crop delete it, as the change of dayOfCurrentPhase means it was added to
                    // the inventory
                    if (!this.FlowerGrassConfig.KeepRegrowFlower)
                    {
                        this.Crop = null;
                    }

                }
            }
        }

        public override bool performUseAction(Vector2 tileLocation)
        {
            if (this.Crop != null && !this.FlowerGrassConfig.UseScythe)
            {
                Harvest(tileLocation, false);
            }

            return false;
        }

        public override bool performToolAction(Tool tool, int damage, Vector2 tileLocation)
        {
            if (this.Crop != null && this.FlowerGrassConfig.UseScythe && tool != null && tool.isScythe())
            {
                Harvest(tileLocation, true);
            }
            return base.performToolAction(tool, damage, tileLocation);
        }

        public string ToDebugString()
        {
            var flowerStr = "None";
            if (this.Crop != null)
            {
                flowerStr = $"{this.Crop.DrawnCropTexture} ::: {this.Crop.forageCrop.Value} ::: {this.Crop.flip.Value} ::: {this.Crop.sourceRect}";
            }

            var location = "?";
            if (this.Location != null)
            {
                location = this.Location.NameOrUniqueName;
            }
            
            return $"{this.Tile}@{location} :: {this.numberOfWeeds} :: {this.texture.Value} :: {flowerStr}";
        }





    }
}
