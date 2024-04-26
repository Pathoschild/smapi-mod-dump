/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using MoonShared;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using AtraBase.Toolkit.Extensions;
using AtraBase.Toolkit.Reflection;
using AtraCore.Framework.Caches;
using AtraCore.Framework.ReflectionManager;
using AtraCore.Utilities;
using AtraShared.Utils.Extensions;
using GrowableGiantCrops.Framework.Assets;
using GrowableGiantCrops.Framework.InventoryModels;
using GrowableGiantCrops.HarmonyPatches.Compat;
using GrowableGiantCrops.HarmonyPatches.GrassPatches;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System.Collections.Generic;
using GrowableGiantCrops.Framework;

namespace ShovelToolUpgrades
{
    [XmlType("Mods_moonslime_upgradeableshovel")] // SpaceCore serialisation signature
    public class UpgradeableShovel : GrowableGiantCrops.Framework.ShovelTool
    {
        public new string Name = "Shovel";

        public UpgradeableShovel() : base()
        {
            base.UpgradeLevel = 0;
        }

        public UpgradeableShovel(int upgradeLevel) : base()
        {
            base.UpgradeLevel = upgradeLevel;
            base.InitialParentTileIndex = 0;
            base.IndexOfMenuItemView = -1;
        }

        public override Item getOne()
        {
            UpgradeableShovel shovel = new UpgradeableShovel();
            shovel.UpgradeLevel = base.UpgradeLevel;
            CopyEnchantments(this, shovel);
            shovel._GetOneFrom(this);
            return shovel;
        }

        protected override string loadDisplayName()
        {
            return ModEntry.Instance.I18n.Get("tool.shovel.name").ToString();
        }

        protected override string loadDescription()
        {
            return ModEntry.Instance.I18n.Get("tool.Shovel.description."+this.UpgradeLevel.ToString());
        }

        public static bool CanBeUpgraded()
        {

            Tool shovel = Game1.player.getToolFromName("Shovel");
            int MaxUpgradeLevel = 4;
            if (ModEntry.MoonLoaded == true && ModEntry.MargoLoaded == false)
            {
                MaxUpgradeLevel = 6;
            }
            if (ModEntry.MoonLoaded == true && ModEntry.MargoLoaded == true)
            {
                MaxUpgradeLevel = 6;
            }
            if (ModEntry.MoonLoaded == false && ModEntry.MargoLoaded == true)
            {
                MaxUpgradeLevel = 5;
            }

            return shovel is not null && shovel.UpgradeLevel != MaxUpgradeLevel;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            spriteBatch.Draw(
                texture: GetTexture(),
                position: location + new Vector2(32f, 32f),
                sourceRectangle: IconSourceRectangle(this.UpgradeLevel),
                color: color * transparency,
                rotation: 0f,
                origin: new Vector2(8, 8),
                scale: Game1.pixelZoom * scaleSize,
                effects: SpriteEffects.None,
                layerDepth: layerDepth);
        }

        public static Rectangle IconSourceRectangle(int upgradeLevel)
        {
            Rectangle source = new(96, 16, 16, 16);
            source.Y += upgradeLevel * source.Height*2;
            return source;
        }

        public override bool canBeTrashed()
        {
            return true;
        }

        public override Texture2D GetTexture()
        {
            return ModEntry.Assets.ToolSprites;
        }

        public override bool actionWhenPurchased()
        {

            if (this.UpgradeLevel > 0 && Game1.player.toolBeingUpgraded.Value == null)
            {
                Tool t = Game1.player.getToolFromName("Shovel");
                Game1.player.removeItemFromInventory(t);
                if (t is not UpgradeableShovel)
                {
                    t = new UpgradeableShovel(upgradeLevel: 1);
                } else
                {
                    t.UpgradeLevel++;
                }
                Game1.player.toolBeingUpgraded.Value = t;
                Game1.player.daysLeftForToolUpgrade.Value = 2;
                Game1.playSound("parry");
                Game1.exitActiveMenu();
                Game1.drawDialogue(Game1.getCharacterFromName("Clint"), Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14317"));
                return true;
            }
            return base.actionWhenPurchased();
        }

        public static void AddToShopStock(Dictionary<ISalable, int[]> itemPriceAndStock, Farmer who)
        {
            if (who == Game1.player && CanBeUpgraded())
            {
                int quantity = 1;
                Tool tool = who.getToolFromName("Shovel");
                int upgradeLevel = 0;
                if (who.getToolFromName("Shovel") is not UpgradeableShovel)
                {
                    upgradeLevel = 1;
                } else
                {
                    upgradeLevel = tool.UpgradeLevel + 1;
                }
                int upgradePrice = ModEntry.PriceForToolUpgradeLevelClint(upgradeLevel); 
                upgradePrice = (int)(upgradePrice * 1);
                int extraMaterialIndex = ModEntry.IndexOfExtraMaterialForToolUpgradeClint(upgradeLevel);
                itemPriceAndStock.Add(
                    new UpgradeableShovel(upgradeLevel: upgradeLevel),
                    new int[] { upgradePrice, quantity, extraMaterialIndex, 5 });
            }
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
        {
            float staminaUse = (ModEntry.Config.ShovelMaxEnergyUsage - (this.UpgradeLevel * ModEntry.Config.ShovelEnergyDecreasePerLevel));
            who.stamina -= Math.Max(staminaUse, 0);

            power = who.toolPower;

            List<Vector2> list = tilesAffected(new Vector2(x / 64, y / 64), power, who);
            foreach (Vector2 item in list)
            {
                int newX = (int)(item.X * 64);
                int newY = (int)(item.Y * 64);
                base.DoFunction(location, newX, newY, power, who);
            }

        }
    }
}
