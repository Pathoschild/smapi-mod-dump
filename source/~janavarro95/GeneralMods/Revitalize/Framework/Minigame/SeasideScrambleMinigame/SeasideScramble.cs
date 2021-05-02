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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Revitalize.Framework.Minigame.SeasideScrambleMinigame;
using StardewValley;
using StardustCore.UIUtilities;
namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame
{
    /// <summary>
    /// TODO: Finish character select screen.
    ///     -Make Sound effects happen
    ///     -make prompt for color selection
    ///         -a,d for keyboard
    ///         -dpad for p2-4
    /// Also add interface for game entity for camera to consistently have a focus target.
    /// -Make moving target enemies
    ///     -Make a sound effect play when they break
    /// -Fix positioning of status effects on player HUD
    /// -Add effect on player for a more visual representation of status effects.
    ///
    /// -Make shooting gallary game mode
    /// -Make shooting gallery map wider.
    /// -Add in score system for shooting gallery mode.
    /// 
    /// -Make More guns
    /// </summary>
    public class SeasideScramble : StardewValley.Minigames.IMinigame
    {
        /// <summary>
        /// A static reference to the game.
        /// </summary>
        public static SeasideScramble self;

        public SeasideScrambleMap currentMap;
        public Dictionary<string, SeasideScrambleMap> SeasideScrambleMaps;

        public int currentNumberOfPlayers
        {
            get
            {
                return this.players.Count;
            }
        }
        public const int maxPlayers = 4;
        public Dictionary<SSCEnums.PlayerID, SSCPlayer> players;

        public bool quitGame;
        public Vector2 topLeftScreenCoordinate;

        public SSCTextureUtilities textureUtils;

        public SSCCamera camera;

        public SSCMenus.SSCMenuManager menuManager;

        public Vector2 oldMousePosition;

        public SSCEntities.SSCEntityManager entities;
        public SSCGuns.SSCGunManager guns;

        public SSCFonts.SSCFont gameFont;

        /// <summary>
        /// The current game mode.
        /// </summary>
        public SSCEnums.SSCGameMode gameMode;
        public bool friendlyFireEnabled;

        /// <summary>
        /// RNG.
        /// </summary>
        public Random random
        {
            get
            {
                return Game1.random;
            }
        }

        /// <summary>
        /// Constuctor.
        /// </summary>
        public SeasideScramble()
        {
            self = this;
            this.camera = new SSCCamera();
            //this.viewport = new xTile.Dimensions.Rectangle(StardewValley.Game1.viewport);
            this.topLeftScreenCoordinate = new Vector2((float)(this.camera.viewport.Width / 2 - 384), (float)(this.camera.viewport.Height / 2 - 384));

            this.LoadTextures();

            this.entities = new SSCEntities.SSCEntityManager();

            this.LoadMaps();
            this.loadStartingMap();
            this.quitGame = false;

            this.players = new Dictionary<SSCEnums.PlayerID, SSCPlayer>();
            //this.players.Add(SSCEnums.PlayerID.One, new SSCPlayer(SSCEnums.PlayerID.One));
            //this.getPlayer(SSCEnums.PlayerID.One).setColor(Color.PaleVioletRed);


            this.menuManager = new SSCMenus.SSCMenuManager();

            this.menuManager.addNewMenu(new SSCMenus.TitleScreen(this.camera.viewport));
            this.oldMousePosition = new Vector2(Game1.getMousePosition().X, Game1.getMousePosition().Y);

            this.gameFont = new SSCFonts.SSCFont(new SSCFonts.SSCFontCharacterSheet());
            this.guns = new SSCGuns.SSCGunManager();
        }

        public SSCPlayer getPlayer(SSCEnums.PlayerID id)
        {
            if (this.players.ContainsKey(id))
            {
                return this.players[id];
            }
            else return null;
        }

        /// <summary>
        /// Loads in all of the necessary textures for Seaside Scramble.
        /// </summary>
        private void LoadTextures()
        {
            this.textureUtils = new SSCTextureUtilities();
            TextureManager playerManager = new TextureManager("SSCPlayer");
            playerManager.searchForTextures(ModCore.ModHelper, ModCore.Manifest, Path.Combine("Content", "Minigames", "SeasideScramble", "Graphics", "Player"));
            TextureManager mapTextureManager = new TextureManager("SSCMaps");
            mapTextureManager.searchForTextures(ModCore.ModHelper, ModCore.Manifest, Path.Combine("Content", "Minigames", "SeasideScramble", "Maps", "Backgrounds"));
            TextureManager UIManager = new TextureManager("SSCUI");
            UIManager.searchForTextures(ModCore.ModHelper, ModCore.Manifest, Path.Combine("Content", "Minigames", "SeasideScramble", "Graphics", "UI"));
            TextureManager projectileManager = new TextureManager("Projectiles");
            projectileManager.searchForTextures(ModCore.ModHelper, ModCore.Manifest, Path.Combine("Content", "Minigames", "SeasideScramble", "Graphics", "Projectiles"));
            TextureManager gunManager = new TextureManager("Guns");
            gunManager.searchForTextures(ModCore.ModHelper, ModCore.Manifest, Path.Combine("Content", "Minigames", "SeasideScramble", "Graphics", "Guns"));
            TextureManager enemies = new TextureManager("Enemies");
            enemies.searchForTextures(ModCore.ModHelper, ModCore.Manifest, Path.Combine("Content", "Minigames", "SeasideScramble", "Graphics", "Enemies"));

            this.textureUtils.addTextureManager(playerManager);
            this.textureUtils.addTextureManager(mapTextureManager);
            this.textureUtils.addTextureManager(UIManager);
            this.textureUtils.addTextureManager(projectileManager);
            this.textureUtils.addTextureManager(gunManager);
            this.textureUtils.addTextureManager(enemies);
        }

        /// <summary>
        /// Loads in all of the maps for Seaside Scramble.
        /// </summary>
        private void LoadMaps()
        {
            this.SeasideScrambleMaps = new Dictionary<string, SeasideScrambleMap>();
            this.SeasideScrambleMaps.Add("TestRoom", new SeasideScrambleMap(SeasideScrambleMap.LoadMap("TestRoom.tbin").Value));
            this.SeasideScrambleMaps.Add("ShootingGallery", new SSCMaps.ShootingGallery(SeasideScrambleMap.LoadMap("ShootingGallery.tbin").Value));
        }
        /// <summary>
        /// Loads in a default map for Seaside Scramble.
        /// </summary>
        private void loadStartingMap()
        {
            this.currentMap = this.SeasideScrambleMaps["ShootingGallery"];
        }

        /// <summary>
        /// What happens when the screen changes size.
        /// </summary>
        public void changeScreenSize()
        {
            Viewport viewport = StardewValley.Game1.graphics.GraphicsDevice.Viewport;
            double num1 = (double)(viewport.Width / 2 - 384);
            viewport = StardewValley.Game1.graphics.GraphicsDevice.Viewport;
            double num2 = (double)(viewport.Height / 2 - 384);
            this.topLeftScreenCoordinate = new Vector2((float)num1, (float)num2);
            this.camera.viewport = new xTile.Dimensions.Rectangle(StardewValley.Game1.viewport);
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Used to update Stardew Valley while this minigame runs. True means SDV updates false means the SDV pauses all update ticks.
        /// </summary>
        /// <returns></returns>
        public bool doMainGameUpdates()
        {
            return false;
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Draws all game aspects to the screen.
        /// </summary>
        /// <param name="b"></param>
        public void draw(SpriteBatch b)
        {
            
            if (this.currentMap != null)
            {
                this.currentMap.draw(b);
            }
            
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null);

            foreach(SSCPlayer p in this.players.Values) {
                p.draw(b);

            }

            /*
            if (this.menuManager.activeMenu != null)
            {
                this.menuManager.activeMenu.draw(b);
            }
            */
            foreach (SSCPlayer p in this.players.Values)
            {
                p.drawHUD(b);
                if (p.playerID == SSCEnums.PlayerID.One)
                {
                    p.drawMouse(b);
                }
            }
            this.entities.draw(b);

            this.menuManager.drawAll(b);

            b.End();
        }



        /// <summary>
        /// The id of the minigame???
        /// </summary>
        /// <returns></returns>
        public string minigameId()
        {
            return "Seaside Scramble Stardew Lite Edition";
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the game mode for the game.
        /// </summary>
        /// <param name="Mode"></param>
        public void setMode(SSCEnums.SSCGameMode Mode)
        {
            if (Mode == SSCEnums.SSCGameMode.None)
            {
                this.friendlyFireEnabled = false;
            }
            if(Mode== SSCEnums.SSCGameMode.ShootingGallery)
            {
                this.friendlyFireEnabled = true;
            }

            if(Mode== SSCEnums.SSCGameMode.PVP)
            {
                this.friendlyFireEnabled = true;
            }
            if(Mode== SSCEnums.SSCGameMode.Story)
            {
                this.friendlyFireEnabled = false;
            }

            this.gameMode = Mode;
        }

        //~~~~~~~~~~~~~~~~~//
        //   Input Logic   //
        //~~~~~~~~~~~~~~~~~//
        #region

        /// <summary>
        /// What happens when the left click is held.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void leftClickHeld(int x, int y)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Does this override free mous emovements?
        /// </summary>
        /// <returns></returns>
        public bool overrideFreeMouseMovement()
        {
            return true;
            //throw new NotImplementedException();
        }

        /// <summary>
        /// ??? Undocumended.
        /// </summary>
        /// <param name="data"></param>
        public void receiveEventPoke(int data)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// What happens when a key is pressed.
        /// </summary>
        /// <param name="k"></param>
        public void receiveKeyPress(Keys k)
        {
            //throw new NotImplementedException();
            if (k == Keys.Escape)
            {
                this.quitGame = true;
            }

            foreach(SSCPlayer player in this.players.Values)
            {
                player.receiveKeyPress(k);
            }

            if (this.menuManager.isMenuUp)
            {
                this.menuManager.activeMenu.receiveKeyPress(k);
            }

        }

        /// <summary>
        /// Gets a gamepad state.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public GamePadState getGamepadState(PlayerIndex index)
        {
            return Microsoft.Xna.Framework.Input.GamePad.GetState(index);
        }

        /// <summary>
        /// Called when the minigame registeres a key on the keyboard being released.
        /// </summary>
        /// <param name="K"></param>
        public void receiveKeyRelease(Keys K)
        {
            foreach (SSCPlayer player in this.players.Values)
            {
                player.receiveKeyRelease(K);
            }
        }

        /// <summary>
        /// Called when the minigame receives a left click.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="playSound"></param>
        public void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.menuManager.activeMenu != null)
            {
                this.menuManager.activeMenu.receiveLeftClick(x, y, playSound);
            }
            foreach(SSCPlayer player in this.players.Values)
            {
                player.receiveLeftClick(x, y);
            }
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Called when the minigame receives a right click.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="playSound"></param>
        public void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (this.menuManager.activeMenu != null)
            {
                this.menuManager.activeMenu.receiveRightClick(x, y, playSound);
            }
        }

        /// <summary>
        /// What happens when left click is released.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void releaseLeftClick(int x, int y)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// What happens when right click is released.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void releaseRightClick(int x, int y)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Receive input from a specific gamepad.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="ID"></param>
        private void receiveGamepadInput(GamePadState state,SSCEnums.PlayerID ID)
        {
            if (state == null) return;
            else
            {
                if (this.players.ContainsKey(ID))
                {
                    this.players[ID].receiveGamepadInput(state);
                }
            }
        }

        /// <summary>
        /// Returns the delta for mouse movement.
        /// </summary>
        /// <returns></returns>
        public Vector2 getMouseDelta()
        {
            Vector2 ret = -1 * (this.oldMousePosition - new Vector2(Game1.getMousePosition().X, Game1.getMousePosition().Y));
            return ret;
        }

        #endregion



        /// <summary>
        /// Called every update frame.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool tick(GameTime time)
        {
            KeyboardState kState = Keyboard.GetState();

            foreach (Keys k in kState.GetPressedKeys())
            {
                this.receiveKeyPress(k);
            }
            for (int i = 0; i < 4; i++)
            {
                GamePadState state = this.getGamepadState((PlayerIndex)i);
                this.receiveGamepadInput(state,(SSCEnums.PlayerID)i);
            }
            


            if (this.quitGame)
            {
                return true;
            }
            if (this.currentMap != null)
            {
                this.currentMap.update(time);
            }


            if (this.menuManager.activeMenu != null)
            {
                this.menuManager.activeMenu.update(time);
                if (this.menuManager.activeMenu.readyToClose())
                {
                    this.menuManager.closeActiveMenu();
                }
                foreach (SSCPlayer player in this.players.Values)
                {
                    player.update(time);
                }
            }
            else
            {
                foreach (SSCPlayer player in this.players.Values)
                {
                    if (player.playerID == SSCEnums.PlayerID.One) this.camera.centerOnPosition(player.position);
                    player.update(time);
                }
                this.entities.update(time);
            }

            

            this.oldMousePosition = new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY());
            return false;
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Called when the minigame is quit upon.
        /// </summary>
        public void unload()
        {
            //throw new NotImplementedException();
            ModCore.log("Exit the game!");
        }


        //~~~~~~~~~~~~~~~~~~~~//
        //  Static Functions  //
        //~~~~~~~~~~~~~~~~~~~~//

        #region

        /// <summary>
        /// Translates the position passed in into the relative position on the viewport.
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="globalPosition"></param>
        /// <returns></returns>
        public static Vector2 GlobalToLocal(xTile.Dimensions.Rectangle viewport, Vector2 globalPosition)
        {
            return new Vector2(globalPosition.X - (float)viewport.X, globalPosition.Y - (float)viewport.Y);
        }

        /// <summary>
        /// Translates the position passed in into the relative position on the viewport.
        /// </summary>
        /// <param name="globalPosition"></param>
        /// <returns></returns>
        public static Vector2 GlobalToLocal(Vector2 globalPosition)
        {
            return new Vector2(globalPosition.X - (float)SeasideScramble.self.camera.viewport.X, globalPosition.Y - (float)SeasideScramble.self.camera.viewport.Y);
        }

        public bool forceQuit()
        {
            this.quitGame = true;
            return true;
        }

        #endregion
    }
}
