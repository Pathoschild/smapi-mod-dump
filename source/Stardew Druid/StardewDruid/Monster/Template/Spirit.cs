/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Data;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace StardewDruid.Monster.Template
{
    public class Spirit : DustSpirit
    {

        public int tickCount;

        public bool loadedout;

        public Texture2D hatsTexture;

        public Rectangle hatSourceRect;

        public Vector2 hatOffset;

        public int hatIndex;

        public bool hatFlip;

        public bool partyHats;

        public Spirit() { }

        public Spirit(Vector2 position, int combatModifier, bool champion = false)
            : base(position * 64, true)
        {

            focusedOnFarmers = true;

            Health = combatModifier * 15;

            MaxHealth = Health;

            DamageToFarmer = Math.Min(5, Math.Max(15, combatModifier));

            // ---------------------------------

            Slipperiness = 3;

            HideShadow = false;

            jitteriness.Value = 0.0;

            objectsToDrop.Clear();

            if (Game1.random.Next(3) == 0)
            {
                objectsToDrop.Add("382"); // coal

            }
            else if (Game1.random.Next(4) == 0 && combatModifier >= 120)
            {
                objectsToDrop.Add("395"); // coffee (edible)

            }
            else if (Game1.random.Next(5) == 0 && combatModifier >= 240)
            {
                objectsToDrop.Add("251"); // tea sapling
            }

            if (champion)
            {
                isHardModeMonster.Set(true);

            }

        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {

            DialogueData.DisplayText(this,3);

            return base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);

        }

        public void LoadOut()
        {

            hatsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hats");

            List<int> hatList = new()
            {
                103,
                104,
                //201,
                //202,
                //203,
            };

            hatIndex = hatList[Game1.random.Next(hatList.Count)];

            hatSourceRect = Game1.getSourceRectForStandardTileSheet(hatsTexture, hatIndex, 20, 20);

            partyHats = Mod.instance.Config.partyHats;

            loadedout = true;

        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

           // ----------------- hats

            if (!IsInvisible && Utility.isOnScreen(Position, 128) && partyHats)
            {

                Vector2 localPosition = getLocalPosition(Game1.viewport) + new Vector2(32 + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), 64 + yJumpOffset);

                float depth = Math.Max(0f, drawOnTop ? 0.992f : Tile.Y * 2 / 10000f + 0.00005f);

                b.Draw(
                    hatsTexture,
                    localPosition,
                    hatSourceRect,
                    //Color.White * 0.65f,
                    Color.Blue * 0.75f,
                    0f,
                    //new Vector2(9f, 13f),
                    new Vector2(9f, 11f),
                    3f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    depth
                 );

            }

        }

        public override void update(GameTime time, GameLocation location)
        {
            if (!loadedout) { LoadOut(); }
            base.update(time, location);
        }

        public override void onDealContactDamage(Farmer who)
        {

            if ((who.health + who.buffs.Defense) - DamageToFarmer < 10)
            {

                who.health = (DamageToFarmer - who.buffs.Defense) + 10;

                Mod.instance.CriticalCondition();

            }

        }

    }

}
