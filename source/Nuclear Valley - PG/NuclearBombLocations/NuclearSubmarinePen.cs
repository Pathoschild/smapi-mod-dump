/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ApryllForever/NuclearBombLocations
**
*************************************************/

using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using xTile.Dimensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile;
using StardewModdingAPI.Utilities;
using StardewValley.Network;
using StardewValley.Objects;
using xTile.Tiles;
using StardewValley.Menus;
using Object = StardewValley.Object;
using StardewValley.Monsters;

namespace NuclearBombLocations
{
    [XmlType("Mods_ApryllForever_RestStopLocations_NuclearSubmarinePen")]
    public class NuclearSubmarinePen : NuclearLocation
    {

        public const float steamZoom = 2f;

        public const float steamYMotionPerMillisecond = 0.1f;

        public const float millisecondsPerSteamFrame = 150f;

        private Texture2D steamAnimation;

        private Texture2D swimShadow;

        private Vector2 steamPosition;

        private float steamYOffset;

        private int swimShadowTimer;

        private int swimShadowFrame;

        public static string EnterSubQuestion = "Do you wish to enter Marisol's Nuclear Submarine?";



        internal static IModHelper ModHelper { get; set; }


        public NuclearSubmarinePen() { }
        public NuclearSubmarinePen(IModContentHelper content)
        : base(content, "NuclearSubmarinePen", "NuclearSubmarinePen")
        {
         
        }

        public NuclearSubmarinePen(IModHelper helper) 
        {
            //MermaidDugoutHouse.ModHelper = IModHelper;
            //helper.Events.GameLoop.DayEnding += OnDayEnding;



        }

        protected override void resetLocalState()
        {
            base.resetLocalState();
            Game1.changeMusicTrack("Cavern");
            swimShadow = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\swimShadow");
           // if (Game1.currentLocation == this)
           // {
             //   Game1.ambientLight = Color.BlueViolet;
           // }
        }
        public override void cleanupBeforePlayerExit()
        {
            base.cleanupBeforePlayerExit();
            if (Game1.player.swimming.Value)
            {
                Game1.player.swimming.Value = false;
            }
            if (Game1.locationRequest != null && !Game1.locationRequest.Name.Contains("BathHouse"))
            {
                Game1.player.bathingClothes.Value = false;
            }
            Game1.changeMusicTrack("none");
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
           
           

            { 
            if (currentEvent != null)
            {
                foreach (NPC j in currentEvent.actors)
                {
                    if ((bool)j.swimming.Value)
                    {
                        b.Draw(swimShadow, Game1.GlobalToLocal(Game1.viewport, j.Position + new Vector2(0f, j.Sprite.SpriteHeight / 3 * 4 + 4)), new Microsoft.Xna.Framework.Rectangle(swimShadowFrame * 16, 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0f);
                    }
                }
            }
            else
            {
                foreach (NPC i in characters)
                {
                    if ((bool)i.swimming.Value)
                    {
                        b.Draw(swimShadow, Game1.GlobalToLocal(Game1.viewport, i.Position + new Vector2(0f, i.Sprite.SpriteHeight / 3 * 4 + 4)), new Microsoft.Xna.Framework.Rectangle(swimShadowFrame * 16, 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0f);
                    }
                }
                foreach (Farmer f in farmers)
                {
                    if ((bool)f.swimming.Value)
                    {
                        b.Draw(swimShadow, Game1.GlobalToLocal(Game1.viewport, f.Position + new Vector2(0f, f.Sprite.SpriteHeight / 4 * 4)), new Microsoft.Xna.Framework.Rectangle(swimShadowFrame * 16, 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0f);
                    }
                }
            }
            _ = (bool)Game1.player.swimming.Value;
        }


           
        }

        public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {

           
            {
                switch (base.getTileIndexAt(tileLocation, "Buildings"))
                {
                    case 941:

                        createQuestionDialogue(EnterSubQuestion, createYesNoResponses(), "EnterSub");

                        return true;
                }
            }

            return base.checkAction(tileLocation, viewport, who);
        }


        public override bool answerDialogue(Response answer)
        {
            if (lastQuestionKey != null && afterQuestion == null)
            {
                string qa = lastQuestionKey.Split(' ')[0] + "_" + answer.responseKey;
                switch (qa)
                {
                    case "EnterSub_Yes":
                        performTouchAction("Warp " + "Custom_MermaidNuclearSub 8 8", Game1.player.Tile);
                        return true;


                       
                }
            }

            return base.answerDialogue(answer);
        }

        public override bool performAction(string action, Farmer who, Location tileLocation)
        {
            
            if (action == "RS.Hospital")
            {
               

            }

            return base.performAction(action, who, tileLocation);
        }

      
        public override void performTouchAction(string actionStr, Vector2 tileLocation)
        {
            string[] split = actionStr.Split(' ');
            string action = split[0];
            int tx = (int)tileLocation.X;
            int ty = (int)tileLocation.Y;

            if (action == "RS.Diveboard")
            {
                Game1.player.jump();
                Game1.player.xVelocity = -16f;
                Game1.player.swimTimer = 800;
                Game1.player.swimming.Value = true;
                Game1.playSound("pullItemFromWater");
                Game1.playSound("bubbles");
            }

            base.performTouchAction(actionStr, tileLocation);
        }

        public override void UpdateWhenCurrentLocation(GameTime time)
        {
            base.UpdateWhenCurrentLocation(time);
            swimShadowTimer -= time.ElapsedGameTime.Milliseconds;
            if (swimShadowTimer <= 0)
            {
                swimShadowTimer = 70;
                swimShadowFrame++;
                swimShadowFrame %= 10;
            }
        }



       
     










    }
}