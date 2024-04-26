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
using Netcode;
using StardewDruid.Data;
using StardewDruid.Event;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Events;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace StardewDruid.Monster.Boss
{
    public class Gargoyle : StardewDruid.Monster.Boss.Boss
    {

        public int bobHeight;

        public NetString netScheme = new("None");

        public Dictionary<int, List<Rectangle>> schemeFrames;

        Dictionary<string, Color> schemeColors;

        public Gargoyle()
        {
        }

        public Gargoyle(Vector2 vector, int CombatModifier, string name = "Gargoyle", string template = "Pepper Rex")
          : base(vector, CombatModifier, name, template)
        {

        }

        protected override void initNetFields()
        {
            
            base.initNetFields();

            NetFields.AddField(netScheme, "netScheme");

        }

        public override void LoadOut()
        {

            GargoyleWalk();

            GargoyleFlight();

            GargoyleSpecial();

            overHead = new(16, -128);

            loadedOut = true;

        }

        public void SchemeLoad()
        {

            Dictionary<string, string> schemeItems = new()
            {
                ["None"] = "93",
                ["Emerald"] = "60",
                ["Aquamarine"] = "62",
                ["Ruby"] = "64",
                ["Amethyst"] = "66",
                ["Topaz"] = "68",
                ["Solar"] = "768",
                ["Void"] = "769",

            };

            objectsToDrop.Clear();

            objectsToDrop.Add(schemeItems[netScheme.Value]);

        }

        public void GargoyleWalk()
        {

            characterTexture = MonsterHandle.MonsterTexture(realName.Value);

            walkCeiling = 7;

            walkFloor = 0;

            walkInterval = 9;

            gait = 2;

            idleFrames = new Dictionary<int, List<Rectangle>>()
            {
                [0] = new List<Rectangle>()
                {
                    new Rectangle(0, 64, 32, 32),
                    new Rectangle(32, 64, 32, 32),
                    new Rectangle(64, 64, 32, 32),
                    new Rectangle(32, 64, 32, 32),
                    new Rectangle(128, 64, 32, 32),
                    new Rectangle(160, 64, 32, 32),
                    new Rectangle(96, 64, 32, 32),
                    new Rectangle(160, 64, 32, 32),
                },

                [1] = new List<Rectangle>()
                {
                    new Rectangle(0, 32, 32, 32),
                    new Rectangle(32, 32, 32, 32),
                    new Rectangle(64, 32, 32, 32),
                    new Rectangle(32, 32, 32, 32),
                    new Rectangle(128, 32, 32, 32),
                    new Rectangle(160, 32, 32, 32),
                    new Rectangle(96, 32, 32, 32),
                    new Rectangle(160, 32, 32, 32),
                },
                [2] = new List<Rectangle>()
                {
                    new Rectangle(0, 0, 32, 32),
                    new Rectangle(32, 0, 32, 32),
                    new Rectangle(64, 0, 32, 32),
                    new Rectangle(32, 0, 32, 32),
                    new Rectangle(128, 0, 32, 32),
                    new Rectangle(160, 0, 32, 32),
                    new Rectangle(96, 0, 32, 32),
                    new Rectangle(160, 0, 32, 32),
                },
                [3] = new List<Rectangle>()
                {
                    new Rectangle(0, 32, 32, 32),
                    new Rectangle(32, 32, 32, 32),
                    new Rectangle(64, 32, 32, 32),
                    new Rectangle(32, 32, 32, 32),
                    new Rectangle(128, 32, 32, 32),
                    new Rectangle(160, 32, 32, 32),
                    new Rectangle(96, 32, 32, 32),
                    new Rectangle(160, 32, 32, 32),
                }
            };

            walkFrames = idleFrames;

            schemeFrames = new()
            {
                [0] = new List<Rectangle>()
                {
                    new Rectangle(64, 192, 32, 32),
                },
                [1] = new List<Rectangle>()
                {
                    new Rectangle(32, 192, 32, 32),
                },
                [2] = new List<Rectangle>()
                {
                    new Rectangle(0, 192, 32, 32),
                },
                [3] = new List<Rectangle>()
                {
                    new Rectangle(32, 192, 32, 32),
                }
            };

            schemeColors = new()
            {
                ["None"] = Color.White,
                ["Emerald"] = new Color(67, 255, 83),
                ["Aquamarine"] = new Color(74, 243, 255),
                ["Ruby"] = new Color(255, 38, 38),
                ["Amethyst"] = new Color(255, 67, 251),
                ["Topaz"] = new Color(255, 156, 33),
                ["Solar"] = new Color(255, 194, 128),
                ["Void"] = new Color(200, 100, 190),
            };

        }

        public void GargoyleFlight()
        {
            flightSpeed = 12;

            flightHeight = 2;

            flightFloor = 0;

            flightCeiling = 7;

            flightLast = 0;

            flightInterval = 9;

            flightFrames = new Dictionary<int, List<Rectangle>>()
            {
                [0] = new List<Rectangle>()
                {
                    new Rectangle(0, 160, 32, 32),
                    new Rectangle(32, 160, 32, 32),
                    new Rectangle(64, 160, 32, 32),
                    new Rectangle(32, 160, 32, 32),
                    new Rectangle(128, 160, 32, 32),
                    new Rectangle(160, 160, 32, 32),
                    new Rectangle(96, 160, 32, 32),
                    new Rectangle(160, 160, 32, 32),
                },

                [1] = new List<Rectangle>()
                {
                    new Rectangle(0, 128, 32, 32),
                    new Rectangle(32, 128, 32, 32),
                    new Rectangle(64, 128, 32, 32),
                    new Rectangle(32, 128, 32, 32),
                    new Rectangle(128, 128, 32, 32),
                    new Rectangle(160, 128, 32, 32),
                    new Rectangle(96, 128, 32, 32),
                    new Rectangle(160, 128, 32, 32),
                },
                [2] = new List<Rectangle>()
                {
                    new Rectangle(0, 96, 32, 32),
                    new Rectangle(32, 96, 32, 32),
                    new Rectangle(64, 96, 32, 32),
                    new Rectangle(32, 96, 32, 32),
                    new Rectangle(128, 96, 32, 32),
                    new Rectangle(160, 96, 32, 32),
                    new Rectangle(96, 96, 32, 32),
                    new Rectangle(160, 96, 32, 32),
                },
                [3] = new List<Rectangle>()
                {
                    new Rectangle(0, 128, 32, 32),
                    new Rectangle(32, 128, 32, 32),
                    new Rectangle(64, 128, 32, 32),
                    new Rectangle(32, 128, 32, 32),
                    new Rectangle(128, 128, 32, 32),
                    new Rectangle(160, 128, 32, 32),
                    new Rectangle(96, 128, 32, 32),
                    new Rectangle(160, 128, 32, 32),
                }
            };

        }

        public virtual void GargoyleSpecial()
        {

            abilities = 1;

            specialCeiling = 7;

            specialFloor = 0;

            specialInterval = 9;

            cooldownInterval = 240;

            cooldownTimer = cooldownInterval;

            reachThreshold = 64;

            safeThreshold = 544;

            specialThreshold = 320;

            barrageThreshold = 544;

            specialFrames = idleFrames;

            specialScheme = SpellHandle.schemes.ether;

            sweepSet = false;

            sweepInterval = 12;

            sweepTexture = characterTexture;

            sweepFrames = walkFrames;
            
        }

        public override Rectangle GetBoundingBox()
        {
            Vector2 position = Position;

            int netScale = netMode.Value > 5 ? netMode.Value - 4 : netMode.Value;

            return new Rectangle((int)position.X - 24 - (4 * netScale), (int)position.Y - flightHeight - 48 - bobHeight - (8 * netScale), 96 + (8 * netScale), 96 + (8 * netScale));
        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {


        }

        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {
            
            base.drawAboveAlwaysFrontLayer(b);

            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            float drawLayer = (float)StandingPixel.Y / 10000f;

            DrawEmote(b, localPosition, drawLayer);

            Color schemeColor = schemeColors[netScheme.Value];

            int netScale = netMode.Value > 5 ? netMode.Value - 4 : netMode.Value;

            Vector2 spritePosition = new Vector2(localPosition.X - 24f - (4 * netScale), localPosition.Y - 48f - flightHeight - bobHeight - (8 * netScale));

            float spriteSize = 3.5f + (netScale * 0.25f);

            if (netFlightActive.Value)
            {

                b.Draw(characterTexture, spritePosition, new Rectangle?(flightFrames[netDirection.Value][flightFrame]), Color.White, 0, new Vector2(0.0f, 0.0f), spriteSize, netDirection.Value == 3 || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

                if (netScheme.Value != "None")
                {

                    b.Draw(characterTexture, spritePosition, new Rectangle?(schemeFrames[netDirection.Value][0]), schemeColor, 0, new Vector2(0.0f, 0.0f), spriteSize, netDirection.Value == 3 || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer + 0.0001f);

                }

            }
            else if (netSpecialActive.Value)
            {

                b.Draw(characterTexture, spritePosition, new Rectangle?(flightFrames[netDirection.Value][specialFrame]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), spriteSize, netDirection.Value == 3 || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

                if (netScheme.Value != "None")
                {

                    b.Draw(characterTexture, spritePosition, new Rectangle?(schemeFrames[netDirection.Value][0]), schemeColor, 0, new Vector2(0.0f, 0.0f), spriteSize, netDirection.Value == 3 || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer + 0.0001f);

                }

            }
            else
            {

                b.Draw(characterTexture, spritePosition, new Rectangle?(walkFrames[netDirection.Value][walkFrame]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), spriteSize, netDirection.Value == 3 || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

                if (netScheme.Value != "None")
                {

                    b.Draw(characterTexture, spritePosition, new Rectangle?(schemeFrames[netDirection.Value][0]), schemeColor, 0, new Vector2(0.0f, 0.0f), spriteSize, netDirection.Value == 3 || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer + 0.0001f);

                }

            }

            b.Draw(Game1.shadowTexture, new(localPosition.X, localPosition.Y + 64f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, Vector2.Zero, 4f, 0, drawLayer - 1E-06f);

        }

        public override void update(GameTime time, GameLocation location)
        {

            base.update(time, location);

            if (bobHeight <= 0)
            {
                bobHeight++;
            }
            else if (bobHeight >= 64)
            {
                bobHeight--;
            }

        }

    }

}