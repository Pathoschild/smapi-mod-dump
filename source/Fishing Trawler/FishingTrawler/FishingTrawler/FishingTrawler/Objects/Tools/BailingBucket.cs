/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.GameLocations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using PyTK.CustomElementHandler;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingTrawler.Objects.Tools
{
    internal class BailingBucket : MilkPail, ISaveElement
    {
        private string _displayName = ModEntry.i18n.Get("item.bailing_bucket.name");
        private readonly NetEvent0 _finishEvent = new NetEvent0();

        private bool _containsWater = false;
        private float _bucketScale = 0f;

        public BailingBucket() : base()
        {
            this.modData.Add(ModEntry.BAILING_BUCKET_KEY, "true");
            this.description = ModEntry.i18n.Get("item.bailing_bucket.description_empty");
        }

        public object getReplacement()
        {
            return new MilkPail();
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            return new Dictionary<string, string>();
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            this.modData = (replacement as MilkPail).modData;
        }

        public override Item getOne()
        {
            BailingBucket bucket = new BailingBucket();
            bucket._GetOneFrom(this);
            return bucket;
        }

        protected override string loadDisplayName()
        {
            return _displayName;
        }

        public override bool canBeTrashed()
        {
            if (ModEntry.IsPlayerOnTrawler())
            {
                return false;
            }

            return true;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            base.IndexOfMenuItemView = 0;

            int spriteOffset = this._containsWater ? 16 : 0;
            spriteBatch.Draw(ModResources.bucketTexture, location + new Vector2(32f, 32f), new Rectangle(spriteOffset, 0, 16, 16), color * transparency, 0f, new Vector2(8f, 8f), 4f * (scaleSize + this._bucketScale), SpriteEffects.None, layerDepth);
        }

        protected override void initNetFields()
        {
            base.initNetFields();
            base.NetFields.AddFields(this._finishEvent);
            this._finishEvent.onEvent += doFinish;
        }

        public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
        {
            if (!ModEntry.IsPlayerOnTrawler() || who is null || (who != null && !Game1.player.Equals(who)))
            {
                who.forceCanMove();
                return false;
            }

            if (location is TrawlerHull trawlerHull)
            {
                if (this._containsWater)
                {
                    Game1.addHUDMessage(new HUDMessage(ModEntry.i18n.Get("game_message.bailing_bucket.empty_into_sea"), 3));
                }
                else if (trawlerHull.IsFlooding())
                {
                    this._containsWater = true;
                    this._bucketScale = 0.5f;
                    this.description = ModEntry.i18n.Get("item.bailing_bucket.description_full");

                    trawlerHull.ChangeWaterLevel(-5);
                    trawlerHull.localSound("slosh");
                    ModEntry.SyncTrawler(Messages.SyncType.WaterLevel, trawlerHull.GetWaterLevel(), ModEntry.GetFarmersOnTrawler());
                }
                else
                {
                    Game1.addHUDMessage(new HUDMessage(ModEntry.i18n.Get("game_message.bailing_bucket.no_water_to_bail"), 3));
                }
            }
            else if (location is TrawlerSurface trawlerSurface && this._containsWater)
            {
                if (trawlerSurface.IsPlayerByBoatEdge(who))
                {
                    this._containsWater = false;
                    this._bucketScale = 0.5f;
                    this.description = ModEntry.i18n.Get("item.bailing_bucket.description_empty");

                    who.currentLocation.localSound("waterSlosh");
                }
                else
                {
                    Game1.addHUDMessage(new HUDMessage(ModEntry.i18n.Get("game_message.bailing_bucket.stand_closer_to_edge"), 3));
                }
            }
            else
            {
                Game1.addHUDMessage(new HUDMessage(ModEntry.i18n.Get("game_message.bailing_bucket.bail_from_hull"), 3));
            }

            who.forceCanMove();
            return true;
        }

        public override void tickUpdate(GameTime time, Farmer who)
        {
            if (this._bucketScale > 0f)
            {
                this._bucketScale -= 0.01f;
            }

            base.lastUser = who;
            base.tickUpdate(time, who);
            this._finishEvent.Poll();
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
        {
            base.DoFunction(location, x, y, power, who);
            who.Stamina -= 4f;
            base.CurrentParentTileIndex = 0;
            base.IndexOfMenuItemView = 0;

            this.finish();
        }

        private void finish()
        {
            this._finishEvent.Fire();
        }

        private void doFinish()
        {
            base.lastUser.CanMove = true;
            base.lastUser.completelyStopAnimatingOrDoingAction();
            base.lastUser.UsingTool = false;
            base.lastUser.canReleaseTool = true;
        }
    }
}
