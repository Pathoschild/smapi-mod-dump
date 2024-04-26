/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/arannya/BoomerangMod
**
*************************************************/

using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Weapons;
using StardewValley.Menus;
using StardewValley.Tools;

namespace Boomerang
{
    internal class ModEntry : StardewModdingAPI.Mod
    {
        public static ModEntry Instance;
        public const String itemID_c = "Arannya.Weapons.Boomerangs.WoodenBoomerang";
        private ThrownBoomerang Thrown;
        
        public override void Entry(IModHelper helper)
        {
            ModEntry.Instance = this;
            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(MeleeWeapon), nameof(MeleeWeapon.getCategoryName)),
                prefix: new HarmonyMethod(typeof(Boomerang.Patches), nameof(Boomerang.Patches.getCategoryName_Prefix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(MeleeWeapon), nameof(MeleeWeapon.drawDuringUse),
                    new Type[]{ typeof(int), typeof(int), typeof(SpriteBatch), typeof(Vector2), 
                        typeof(Farmer), typeof(string), typeof(int), typeof(bool)}),
                transpiler: new HarmonyMethod(typeof(Boomerang.Patches), nameof(Boomerang.Patches.Transpiler)));
            
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.Input.ButtonPressed += this.OnButtonPress;
            helper.Events.GameLoop.UpdateTicking += this.OnUpdateTicking;
            helper.Events.Player.Warped += this.OnWarped;
        }

        public void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu shop)
            {
                if (shop.ShopId != Game1.shop_adventurersGuild) return;
                int index = 0;
                for (; index < shop.forSale.Count; index++)
                {
                    var item = shop.forSale[index];
                    if (item is not MeleeWeapon)
                        break;
                }
                var boomerangForSale = ItemRegistry.Create(itemID_c);
                shop.forSale.Insert(index, boomerangForSale);
                shop.itemPriceAndStock.Add(boomerangForSale,
                    new ItemStockInformation(500, 1)); // sale price and available stock
            }
        }

        private void OnButtonPress(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;
            
            if (e.Button == SButton.MouseRight &&
                Game1.player.CurrentTool is not null &&
                Game1.player.CurrentTool is MeleeWeapon &&
                Game1.player.CurrentTool.itemId.Value.Equals(itemID_c) &&
                this.Thrown is null)
            {
                if ((MeleeWeapon.defenseCooldown > 0))
                    return;
                
                this.Thrown = new ThrownBoomerang(Game1.player, e.Cursor.AbsolutePixels);
                Game1.playSound("throw");
                Game1.currentLocation.projectiles.Add(this.Thrown);
            }

            if (e.Button == SButton.MouseLeft &&
                Game1.player.CurrentTool is not null &&
                Game1.player.CurrentTool is MeleeWeapon &&
                Game1.player.CurrentTool.itemId.Value.Equals(itemID_c))
            {
                if (this.Thrown is not null)
                    Helper.Input.Suppress(SButton.MouseLeft);
            }
        }

        private void OnUpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            if (this.Thrown is not null)
            {
                if ((this.Thrown.GetPosition() - this.Thrown.Target.Value).Length() < 1)
                {
                    var playerPos = Game1.player.getStandingPosition();
                    playerPos.X -= 16;
                    playerPos.Y -= 64;
                    this.Thrown.Target.Value = playerPos;
                    if ((this.Thrown.GetPosition() - playerPos).Length() < 16)
                    {
                        this.Thrown.Destroyed = true;
                    }
                }

                if (this.Thrown.Destroyed)
                {
                    this.Thrown = null;
                }
            }
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (this.Thrown is not null)
            {
                this.Thrown.Destroyed = true;
                this.Thrown = null;
            }
        }
    }
}