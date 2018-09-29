using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SFarmer = StardewValley.Farmer;
using SGame = StardewValley.Game1;
using StardewValley.Menus;
using DatableType = RelationshipTracker.ModConfig.DatableType;

namespace RelationshipTracker
{
    public enum Validation
    {
        AllValid,
        NoValid,
        NoBachelors,
        NoBachelorettes
    }

    public class ModEntry : Mod
    {
        internal ModConfig Config;
        private Texture2D Pixel;
        private Texture2D Cursors;
        private BackgroundRectangle backgroundRect;
        private FriendshipStats[] Stats = new FriendshipStats[6];
        private int ToGoWidth;
        //private int fToGoWidth;
        //private int mToGoWidth;
        private int NameWidth;
        private int NameLength;
        //private List<NPC> Bachelors;
        //private List<NPC> Bachelorettes;
        //private List<FriendshipStats> BachelorStats;
        //private List<FriendshipStats> BacheloretteStats;
        //private bool NoBachelors = false;
        //private bool NoBachelorettes = false;
        private string MaxName;
        private Rectangle HeartCoords = new Rectangle(62, 770, 32, 32);
        private Rectangle RightArrowCoords = new Rectangle(365, 495, 12, 11);
        private Rectangle LeftArrowCoords = new Rectangle(352, 495, 12, 11);
        private Rectangle PortraitCoords = new Rectangle(0, 0, 64, 64);
        private bool toggle = false;
        private ClickableTextureComponent LeftArrowButton;
        private ClickableTextureComponent RightArrowButton;

        //internal DisposableList<NPC> NPCs;

        internal ITranslationHelper i18n => Helper.Translation;

        public override void Entry(IModHelper helper)
        {
            string startingMessage = i18n.Get("template.start", new { mod = helper.ModRegistry.ModID, folder = helper.DirectoryPath });
            //Monitor.Log(startingMessage);

            Config = helper.ReadConfig<ModConfig>();
            if (Config.datableType != DatableType.Bachelor && Config.datableType != DatableType.Bachelorette)
            {
                Config.datableType = DatableType.Bachelorette;
            }
            
            Pixel = new Texture2D(SGame.graphics.GraphicsDevice, 1, 1);
            Cursors = SGame.mouseCursors;

            InputEvents.ButtonPressed += InputEvents_ButtonPressed;
            SaveEvents.AfterLoad += ResetState;
            GameEvents.OneSecondTick += GameEvents_OneSecondTick;
        }

        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            int arrowScaleOffset = 30;

            e.Button.TryGetKeyboard(out Keys keyPressed);
            e.Button.TryGetStardewInput(out InputButton button);

            if (button.mouseLeft)
            {
                if (toggle)
                {
                    ICursorPosition cursonPosition = Helper.Input.GetCursorPosition();
                    if (LeftArrowButton != null && Config.datableType == DatableType.Bachelor)
                    {
                        if (new Rectangle(LeftArrowButton.bounds.X, LeftArrowButton.bounds.Y,
                                LeftArrowButton.bounds.Right + arrowScaleOffset,
                                LeftArrowButton.bounds.Bottom + arrowScaleOffset)
                            .Contains((int) cursonPosition.ScreenPixels.X, (int) cursonPosition.ScreenPixels.Y))
                        {
                            Helper.Input.Suppress(SButton.MouseLeft);
                            if (Validate(DatableType.Bachelorette) != Validation.NoBachelorettes)
                            {
                                SGame.playSound("smallSelect");
                                Config.datableType = DatableType.Bachelorette;
                                GraphicsEvents.OnPostRenderHudEvent -= this.GraphicsEvents_OnPostRenderHudEvent;
                                ProcessAndRender();
                            }
                            else
                            {
                                SGame.showRedMessage("You don't know any Bachelorettes!");
                            }
                        }
                    }

                    if (RightArrowButton != null && Config.datableType == DatableType.Bachelorette)
                    {
                        if (new Rectangle(RightArrowButton.bounds.X, RightArrowButton.bounds.Y,
                                RightArrowButton.bounds.Right + arrowScaleOffset,
                                RightArrowButton.bounds.Bottom + arrowScaleOffset)
                            .Contains((int) cursonPosition.ScreenPixels.X, (int) cursonPosition.ScreenPixels.Y))
                        {
                            Helper.Input.Suppress(SButton.MouseLeft);
                            if (Validate(DatableType.Bachelor) != Validation.NoBachelors)
                            {
                                SGame.playSound("smallSelect");
                                Config.datableType = DatableType.Bachelor;
                                GraphicsEvents.OnPostRenderHudEvent -= this.GraphicsEvents_OnPostRenderHudEvent;
                                ProcessAndRender();
                            }
                            else
                            {
                                SGame.showRedMessage("You don't know any Bachelors!");
                            }
                        }
                    }
                }
            }

            if (keyPressed.Equals(Config.activateKey))
            {
                if (!toggle)
                {
                    //Monitor.Log(i18n.Get("template.key"), LogLevel.Info);
                    if (Validate(Config.datableType, true) == Validation.NoValid)
                    {
                        SGame.showRedMessage("You don't know any eligible villagers");
                    }
                    else if (Config.datableType == DatableType.Bachelorette && Validate(DatableType.Bachelorette) == Validation.NoBachelorettes)
                    {
                        Config.datableType = DatableType.Bachelor;
                        if (Validate(Config.datableType) != Validation.NoBachelors)
                        {
                            toggle = !toggle;
                            ProcessAndRender();
                        }
                    }
                    else if (Config.datableType == DatableType.Bachelor && Validate(DatableType.Bachelor) == Validation.NoBachelors)
                    {
                        Config.datableType = DatableType.Bachelorette;
                        if (Validate(Config.datableType) != Validation.NoBachelorettes)
                        {
                            toggle = !toggle;
                            ProcessAndRender();
                        }
                    }
                    else
                    {
                        toggle = !toggle;
                        ProcessAndRender();
                    }
                    //}
                }
                else
                {
                    toggle = !toggle;
                    LeftArrowButton = null;
                    RightArrowButton = null;
                    GraphicsEvents.OnPostRenderHudEvent -= this.GraphicsEvents_OnPostRenderHudEvent;
                }
            }
        }

        public void ProcessAndRender()
        {
            var farmers = SGame.getAllFarmers();
            Friendship friendship;
            int counter = 0;
            ToGoWidth = 0;
            NameWidth = 0;
            NameLength = 0;
            MaxName = "";
            for (int i=0; i < 6; i++)
            {
                Stats[i] = null;
            }

            foreach (NPC npc in GetDatables(Config.datableType))
            {
                foreach (SFarmer farmer in farmers)
                {
                    if (!farmer.friendshipData.ContainsKey(npc.getName()))
                    {
                        continue;
                    }

                    friendship = farmer.friendshipData[npc.getName()];
                    Stats[counter] = new FriendshipStats(farmer, npc, friendship, Config.datableType);
                    if (SGame.smallFont.MeasureString(Stats[counter].Level.ToString()).X > ToGoWidth)
                    {
                        ToGoWidth = (int)SGame.smallFont.MeasureString(Stats[counter].Level.ToString()).X;
                    }
                    if ((int)SGame.smallFont.MeasureString(Stats[counter].Name).X > NameWidth)
                    {
                        NameWidth = (int)SGame.smallFont.MeasureString(Stats[counter].Name).X;
                    }
                    if (Stats[counter].Name.Length > NameLength)
                    {
                        NameLength = Stats[counter].Name.Length;
                        MaxName = Stats[counter].Name;
                    }
                    counter++;
                }
            }

                GraphicsEvents.OnPostRenderHudEvent += this.GraphicsEvents_OnPostRenderHudEvent;

        }

        private List<NPC> GetDatables(DatableType datableType)
        {
            List<NPC> datables = new List<NPC>();

            foreach (NPC character in Utility.getAllCharacters())
            {
                if (character.isVillager() && character.Gender == (int)datableType)
                    if (character.datable.Value)
                    {
                        datables.Add(character);
                    }
            }
            datables.Sort();
            datables.Reverse();
            return datables;
        }

        public void GraphicsEvents_OnPostRenderHudEvent(object sender, EventArgs e)
        {
            int x = Config.offsetX;
            int y = Config.offsetY;

            int row = y + 5 + 54;
            int textX = 5;
            int textX2 = 6;

            int headingYOffset = 11;
            int portraitOffset = 0;
            int nameSpace = (int)SGame.smallFont.MeasureString(MaxName).X;

            if (Config.showPortrait)
            {
                portraitOffset = 32;
            }
            int bachelorOffset = 0;

            string heading = "Bachelorettes";
            if (Config.datableType == DatableType.Bachelor)
            {
                bachelorOffset = 50;
                heading = "Bachelors";
            }
            Vector2 headingSpace = SGame.smallFont.MeasureString(heading);

            float heartScale = 0.8f;
            int heartWidth = (int)(32 * heartScale);
            int baseWidth = (int)SGame.smallFont.MeasureString(" | " + " | " + " to go").X;
            int width = (int)((baseWidth + headingSpace.X + NameWidth + heartWidth + ToGoWidth) * 0.8f);
            int height = 218 + 54;

            int midSpace = 0;
            string msg;
            string msgMid;
            string msgToGo;
            float headingX = ((portraitOffset + width + bachelorOffset - headingSpace.X) / 2);
            int alpha = (int)(255 * Config.backgroundOpacity);

            if (Config.drawBackground)
            {
                backgroundRect = new BackgroundRectangle(x, y, portraitOffset + width + bachelorOffset, height, new Color(255, 210, 132, alpha), Game1.spriteBatch, SGame.graphics.GraphicsDevice, Pixel);
                backgroundRect.Draw();
                backgroundRect.DrawBorder();
            }
            if (heading == "Bachelors")
            {
                //LeftArrowButton = new ClickableTextureComponent(new Rectangle(x + 6, y + 6, 12, 11), Cursors, LeftArrowCoords, 4f) { hoverText = "Show Bachelorettes" };
                LeftArrowButton = new ClickableTextureComponent(new Rectangle((int)headingX - (int)(13*4.0f), y + 6, 12, 11), Cursors, LeftArrowCoords, 4f) { hoverText = "Show Bachelorettes" };
                LeftArrowButton.draw(SGame.spriteBatch);
            }
            SGame.spriteBatch.DrawString(SGame.smallFont, heading, new Vector2(headingX+1, y + headingYOffset + 1), Color.DarkGoldenrod);
            SGame.spriteBatch.DrawString(SGame.smallFont, heading, new Vector2(headingX+2, y + headingYOffset), new Color(73, 45, 51));
            if (heading == "Bachelorettes")
            {
                //RightArrowButton = new ClickableTextureComponent(new Rectangle(portraitOffset + width + bachelorOffset - 8 - Arrow.Width, y + 6, 12, 11), Cursors, RightArrowCoords, 4f) { hoverText = "Show Bachelors" };
                RightArrowButton = new ClickableTextureComponent(new Rectangle((int)headingX + (int)headingSpace.X + 8, y + 6, 12, 11), Cursors, RightArrowCoords, 4f) { hoverText = "Show Bachelors" };
                RightArrowButton.draw(SGame.spriteBatch);
            }
            
            for (int i=0; i < 6; i++)
            {
                if (Stats[i] != null)
                {
                    //msg = Stats[i].Name + " | " + Stats[i].Level + "";
                    msg = Stats[i].Name;
                    Vector2 msgSpace = SGame.smallFont.MeasureString(msg);
                    msgMid = " " + Stats[i].Level.ToString() + "";
                    midSpace = nameSpace - (int)msgSpace.X;
                    msgToGo = " | " + Stats[i].ToNextLevel.ToString() + " to next";
                    Vector2 msgToGoSpace = SGame.smallFont.MeasureString(msgToGo);
                    float yOffset = row;
                    if (i > 0)
                    {
                        yOffset += (int)msgSpace.Y;
                    }
                    if (Config.showPortrait)
                    {
                        SGame.spriteBatch.Draw(Stats[i].Portrait.Image, new Vector2(textX2, row - 1), PortraitCoords, Color.White, 0, new Vector2(), 0.5f, SpriteEffects.None, 0);
                    }
                    SGame.spriteBatch.DrawString(SGame.smallFont, msg, new Vector2(portraitOffset + textX, row + 1), Color.DarkGoldenrod);
                    SGame.spriteBatch.DrawString(SGame.smallFont, msg, new Vector2(portraitOffset + textX2, row), new Color(73, 45, 51));
                    SGame.spriteBatch.DrawString(SGame.smallFont, msgMid, new Vector2(portraitOffset + textX + msgSpace.X + midSpace, row + 1), Color.DarkGoldenrod);
                    SGame.spriteBatch.DrawString(SGame.smallFont, msgMid, new Vector2(portraitOffset + textX2 + msgSpace.X + midSpace, row), new Color(73, 45, 51));
                    SGame.spriteBatch.Draw(SGame.menuTexture, new Vector2(portraitOffset + textX + msgSpace.X + midSpace + heartWidth * 1.2f, row + 3), HeartCoords, Color.DarkGoldenrod, 0, new Vector2(), heartScale, SpriteEffects.None, 0);
                    SGame.spriteBatch.Draw(SGame.menuTexture, new Vector2(portraitOffset + textX2 + msgSpace.X + midSpace + heartWidth * 1.2f, row + 2), HeartCoords, Color.White, 0, new Vector2(), heartScale, SpriteEffects.None, 0);
                    SGame.spriteBatch.DrawString(SGame.smallFont, msgToGo, new Vector2(portraitOffset + textX + msgSpace.X + midSpace + heartWidth + 33, row + 1), Color.DarkGoldenrod);
                    SGame.spriteBatch.DrawString(SGame.smallFont, msgToGo, new Vector2(portraitOffset + textX2 + msgSpace.X + midSpace + heartWidth + 33, row), new Color(75, 45, 51));
                    row += 1;
                    row += (int)msgSpace.Y;
                }
            }
        }
                
        public void ResetState(object sender, EventArgs e)
        {
            for (int i=0; i < 6; i++)
            {
                Stats[i] = null;
                //Attempts = 0;
                //NoBachelors = false;
                //NoBachelorettes = false;
            }
            if (toggle)
            {
                GraphicsEvents.OnPostRenderHudEvent -= GraphicsEvents_OnPostRenderHudEvent;
            }
        }

        //public void GetStats()
        //{
        //    var farmers = SGame.getAllFarmers();
        //    Friendship friendship;
        //    int i = 0;
        //    fToGoWidth = 0; // need separate M/F values
        //    mToGoWidth = 0;
        //    NameWidth = 0; // need separate M/F values
        //    NameLength = 0; // need separate M/F values
        //    MaxName = ""; // need separate M/F values

        //    Bachelors = GetDatables(DatableType.Bachelor);
        //    Bachelorettes = GetDatables(DatableType.Bachelorette);
        //    foreach (NPC npc in Bachelors)
        //    {
        //        foreach (SFarmer farmer in farmers)
        //        {
        //            if (!farmer.friendshipData.ContainsKey(npc.getName()))
        //                continue;

        //            friendship = farmer.friendshipData[npc.getName()];
        //            BachelorStats.Add(new FriendshipStats(farmer, npc, friendship, DatableType.Bachelor));
        //            if ((int)SGame.smallFont.MeasureString(BachelorStats[i].Level.ToString()).X > mToGoWidth)
        //                mToGoWidth = (int)SGame.smallFont.MeasureString(BachelorStats[i].Level.ToString()).X;
        //            if (SGame.smallFont.MeasureString(BachelorStats[i].Name.ToString()).X > NameWidth)
        //                NameWidth = (int)SGame.smallFont.MeasureString(BachelorStats[i].Level.ToString()).X;
        //            if (BachelorStats[i].Name.Length > NameLength)
        //            {
        //                NameLength = BachelorStats[i].Name.Length;
        //                MaxName = BachelorStats[i].Name;
        //            }
        //        }
        //    }

        //}

        private Validation Validate(DatableType datableType, bool checkAll = false, [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            Validation validation = Validation.AllValid;
            var farmers = SGame.getAllFarmers();
            int i = 0;
            int invalidCounter = 0;
            if (datableType == DatableType.Bachelor || checkAll == true)
            {
                foreach (NPC npc in GetDatables(DatableType.Bachelor))
                {
                    foreach (SFarmer farmer in farmers)
                    {
                        if (!farmer.friendshipData.ContainsKey(npc.getName()))
                            continue;

                        i++;
                    }
                }
                if (i == 0)
                {
                    validation = Validation.NoBachelors;
                    if (checkAll)
                    {
                        invalidCounter++;
                    }
                    else
                    {
                        return validation;
                    }
                }
            }
            i = 0;
            if (datableType == DatableType.Bachelorette || checkAll == true)
            {
                foreach (NPC npc in GetDatables(DatableType.Bachelorette))
                {
                    foreach (SFarmer farmer in farmers)
                    {
                        if (!farmer.friendshipData.ContainsKey(npc.getName()))
                            continue;

                        i++;
                    }
                }
                if (i == 0)
                {
                    validation = Validation.NoBachelorettes;
                    if (checkAll)
                    {
                        invalidCounter++;
                    }
                    else
                    {
                        return validation;
                    }
                    
                }
            }
            if (checkAll == true)
            {
                if (invalidCounter == 2)
                    validation = Validation.NoValid;
            }
            return validation;
        }

        private void GameEvents_OneSecondTick(object sender, EventArgs e)
        {
            if (toggle)
            {
                if (Config.datableType == DatableType.Bachelor && Validate(Config.datableType) != Validation.NoBachelors)
                {
                    GraphicsEvents.OnPostRenderHudEvent -= this.GraphicsEvents_OnPostRenderHudEvent;
                    ProcessAndRender();
                }
                else if (Config.datableType == DatableType.Bachelorette && Validate(Config.datableType) != Validation.NoBachelorettes)
                {
                    GraphicsEvents.OnPostRenderHudEvent -= this.GraphicsEvents_OnPostRenderHudEvent;
                    ProcessAndRender();
                }
            }
        }
    }
}
