/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using PyTK.CustomElementHandler;

namespace FishingTrawler.Objects.Rewards
{
    public enum FlagType
    {
        Unknown,
        Parley,
        JollyRoger,
        GamblersCrest,
        MermaidsBlessing,
        PatronSaint,
        SharksFin,
        Worldly,
        SlimeKing,
        KingCrab
    }

    public class AncientFlag : Furniture, ISaveElement, ICustomObject
    {
        internal static Texture2D flagTexture;

        public FlagType FlagType
        {
            get
            {
                if (this.modData.ContainsKey(ModEntry.ANCIENT_FLAG_KEY) && Enum.TryParse(this.modData[ModEntry.ANCIENT_FLAG_KEY], out FlagType flagType))
                {
                    return flagType;
                }

                return FlagType.Unknown;
            }
        }

        // Using Pirate Flag id (1900) as base vanilla object
        public AncientFlag()
        {

        }

        public AncientFlag(FlagType flagType) : this(flagType, Vector2.Zero)
        {

        }

        public AncientFlag(FlagType flagType, Vector2 tile) : base(1900, tile)
        {
            flagTexture = ModResources.ancientFlagsTexture;
            base.defaultSourceRect.Value = new Rectangle(32 * (int)flagType % flagTexture.Width, 32 * (int)flagType / flagTexture.Width * 32, 32, 32);
            base.modData.Add(ModEntry.ANCIENT_FLAG_KEY, flagType.ToString());
        }

        public object getReplacement()
        {
            return new Furniture(1900, base.TileLocation) { modData = this.modData };
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            //saveData.Add("something", "myValue");
            return new Dictionary<string, string>(); ;
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            // Unused
        }

        public ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            Furniture baseFlag = (replacement as Furniture);
            flagTexture = ModResources.ancientFlagsTexture;
            base.defaultSourceRect.Value = new Rectangle(32 * (int)this.FlagType % flagTexture.Width, 32 * (int)this.FlagType / flagTexture.Width * 32, 32, 32);

            if (baseFlag.modData.ContainsKey(ModEntry.ANCIENT_FLAG_KEY) && Enum.TryParse(baseFlag.modData[ModEntry.ANCIENT_FLAG_KEY], out FlagType flagType))
            {
                return new AncientFlag(flagType, baseFlag.TileLocation);
            }
            return new AncientFlag(FlagType.Unknown, baseFlag.TileLocation);
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (base.isTemporarilyInvisible)
            {
                return;
            }

            if (Furniture.isDrawingLocationFurniture)
            {
                spriteBatch.Draw(flagTexture, Game1.GlobalToLocal(Game1.viewport, this.drawPosition + ((base.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero)), this.defaultSourceRect, Color.White * alpha, 0f, Vector2.Zero, 4f, base.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, ((int)this.furniture_type == 12) ? (2E-09f + base.tileLocation.Y / 100000f) : ((float)(base.boundingBox.Value.Bottom - (((int)this.furniture_type == 6 || (int)this.furniture_type == 17 || (int)this.furniture_type == 13) ? 48 : 8)) / 10000f));
            }
            else
            {
                spriteBatch.Draw(flagTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - (this.sourceRect.Height * 4 - base.boundingBox.Height) + ((base.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0))), this.defaultSourceRect, Color.White * alpha, 0f, Vector2.Zero, 4f, base.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, ((int)this.furniture_type == 12) ? (2E-09f + base.tileLocation.Y / 100000f) : ((float)(base.boundingBox.Value.Bottom - (((int)this.furniture_type == 6 || (int)this.furniture_type == 17 || (int)this.furniture_type == 13) ? 48 : 8)) / 10000f));
            }
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            spriteBatch.Draw(flagTexture, location + new Vector2(32f, 32f), this.defaultSourceRect, color * transparency, 0f, new Vector2(this.defaultSourceRect.Width / 2, this.defaultSourceRect.Height / 2), 2f * scaleSize, SpriteEffects.None, layerDepth);
            if (((drawStackNumber == StackDrawType.Draw && this.maximumStackSize() > 1 && this.Stack > 1) || drawStackNumber == StackDrawType.Draw_OneInclusive) && (double)scaleSize > 0.3 && this.Stack != int.MaxValue)
            {
                Utility.drawTinyDigits(base.stack, spriteBatch, location + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString(base.stack, 3f * scaleSize)) + 3f * scaleSize, 64f - 18f * scaleSize + 2f), 3f * scaleSize, 1f, color);
            }
        }

        public override void drawPlacementBounds(SpriteBatch spriteBatch, GameLocation location)
        {
            if (location is Beach)
            {
                // Draw nothing to avoid covering up Murphy when attempting to give him an ancient flag
                return;
            }

            base.drawPlacementBounds(spriteBatch, location);
        }

        protected override string loadDisplayName()
        {
            return GetFlagName(this.FlagType);
        }

        public override string getDescription()
        {
            return Game1.parseText(GetFlagDescription(this.FlagType), Game1.smallFont, this.getDescriptionWidth());
            //return Game1.parseText(GetFlagDescription((FlagType)base.SpecialVariable), Game1.smallFont, this.getDescriptionWidth());
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            if (this.FlagType is FlagType.Unknown)
            {
                Game1.showRedMessage(ModEntry.i18n.Get("game_message.identify_flag_first"));
                return false;
            }

            return base.placementAction(location, x, y, who);
        }

        internal static string GetFlagName(FlagType flagType)
        {
            switch (flagType)
            {
                case FlagType.Parley:
                    return ModEntry.i18n.Get("item.ancient_flag.parely.name");
                case FlagType.JollyRoger:
                    return ModEntry.i18n.Get("item.ancient_flag.jolly_roger.name");
                case FlagType.GamblersCrest:
                    return ModEntry.i18n.Get("item.ancient_flag.gamblers_crest.name");
                case FlagType.MermaidsBlessing:
                    return ModEntry.i18n.Get("item.ancient_flag.mermaids_blessing.name");
                case FlagType.PatronSaint:
                    return ModEntry.i18n.Get("item.ancient_flag.patron_saint.name");
                case FlagType.SharksFin:
                    return ModEntry.i18n.Get("item.ancient_flag.sharks_fin.name");
                case FlagType.Worldly:
                    return ModEntry.i18n.Get("item.ancient_flag.worldly_flag.name");
                case FlagType.SlimeKing:
                    return ModEntry.i18n.Get("item.ancient_flag.slime_king.name");
                case FlagType.KingCrab:
                    return ModEntry.i18n.Get("item.ancient_flag.king_crab.name");
                default:
                    return ModEntry.i18n.Get("item.ancient_flag.unknown.name");
            }
        }

        private string GetFlagDescription(FlagType flagType)
        {
            switch (flagType)
            {
                case FlagType.Parley:
                    return ModEntry.i18n.Get("item.ancient_flag.parely.description");
                case FlagType.JollyRoger:
                    return ModEntry.i18n.Get("item.ancient_flag.jolly_roger.description");
                case FlagType.GamblersCrest:
                    return ModEntry.i18n.Get("item.ancient_flag.gamblers_crest.description");
                case FlagType.MermaidsBlessing:
                    return ModEntry.i18n.Get("item.ancient_flag.mermaids_blessing.description");
                case FlagType.PatronSaint:
                    return ModEntry.i18n.Get("item.ancient_flag.patron_saint.description");
                case FlagType.SharksFin:
                    return ModEntry.i18n.Get("item.ancient_flag.sharks_fin.description");
                case FlagType.Worldly:
                    return ModEntry.i18n.Get("item.ancient_flag.worldly_flag.description");
                case FlagType.SlimeKing:
                    return ModEntry.i18n.Get("item.ancient_flag.slime_king.description");
                case FlagType.KingCrab:
                    return ModEntry.i18n.Get("item.ancient_flag.king_crab.description");
                default:
                    return ModEntry.i18n.Get("item.ancient_flag.unknown.description");
            }
        }
    }
}
