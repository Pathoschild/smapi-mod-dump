using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace BetterMeteors
{
    public class CustomResourceClump : ResourceClump
    {
        /*
        public readonly int width;
        public readonly int height;
        public readonly int parentSheetIndex;
        public readonly float health;
        public readonly Vector2 tile;
        public const int stumpIndex = 600;
        public const int hollowLogIndex = 602;
        public const int meteoriteIndex = 622;
        public const int boulderIndex = 672;
        public const int mineRock1Index = 752;
        public const int mineRock2Index = 754;
        public const int mineRock3Index = 756;
        public const int mineRock4Index = 758;
        protected float shakeTimer;*/
        public CustomResourceClump()
        {
            
        }
        public CustomResourceClump(ResourceClump rc) : base(rc.parentSheetIndex.Value, rc.width.Value, rc.height.Value,
            rc.currentTileLocation)
        {
            parentSheetIndex.Value = rc.parentSheetIndex.Value;
            width.Value = rc.width.Value;
            height.Value = rc.height.Value;
            currentTileLocation = rc.currentTileLocation;
            health.Value = rc.health.Value;

        }

        public override bool performToolAction(Tool t, int damage, Vector2 tileLocation, GameLocation location)
        {
            /*
            if (parentSheetIndex.Value == 622 || parentSheetIndex.Value == 600)
            {
                HUDMessage hmsg = new HUDMessage($"Hijacked The Meteor or Stump");
                Game1.addHUDMessage(hmsg);
            }
            return false;*/
            if (t == null)
                return false;
            int debrisType = 12;
            switch ((int)(this.parentSheetIndex.Value))
            {
                case 600:
                    if (t is Axe && (int)(t.UpgradeLevel) < 1)
                    {
                        location.playSound("axe", NetAudio.SoundContext.Default);
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceClump.cs.13945"));
                        Game1.player.jitterStrength = 1f;
                        return false;
                    }
                    if (!(t is Axe))
                        return false;
                    location.playSound("axchop", NetAudio.SoundContext.Default);
                    break;
                case 602:
                    if (t is Axe && (int)(t.UpgradeLevel) < 2)
                    {
                        location.playSound("axe", NetAudio.SoundContext.Default);
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceClump.cs.13948"));
                        Game1.player.jitterStrength = 1f;
                        return false;
                    }
                    if (!(t is Axe))
                        return false;
                    location.playSound("axchop", NetAudio.SoundContext.Default);
                    break;
                case 622:
                    if (t is Pickaxe && (int)(t.UpgradeLevel) < 3)
                    {
                        location.playSound("clubhit", NetAudio.SoundContext.Default);
                        location.playSound("clank", NetAudio.SoundContext.Default);
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceClump.cs.13952"));
                        Game1.player.jitterStrength = 1f;
                        return false;
                    }
                    if (!(t is Pickaxe))
                        return false;
                    location.playSound("hammer", NetAudio.SoundContext.Default);
                    debrisType = 14;
                    break;
                case 672:
                    if (t is Pickaxe && (int)(t.UpgradeLevel) < 2)
                    {
                        location.playSound("clubhit", NetAudio.SoundContext.Default);
                        location.playSound("clank", NetAudio.SoundContext.Default);
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceClump.cs.13956"));
                        Game1.player.jitterStrength = 1f;
                        return false;
                    }
                    if (!(t is Pickaxe))
                        return false;
                    location.playSound("hammer", NetAudio.SoundContext.Default);
                    debrisType = 14;
                    break;
                case 752:
                case 754:
                case 756:
                case 758:
                    if (!(t is Pickaxe))
                        return false;
                    location.playSound("hammer", NetAudio.SoundContext.Default);
                    debrisType = 14;
                    this.shakeTimer = 500f;
                    this.NeedsUpdate = true;
                    break;
            }
            this.health.Value -= Math.Max(1f, (float)((int)(t.UpgradeLevel) + 1) * 0.75f);
            Game1.createRadialDebris(Game1.currentLocation, debrisType, (int)tileLocation.X + Game1.random.Next((int)(this.width.Value) / 2 + 1), (int)tileLocation.Y + Game1.random.Next((int)(this.height.Value) / 2 + 1), Game1.random.Next(4, 9), false, -1, false, -1);
            if ((double)(float)(this.health.Value) <= 0.0)
            {
                if (t != null && t.getLastFarmerToUse().hasMagnifyingGlass && Game1.random.NextDouble() < 0.05)
                {
                    StardewValley.Object unseenSecretNote = location.tryToCreateUnseenSecretNote(t.getLastFarmerToUse());
                    if (unseenSecretNote != null)
                        Game1.createItemDebris((Item)unseenSecretNote, tileLocation * 64f, -1, location, -1);
                }
                if (Game1.IsMultiplayer)
                {
                    Random multiplayerRandom = Game1.recentMultiplayerRandom;
                }
                else
                {
                    Random random1 = new Random((int)((double)Game1.uniqueIDForThisGame + (double)tileLocation.X * 7.0 + (double)tileLocation.Y * 11.0 + (double)Game1.stats.DaysPlayed + (double)(float)(this.health.Value)));
                }
                Random random2;

                if (parentSheetIndex.Value == 622 || parentSheetIndex.Value == 600)
                {
                    HUDMessage hmsg = new HUDMessage($"Hijacked The Meteor or Stump");
                    Game1.addHUDMessage(hmsg);
                    return true;
                }
                /*
                switch ((int)((NetFieldBase<int, NetInt>)this.parentSheetIndex))
                {
                    case 600:
                    case 602:
                        if (t.getLastFarmerToUse() == Game1.player)
                            ++Game1.stats.StumpsChopped;
                        t.getLastFarmerToUse().gainExperience(2, 25);
                        int number1 = (int)((NetFieldBase<int, NetInt>)this.parentSheetIndex) == 602 ? 8 : 2;
                        Random random3;
                        if (Game1.IsMultiplayer)
                        {
                            Game1.recentMultiplayerRandom = new Random((int)tileLocation.X * 1000 + (int)tileLocation.Y);
                            random3 = Game1.recentMultiplayerRandom;
                        }
                        else
                            random3 = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + (int)tileLocation.X * 7 + (int)tileLocation.Y * 11);
                        if (t.getLastFarmerToUse().professions.Contains(12))
                        {
                            if (number1 == 8)
                                number1 = 10;
                            else if (random3.NextDouble() < 0.5)
                                ++number1;
                        }
                        if (Game1.IsMultiplayer)
                            Game1.createMultipleObjectDebris(709, (int)tileLocation.X, (int)tileLocation.Y, number1, t.getLastFarmerToUse().UniqueMultiplayerID);
                        else
                            Game1.createMultipleObjectDebris(709, (int)tileLocation.X, (int)tileLocation.Y, number1);
                        location.playSound("stumpCrack", NetAudio.SoundContext.Default);
                        Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(23, tileLocation * 64f, Color.White, 4, false, 140f, 0, 128, -1f, 128, 0));
                        Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(385, 1522, (int)sbyte.MaxValue, 79), 2000f, 1, 1, tileLocation * 64f + new Vector2(0.0f, 49f), false, false, 1E-05f, 0.016f, Color.White, 1f, 0.0f, 0.0f, 0.0f, false));
                        Game1.createRadialDebris(Game1.currentLocation, 34, (int)tileLocation.X, (int)tileLocation.Y, Game1.random.Next(4, 9), false, -1, false, -1);
                        return true;
                    case 622:
                        int number2 = 6;
                        if (Game1.IsMultiplayer)
                        {
                            Game1.recentMultiplayerRandom = new Random((int)tileLocation.X * 1000 + (int)tileLocation.Y);
                            random2 = Game1.recentMultiplayerRandom;
                        }
                        else
                            random2 = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + (int)tileLocation.X * 7 + (int)tileLocation.Y * 11);
                        if (Game1.IsMultiplayer)
                        {
                            Game1.createMultipleObjectDebris(386, (int)tileLocation.X, (int)tileLocation.Y, number2, t.getLastFarmerToUse().UniqueMultiplayerID);
                            Game1.createMultipleObjectDebris(390, (int)tileLocation.X, (int)tileLocation.Y, number2, t.getLastFarmerToUse().UniqueMultiplayerID);
                            Game1.createMultipleObjectDebris(535, (int)tileLocation.X, (int)tileLocation.Y, 2, t.getLastFarmerToUse().UniqueMultiplayerID);
                        }
                        else
                        {
                            Game1.createMultipleObjectDebris(386, (int)tileLocation.X, (int)tileLocation.Y, number2);
                            Game1.createMultipleObjectDebris(390, (int)tileLocation.X, (int)tileLocation.Y, number2);
                            Game1.createMultipleObjectDebris(535, (int)tileLocation.X, (int)tileLocation.Y, 2);
                        }
                        location.playSound("boulderBreak", NetAudio.SoundContext.Default);
                        Game1.createRadialDebris(Game1.currentLocation, 32, (int)tileLocation.X, (int)tileLocation.Y, Game1.random.Next(6, 12), false, -1, false, -1);
                        Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, tileLocation * 64f, Color.White, 8, false, 100f, 0, -1, -1f, -1, 0));
                        Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(1f, 0.0f)) * 64f, Color.White, 8, false, 110f, 0, -1, -1f, -1, 0));
                        Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(1f, 1f)) * 64f, Color.White, 8, true, 80f, 0, -1, -1f, -1, 0));
                        Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(0.0f, 1f)) * 64f, Color.White, 8, false, 90f, 0, -1, -1f, -1, 0));
                        Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, tileLocation * 64f + new Vector2(32f, 32f), Color.White, 8, false, 70f, 0, -1, -1f, -1, 0));
                        return true;
                    case 672:
                    case 752:
                    case 754:
                    case 756:
                    case 758:
                        int num = (int)((NetFieldBase<int, NetInt>)this.parentSheetIndex) == 672 ? 15 : 10;
                        if (Game1.IsMultiplayer)
                        {
                            Game1.recentMultiplayerRandom = new Random((int)tileLocation.X * 1000 + (int)tileLocation.Y);
                            random2 = Game1.recentMultiplayerRandom;
                        }
                        else
                            random2 = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + (int)tileLocation.X * 7 + (int)tileLocation.Y * 11);
                        if (Game1.IsMultiplayer)
                            Game1.createMultipleObjectDebris(390, (int)tileLocation.X, (int)tileLocation.Y, num, t.getLastFarmerToUse().UniqueMultiplayerID);
                        else
                            Game1.createRadialDebris(Game1.currentLocation, 390, (int)tileLocation.X, (int)tileLocation.Y, num, false, -1, true, -1);
                        location.playSound("boulderBreak", NetAudio.SoundContext.Default);
                        Game1.createRadialDebris(Game1.currentLocation, 32, (int)tileLocation.X, (int)tileLocation.Y, Game1.random.Next(6, 12), false, -1, false, -1);
                        Color color = Color.White;
                        switch ((int)((NetFieldBase<int, NetInt>)this.parentSheetIndex))
                        {
                            case 752:
                                color = new Color(188, 119, 98);
                                break;
                            case 754:
                                color = new Color(168, 120, 95);
                                break;
                            case 756:
                            case 758:
                                color = new Color(67, 189, 238);
                                break;
                        }
                        Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(48, tileLocation * 64f, color, 5, false, 180f, 0, 128, -1f, 128, 0)
                        {
                            alphaFade = 0.01f
                        });
                        return true;
                }*/


            }
            else
            {
                this.shakeTimer = 100f;
                this.NeedsUpdate = true;
            }
            return false;
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            Rectangle standardTileSheet = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int)parentSheetIndex.Value, 16, 16);
            standardTileSheet.Width = (int)width.Value * 16;
            standardTileSheet.Height = (int)height.Value * 16;
            Vector2 globalPosition = this.tile.Value * 64f;
            if ((double)this.shakeTimer > 0.0)
                globalPosition.X += (float)Math.Sin(2.0 * Math.PI / (double)this.shakeTimer) * 4f;
            spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Rectangle?(standardTileSheet), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(((double)this.tile.Y + 1.0) * 64.0 / 10000.0 + (double)this.tile.X / 100000.0));
        }
    }
}
