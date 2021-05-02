/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCEnemies.Spawners;
using Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCStatusEffects;
using StardewValley;
using StardustCore.Animations;
using StardustCore.UIUtilities;
using StardustCore.UIUtilities.MenuComponents;
using StardustCore.UIUtilities.MenuComponents.ComponentsV1;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCMenus
{
    /// <summary>
    /// TODO: Giev prompt to start the game by pressing enter/start/space.
    /// </summary>
    public class CharacterSelectScreen : IClickableMenuExtended
    {
        StardustCore.UIUtilities.Texture2DExtended background;
        string menuTitle;

        public Dictionary<SSCEnums.PlayerID, int> playerColorIndex;

        public Dictionary<SSCEnums.PlayerID, Vector2> playerDisplayLocations;

        public List<Color> possibleColors;

        public bool closeMenu;

        public Dictionary<SSCEnums.PlayerID, int> inputDelays;
        public int maxInputDelay = 20;

        public Dictionary<string, StardustCore.Animations.AnimatedSprite> animatedSprites;

        public Dictionary<string, Button> buttons;

        public const float controllerMouseSensitivity = 3f;

        private Vector2 oldMousePos;

        public CharacterSelectScreen(int x, int y, int width, int height) : base(x, y, width, height, false)
        {
            this.background = SeasideScramble.self.textureUtils.getExtendedTexture("SSCMaps", "TitleScreenBackground");
            this.menuTitle = "Character Selection";

            this.playerDisplayLocations = new Dictionary<SSCEnums.PlayerID, Vector2>()
            {
                {SSCEnums.PlayerID.One,new Vector2(SeasideScramble.self.camera.viewport.Width*.2f, SeasideScramble.self.camera.viewport.Height*.5f)},
                {SSCEnums.PlayerID.Two,new Vector2(SeasideScramble.self.camera.viewport.Width*.4f, SeasideScramble.self.camera.viewport.Height*.5f)},
                {SSCEnums.PlayerID.Three,new Vector2(SeasideScramble.self.camera.viewport.Width*.6f, SeasideScramble.self.camera.viewport.Height*.5f)},
                {SSCEnums.PlayerID.Four,new Vector2(SeasideScramble.self.camera.viewport.Width*.8f, SeasideScramble.self.camera.viewport.Height*.5f)},
            };

            this.possibleColors = new List<Color>()
            {
                Color.PaleVioletRed,
                Color.LightSkyBlue,
                Color.LawnGreen,
                Color.LightGoldenrodYellow,
                Color.HotPink,
                Color.Red,
                Color.Purple
            };
            this.playerColorIndex = new Dictionary<SSCEnums.PlayerID, int>()
            {
                { SSCEnums.PlayerID.One,-1},
                { SSCEnums.PlayerID.Two,-1},
                { SSCEnums.PlayerID.Three,-1},
                { SSCEnums.PlayerID.Four,-1},
            };
            this.inputDelays = new Dictionary<SSCEnums.PlayerID, int>()
            {
                { SSCEnums.PlayerID.One,0},
                { SSCEnums.PlayerID.Two,0},
                { SSCEnums.PlayerID.Three,0},
                { SSCEnums.PlayerID.Four,0},
            };
            SeasideScramble.self.camera.snapToPosition(new Vector2(0, 0));

            this.animatedSprites = new Dictionary<string, StardustCore.Animations.AnimatedSprite>();

            this.animatedSprites.Add("P1AButton", new StardustCore.Animations.AnimatedSprite("P1AButton", this.playerDisplayLocations[SSCEnums.PlayerID.One] + new Vector2(0, 100), new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "AButton"), new Animation(0, 0, 28, 27)), Color.White));
            this.animatedSprites.Add("P2AButton", new StardustCore.Animations.AnimatedSprite("P2AButton", this.playerDisplayLocations[SSCEnums.PlayerID.Two] + new Vector2(0, 100), new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "AButton"), new Animation(0, 0, 28, 27)), Color.White));
            this.animatedSprites.Add("P3AButton", new StardustCore.Animations.AnimatedSprite("P3AButton", this.playerDisplayLocations[SSCEnums.PlayerID.Three] + new Vector2(0, 100), new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "AButton"), new Animation(0, 0, 28, 27)), Color.White));
            this.animatedSprites.Add("P4AButton", new StardustCore.Animations.AnimatedSprite("P4AButton", this.playerDisplayLocations[SSCEnums.PlayerID.Four] + new Vector2(0, 100), new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "AButton"), new Animation(0, 0, 28, 27)), Color.White));

            this.animatedSprites.Add("P1Click", new StardustCore.Animations.AnimatedSprite("P1Click", this.playerDisplayLocations[SSCEnums.PlayerID.One] + new Vector2(0, 150), new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "MouseClick"), new Animation(0, 0, 31, 32), new Dictionary<string, List<Animation>>()
            {
                {"Click1",new List<Animation>(){
                    new Animation(0,0,31,32,60),
                    new Animation(31,0,31,32,60)

                } }
            }, "Click1"), Color.White));

            this.animatedSprites.Add("P1Color", new StardustCore.Animations.AnimatedSprite("P1Color", new Vector2(this.playerDisplayLocations[SSCEnums.PlayerID.One].X, this.playerDisplayLocations[SSCEnums.PlayerID.One].Y + 250), new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "BlankTexture"), new Animation(0, 0, 32, 32)), Color.White));
            this.animatedSprites.Add("P2Color", new StardustCore.Animations.AnimatedSprite("P2Color", new Vector2(this.playerDisplayLocations[SSCEnums.PlayerID.Two].X, this.playerDisplayLocations[SSCEnums.PlayerID.One].Y + 250), new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "BlankTexture"), new Animation(0, 0, 32, 32)), Color.White));
            this.animatedSprites.Add("P3Color", new StardustCore.Animations.AnimatedSprite("P3Color", new Vector2(this.playerDisplayLocations[SSCEnums.PlayerID.Three].X, this.playerDisplayLocations[SSCEnums.PlayerID.One].Y + 250), new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "BlankTexture"), new Animation(0, 0, 32, 32)), Color.White));
            this.animatedSprites.Add("P4Color", new StardustCore.Animations.AnimatedSprite("P4Color", new Vector2(this.playerDisplayLocations[SSCEnums.PlayerID.Four].X, this.playerDisplayLocations[SSCEnums.PlayerID.One].Y + 250), new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "BlankTexture"), new Animation(0, 0, 32, 32)), Color.White));


            this.buttons = new Dictionary<string, Button>();
            this.buttons.Add("P1PrevButton", new Button("P1PrevButton", new Rectangle((int)this.playerDisplayLocations[SSCEnums.PlayerID.One].X - 64, (int)this.playerDisplayLocations[SSCEnums.PlayerID.One].Y + 250, 64, 64), SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "lastPageButton"), new Rectangle(0, 0, 32, 32), 2f));
            this.buttons.Add("P1NextButton", new Button("P1NextButton", new Rectangle((int)this.playerDisplayLocations[SSCEnums.PlayerID.One].X + 64, (int)this.playerDisplayLocations[SSCEnums.PlayerID.One].Y + 250, 64, 64), SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "nextPageButton"), new Rectangle(0, 0, 32, 32), 2f));
            this.buttons.Add("P2PrevButton", new Button("P2PrevButton", new Rectangle((int)this.playerDisplayLocations[SSCEnums.PlayerID.Two].X - 64, (int)this.playerDisplayLocations[SSCEnums.PlayerID.Two].Y + 250, 64, 64), SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "lastPageButton"), new Rectangle(0, 0, 32, 32), 2f));
            this.buttons.Add("P2NextButton", new Button("P2NextButton", new Rectangle((int)this.playerDisplayLocations[SSCEnums.PlayerID.Two].X + 64, (int)this.playerDisplayLocations[SSCEnums.PlayerID.Two].Y + 250, 64, 64), SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "nextPageButton"), new Rectangle(0, 0, 32, 32), 2f));
            this.buttons.Add("P3PrevButton", new Button("P3PrevButton", new Rectangle((int)this.playerDisplayLocations[SSCEnums.PlayerID.Three].X - 64, (int)this.playerDisplayLocations[SSCEnums.PlayerID.Three].Y + 250, 64, 64), SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "lastPageButton"), new Rectangle(0, 0, 32, 32), 2f));
            this.buttons.Add("P3NextButton", new Button("P3NextButton", new Rectangle((int)this.playerDisplayLocations[SSCEnums.PlayerID.Three].X + 64, (int)this.playerDisplayLocations[SSCEnums.PlayerID.Three].Y + 250, 64, 64), SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "nextPageButton"), new Rectangle(0, 0, 32, 32), 2f));
            this.buttons.Add("P4PrevButton", new Button("P4PrevButton", new Rectangle((int)this.playerDisplayLocations[SSCEnums.PlayerID.Four].X - 64, (int)this.playerDisplayLocations[SSCEnums.PlayerID.Four].Y + 250, 64, 64), SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "lastPageButton"), new Rectangle(0, 0, 32, 32), 2f));
            this.buttons.Add("P4NextButton", new Button("P4NextButton", new Rectangle((int)this.playerDisplayLocations[SSCEnums.PlayerID.Four].X + 64, (int)this.playerDisplayLocations[SSCEnums.PlayerID.Four].Y + 250, 64, 64), SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "nextPageButton"), new Rectangle(0, 0, 32, 32), 2f));
            //this.animatedSprites.Add("P1PrevColor", new StardustCore.Animations.AnimatedSprite("P1PrevColor", this.playerDisplayLocations[SSCEnums.PlayerID.One] + new Vector2(0, 200), new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "lastPageButton"), new Animation(0, 0, 32, 32)), Color.White));
            //this.animatedSprites.Add("P1NextColor", new StardustCore.Animations.AnimatedSprite("P1NextColor", this.playerDisplayLocations[SSCEnums.PlayerID.One] + new Vector2(64, 200), new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "nextPageButton"), new Animation(0, 0, 32, 32)), Color.White));


            this.oldMousePos = new Vector2(Game1.getMousePosition().X, Game1.getMousePosition().Y);
        }

        public CharacterSelectScreen(xTile.Dimensions.Rectangle viewport) : this(0, 0, viewport.Width, viewport.Height)
        {


        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            this.xPositionOnScreen = 0;
            this.yPositionOnScreen = 0;
            this.width = newBounds.Width;
            this.height = newBounds.Height;
            //base.gameWindowSizeChanged(oldBounds, newBounds);

            //this.p1DisplayLocation = new Vector2(this.width/10,this.height/2);
            SeasideScramble.self.camera.snapToPosition(new Vector2(0, 0));

            this.playerDisplayLocations[SSCEnums.PlayerID.One] = new Vector2(SeasideScramble.self.camera.viewport.Width * .2f, SeasideScramble.self.camera.viewport.Height * .5f);
            this.playerDisplayLocations[SSCEnums.PlayerID.Two] = new Vector2(SeasideScramble.self.camera.viewport.Width * .4f, SeasideScramble.self.camera.viewport.Height * .5f);
            this.playerDisplayLocations[SSCEnums.PlayerID.Three] = new Vector2(SeasideScramble.self.camera.viewport.Width * .6f, SeasideScramble.self.camera.viewport.Height * .5f);
            this.playerDisplayLocations[SSCEnums.PlayerID.Four] = new Vector2(SeasideScramble.self.camera.viewport.Width * .8f, SeasideScramble.self.camera.viewport.Height * .5f);
        }

        public override void update(GameTime time)
        {
            GamePadState p1 = SeasideScramble.self.getGamepadState(PlayerIndex.One);
            GamePadState p2 = SeasideScramble.self.getGamepadState(PlayerIndex.Two);
            GamePadState p3 = SeasideScramble.self.getGamepadState(PlayerIndex.Three);
            GamePadState p4 = SeasideScramble.self.getGamepadState(PlayerIndex.Four);

            if (p1.IsButtonDown(Buttons.A))
            {
                if (SeasideScramble.self.getPlayer(SSCEnums.PlayerID.One) == null)
                {
                    this.initializeCharacter(SSCEnums.PlayerID.One);
                }
                else
                {
                    this.receiveGamepadLeftClick(SSCEnums.PlayerID.One, SeasideScramble.self.getPlayer(SSCEnums.PlayerID.One).mouseCursor.position);
                }
            }
            if (p2.IsButtonDown(Buttons.A))
            {
                if (SeasideScramble.self.getPlayer(SSCEnums.PlayerID.Two) == null)
                {
                    this.initializeCharacter(SSCEnums.PlayerID.Two);
                    SeasideScramble.self.getPlayer(SSCEnums.PlayerID.Two).mouseCursor.position = this.playerDisplayLocations[SSCEnums.PlayerID.Two];
                }
                else
                {
                    this.receiveGamepadLeftClick(SSCEnums.PlayerID.Two, SeasideScramble.self.getPlayer(SSCEnums.PlayerID.Two).mouseCursor.position);
                }
            }
            if (p3.IsButtonDown(Buttons.A))
            {
                if (SeasideScramble.self.getPlayer(SSCEnums.PlayerID.Three) == null)
                {
                    this.initializeCharacter(SSCEnums.PlayerID.Three);
                    SeasideScramble.self.getPlayer(SSCEnums.PlayerID.Three).mouseCursor.position = this.playerDisplayLocations[SSCEnums.PlayerID.Three];
                }
                else
                {
                    this.receiveGamepadLeftClick(SSCEnums.PlayerID.Three, SeasideScramble.self.getPlayer(SSCEnums.PlayerID.Three).mouseCursor.position);
                }
            }
            if (p4.IsButtonDown(Buttons.A))
            {
                if (SeasideScramble.self.getPlayer(SSCEnums.PlayerID.Four) == null)
                {
                    this.initializeCharacter(SSCEnums.PlayerID.Four);
                    SeasideScramble.self.getPlayer(SSCEnums.PlayerID.Four).mouseCursor.position = this.playerDisplayLocations[SSCEnums.PlayerID.Four];
                }
                else
                {
                    this.receiveGamepadLeftClick(SSCEnums.PlayerID.Four, SeasideScramble.self.getPlayer(SSCEnums.PlayerID.Four).mouseCursor.position);
                }
            }
            if (p1.IsButtonDown(Buttons.Start))
            {
                if (SeasideScramble.self.currentNumberOfPlayers > 0)
                {
                    this.setUpForGameplay();
                    SeasideScramble.self.menuManager.closeAllMenus();
                    SeasideScramble.self.menuManager.addNewMenu(new Loby(0, 0, SeasideScramble.self.camera.viewport.Width, SeasideScramble.self.camera.viewport.Height));
                }
            }

            //P1 is handled automatically since SDV maps keyboard to controller.
            this.checkForControllerColorSwap(p2, SSCEnums.PlayerID.Two);
            this.checkForControllerColorSwap(p3, SSCEnums.PlayerID.Three);
            this.checkForControllerColorSwap(p4, SSCEnums.PlayerID.Four);


            this.inputDelays[SSCEnums.PlayerID.One]--;
            this.inputDelays[SSCEnums.PlayerID.Two]--;
            this.inputDelays[SSCEnums.PlayerID.Three]--;
            this.inputDelays[SSCEnums.PlayerID.Four]--;

            if (this.inputDelays[SSCEnums.PlayerID.One] < 0) this.inputDelays[SSCEnums.PlayerID.One] = 0;
            if (this.inputDelays[SSCEnums.PlayerID.Two] < 0) this.inputDelays[SSCEnums.PlayerID.Two] = 0;
            if (this.inputDelays[SSCEnums.PlayerID.Three] < 0) this.inputDelays[SSCEnums.PlayerID.Three] = 0;
            if (this.inputDelays[SSCEnums.PlayerID.Four] < 0) this.inputDelays[SSCEnums.PlayerID.Four] = 0;


            SeasideScramble.self.camera.snapToPosition(new Vector2(0, 0));

            this.oldMousePos = new Vector2(Game1.getMousePosition().X, Game1.getMousePosition().Y);
        }

        /// <summary>
        /// Looks for controller input for swaping character colors.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="player"></param>
        private void checkForControllerColorSwap(GamePadState state, SSCEnums.PlayerID player)
        {
            if (state.IsButtonDown(Buttons.DPadRight))
            {
                this.iteratePlayerColorIndex(player, 1);
            }
            else if (state.IsButtonDown(Buttons.DPadLeft))
            {
                this.iteratePlayerColorIndex(player, -1);
            }
            if (state.ThumbSticks.Left.X < 0)
            {
                this.iteratePlayerColorIndex(player, -1);
            }
            else if (state.ThumbSticks.Left.X > 0)
            {
                this.iteratePlayerColorIndex(player, 1);
            }
        }

        /// <summary>
        /// Initializes a given character.
        /// </summary>
        /// <param name="player"></param>
        public void initializeCharacter(SSCEnums.PlayerID player)
        {
            if (SeasideScramble.self.players.ContainsKey(player))
            {
                return;
            }
            else
            {
                SeasideScramble.self.players.Add(player, new SSCPlayer(player));
                this.iteratePlayerColorIndex(player, 1);
            }
            this.setPlayerColor(player);
        }

        /// <summary>
        /// Sets a given player's color.
        /// </summary>
        /// <param name="player"></param>
        private void setPlayerColor(SSCEnums.PlayerID player)
        {
            if (SeasideScramble.self.getPlayer(player) == null) return;

            ModCore.log("Player: " + player);
            ModCore.log("Index: " + this.playerColorIndex[player]);

            SeasideScramble.self.getPlayer(player).setColor(this.possibleColors[this.playerColorIndex[player]]);
            if (player == SSCEnums.PlayerID.One)
            {
                this.animatedSprites["P1Color"].color = this.possibleColors[this.playerColorIndex[player]];
            }
            if (player == SSCEnums.PlayerID.Two)
            {
                this.animatedSprites["P2Color"].color = this.possibleColors[this.playerColorIndex[player]];
            }
            if (player == SSCEnums.PlayerID.Three)
            {
                this.animatedSprites["P3Color"].color = this.possibleColors[this.playerColorIndex[player]];
            }
            if (player == SSCEnums.PlayerID.Four)
            {
                this.animatedSprites["P4Color"].color = this.possibleColors[this.playerColorIndex[player]];
            }
        }

        /// <summary>
        /// Iterates the player's color index to get the next possible color.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="amount"></param>
        private void iteratePlayerColorIndex(SSCEnums.PlayerID player, int amount)
        {

            if (this.inputDelays[player] != 0) return;
            this.inputDelays[player] = this.maxInputDelay;
            while (this.doesAnyOtherPlayerHaveThisColor(player) == true)
            {
                if (this.playerColorIndex[player] >= this.possibleColors.Count)
                {
                    this.playerColorIndex[player] = 0;
                }
                else if (this.playerColorIndex[player] < 0)
                {
                    this.playerColorIndex[player] = this.possibleColors.Count - 1;
                }
                else
                {
                    this.playerColorIndex[player] += amount;
                }
            }
            this.playerColorIndex[player] += amount;
            if (this.playerColorIndex[player] >= this.possibleColors.Count)
            {
                this.playerColorIndex[player] = 0;
            }
            else if (this.playerColorIndex[player] < 0)
            {
                this.playerColorIndex[player] = this.possibleColors.Count - 1;
            }


            this.setPlayerColor(player);
        }

        /// <summary>
        /// Checks if a given player has the same color index as another to prevent color duplicates.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        private bool doesOtherPlayerHaveThisColor(SSCEnums.PlayerID self, SSCEnums.PlayerID other)
        {
            if (SeasideScramble.self.getPlayer(other) == null) return false;
            if (this.playerColorIndex[self] == this.playerColorIndex[other]) return true;
            else return false;
        }
        /// <summary>
        /// Checks if any other player has the same color index to prevent color duplicates.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        private bool doesAnyOtherPlayerHaveThisColor(SSCEnums.PlayerID self)
        {
            for (int i = 0; i < 4; i++)
            {
                SSCEnums.PlayerID other = (SSCEnums.PlayerID)i;
                if (other == self) continue;
                if (this.doesOtherPlayerHaveThisColor(self, other) == false) continue;
                else return true;
            }
            return false;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (SeasideScramble.self.getGamepadState(PlayerIndex.One).IsButtonDown(Buttons.A))
            {
                //ModCore.log("Button a pressed. Intercepting left click.");
                return;
            }
            ModCore.log("Left clicked for Character selects screen!");
            this.initializeCharacter(SSCEnums.PlayerID.One);

            if (this.buttons["P1NextButton"].containsPoint(x, y))
            {
                this.iteratePlayerColorIndex(SSCEnums.PlayerID.One, 1);
            }
            if (this.buttons["P1PrevButton"].containsPoint(x, y))
            {
                this.iteratePlayerColorIndex(SSCEnums.PlayerID.One, -1);
            }
        }

        public void receiveGamepadLeftClick(SSCEnums.PlayerID player, int x, int y, bool playSound = true)
        {
            if (player == SSCEnums.PlayerID.One)
            {
                if (this.buttons["P1NextButton"].containsPoint(x, y))
                {
                    this.iteratePlayerColorIndex(player, 1);
                }
                if (this.buttons["P1PrevButton"].containsPoint(x, y))
                {
                    this.iteratePlayerColorIndex(player, -1);
                }
            }
            if (player == SSCEnums.PlayerID.Two)
            {
                if (this.buttons["P2NextButton"].containsPoint(x, y))
                {
                    this.iteratePlayerColorIndex(player, 1);
                }
                if (this.buttons["P2PrevButton"].containsPoint(x, y))
                {
                    this.iteratePlayerColorIndex(player, -1);
                }
            }
            if (player == SSCEnums.PlayerID.Three)
            {
                if (this.buttons["P3NextButton"].containsPoint(x, y))
                {
                    this.iteratePlayerColorIndex(player, 1);
                }
                if (this.buttons["P3PrevButton"].containsPoint(x, y))
                {
                    this.iteratePlayerColorIndex(player, -1);
                }
            }
            if (player == SSCEnums.PlayerID.Four)
            {
                if (this.buttons["P4NextButton"].containsPoint(x, y))
                {
                    this.iteratePlayerColorIndex(player, 1);
                }
                if (this.buttons["P4PrevButton"].containsPoint(x, y))
                {
                    this.iteratePlayerColorIndex(player, -1);
                }
            }
        }

        public void receiveGamepadLeftClick(SSCEnums.PlayerID player, Vector2 position, bool playSound = true)
        {
            this.receiveGamepadLeftClick(player, (int)position.X, (int)position.Y, playSound);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (key == Keys.Enter || key == Keys.Space)
            {
                if (SeasideScramble.self.currentNumberOfPlayers > 0)
                {
                    this.setUpForGameplay();
                    SeasideScramble.self.menuManager.closeAllMenus();
                    SeasideScramble.self.menuManager.addNewMenu(new Loby(0, 0, SeasideScramble.self.camera.viewport.Width, SeasideScramble.self.camera.viewport.Height));
                }
            }
            if (key == Keys.A)
            {
                this.iteratePlayerColorIndex(SSCEnums.PlayerID.One, -1);
            }
            if (key == Keys.D)
            {
                this.iteratePlayerColorIndex(SSCEnums.PlayerID.One, 1);
            }
        }

        /// <summary>
        /// Things to to before the game closes.
        /// </summary>
        private void setUpForGameplay()
        {

        }

        /// <summary>
        /// Checks if the menu is ready to close.
        /// </summary>
        /// <returns></returns>
        public override bool readyToClose()
        {
            if (this.closeMenu == true)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Draw everything to the screen.
        /// </summary>
        /// <param name="b"></param>
        public override void draw(SpriteBatch b)
        {
            //Draw background texture.
            b.Draw(this.background.texture, new Vector2(0, 0), SeasideScramble.self.camera.getXNARect(), Color.White);

            Vector2 menuTitlePos = Game1.dialogueFont.MeasureString(this.menuTitle);
            b.DrawString(Game1.dialogueFont, this.menuTitle, new Vector2((this.width / 2) - (menuTitlePos.X / 2), this.height / 10), Color.White);

            foreach (KeyValuePair<SSCEnums.PlayerID, Vector2> pair in this.playerDisplayLocations)
            {
                //Draw player 1
                this.drawDialogueBoxBackground((int)pair.Value.X - 50, (int)pair.Value.Y - 100, 200, 200, Color.Brown);
                if (SeasideScramble.self.getPlayer(pair.Key) != null)
                {
                    SeasideScramble.self.getPlayer(pair.Key).draw(b, pair.Value);
                }
            }

            this.animatedSprites["P1Click"].draw(b, 4f, 0f);
            this.animatedSprites["P1AButton"].draw(b, 2f, 0f);
            this.animatedSprites["P2AButton"].draw(b, 2f, 0f);
            this.animatedSprites["P3AButton"].draw(b, 2f, 0f);
            this.animatedSprites["P4AButton"].draw(b, 2f, 0f);

            /*
            foreach(Button button in this.buttons.Values)
            {
                button.draw(b,Color.White);
            }
            */

            if (SeasideScramble.self.getPlayer(SSCEnums.PlayerID.One) != null)
            {
                this.buttons["P1NextButton"].draw(b, Color.White);
                this.buttons["P1PrevButton"].draw(b, Color.White);
                this.animatedSprites["P1Color"].draw(b, 2f, 0f);
            }
            if (SeasideScramble.self.getPlayer(SSCEnums.PlayerID.Two) != null)
            {
                this.buttons["P2NextButton"].draw(b, Color.White);
                this.buttons["P2PrevButton"].draw(b, Color.White);
                this.animatedSprites["P2Color"].draw(b, 2f, 0f);
            }
            if (SeasideScramble.self.getPlayer(SSCEnums.PlayerID.Three) != null)
            {
                this.buttons["P3NextButton"].draw(b, Color.White);
                this.buttons["P3PrevButton"].draw(b, Color.White);
                this.animatedSprites["P3Color"].draw(b, 2f, 0f);
            }
            if (SeasideScramble.self.getPlayer(SSCEnums.PlayerID.Four) != null)
            {
                this.buttons["P4NextButton"].draw(b, Color.White);
                this.buttons["P4PrevButton"].draw(b, Color.White);
                this.animatedSprites["P4Color"].draw(b, 2f, 0f);
            }


            Vector2 p1Loc = Game1.dialogueFont.MeasureString("Player 1");
            b.DrawString(Game1.dialogueFont, "Player 1", new Vector2(this.playerDisplayLocations[SSCEnums.PlayerID.One].X - (p1Loc.X / 4), this.playerDisplayLocations[SSCEnums.PlayerID.One].Y - 64), Color.White);
            Vector2 p2Loc = Game1.dialogueFont.MeasureString("Player 2");
            b.DrawString(Game1.dialogueFont, "Player 2", new Vector2(this.playerDisplayLocations[SSCEnums.PlayerID.Two].X - (p2Loc.X / 4), this.playerDisplayLocations[SSCEnums.PlayerID.Two].Y - 64), Color.White);
            Vector2 p3Loc = Game1.dialogueFont.MeasureString("Player 3");
            b.DrawString(Game1.dialogueFont, "Player 3", new Vector2(this.playerDisplayLocations[SSCEnums.PlayerID.Three].X - (p3Loc.X / 4), this.playerDisplayLocations[SSCEnums.PlayerID.Three].Y - 64), Color.White);
            Vector2 p4Loc = Game1.dialogueFont.MeasureString("Player 4");
            b.DrawString(Game1.dialogueFont, "Player 4", new Vector2(this.playerDisplayLocations[SSCEnums.PlayerID.Four].X - (p4Loc.X / 4), this.playerDisplayLocations[SSCEnums.PlayerID.Four].Y - 64), Color.White);

            if (SeasideScramble.self.currentNumberOfPlayers >= 1)
            {
                Vector2 finish = Game1.dialogueFont.MeasureString("Press start or enter to start the game.");
                b.DrawString(Game1.dialogueFont, "Press start or enter to start the game.", new Vector2((this.width / 2) - (menuTitlePos.X / 2), this.height * .25f), Color.White);
            }

            if (SeasideScramble.self.getPlayer(SSCEnums.PlayerID.One) != null)
            {
                SeasideScramble.self.getPlayer(SSCEnums.PlayerID.One).drawMouse(b);
            }
            if (SeasideScramble.self.getPlayer(SSCEnums.PlayerID.Two) != null)
            {
                SeasideScramble.self.getPlayer(SSCEnums.PlayerID.Two).drawMouse(b);
            }
            if (SeasideScramble.self.getPlayer(SSCEnums.PlayerID.Three) != null)
            {
                SeasideScramble.self.getPlayer(SSCEnums.PlayerID.Three).drawMouse(b);
            }
            if (SeasideScramble.self.getPlayer(SSCEnums.PlayerID.Four) != null)
            {
                SeasideScramble.self.getPlayer(SSCEnums.PlayerID.Four).drawMouse(b);
            }
        }

        /// <summary>
        /// Exit the menu.
        /// </summary>
        /// <param name="playSound"></param>
        public override void exitMenu(bool playSound = true)
        {
            base.exitMenu(playSound);
        }

    }
}
