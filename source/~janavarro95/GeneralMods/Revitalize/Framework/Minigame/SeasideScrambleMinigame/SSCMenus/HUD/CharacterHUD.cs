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
using Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCMaps;
using StardustCore.Animations;
using StardustCore.UIUtilities;
using StardustCore.UIUtilities.SpriteFonts.Components;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCMenus.HUD
{
    public class CharacterHUD : IClickableMenuExtended
    {
        public AnimatedSprite background;
        public SSCEnums.PlayerID playerID;
        public bool showHUD;


        public AnimatedSprite heart;
        public TexturedString playerHealth;
        public bool showFullHeart;
        public int remainingHealthLerpFrames;
        public int framesToUpdate = 5;

        public AnimatedSprite gun;
        public TexturedString playerAmmo;

        public AnimatedSprite clock;
        public TexturedString reloadTime;

        public AnimatedSprite targetsHit;
        public TexturedString targetsScoreText;
        

        public SSCPlayer Player
        {
            get
            {
                return SeasideScramble.self.getPlayer(this.playerID);
            }
        }

        public CharacterHUD()
        {

        }

        public CharacterHUD(int x, int y, int width, int height, SSCEnums.PlayerID Player) : base(x, y, width, height, false)
        {
            this.background = new AnimatedSprite("Background", new Vector2(x, y), new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "DialogBox"), new Animation(0, 0, 32, 32)), Color.White);
            this.playerID = Player;
            this.showHUD = false;

            this.heart = new AnimatedSprite("Heart", new Vector2(x + 32, y + 10), new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "Heart"), new Animation(0, 0, 7, 6), new Dictionary<string, List<Animation>>() {

                {"Full",new List<Animation>(){
                    new Animation(0,0,7,6)
                }},
                {"Empty",new List<Animation>(){
                    new Animation(7,0,7,6)
                }}
            }, "Full"), Color.White);
            this.heart.animation.playAnimation("Full");
            this.playerHealth = SeasideScramble.self.gameFont.ParseString("100", new Vector2(100, this.yPositionOnScreen + 10), Color.White, true, 2f);
            this.playerHealth.setPosition(new Vector2(this.xPositionOnScreen + 100, this.yPositionOnScreen + 10));
            this.showFullHeart = true;

            this.gun = new AnimatedSprite("Gun", new Vector2(x + 32, y + 50), new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("Guns", "BasicGun"), new Animation(0, 0, 16, 16)), Color.White);
            this.playerAmmo = SeasideScramble.self.gameFont.ParseString("100", new Vector2(100, this.yPositionOnScreen + 50), Color.White, true, 2f);
            this.playerAmmo.setPosition(new Vector2(this.xPositionOnScreen + 100, this.yPositionOnScreen + 50));

            this.clock = new AnimatedSprite("Clock", new Vector2(x + 32, y + 50), new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "Clock"), new Animation(0, 0, 11, 10)), Color.White);
            this.reloadTime = SeasideScramble.self.gameFont.ParseString("100", new Vector2(100, this.yPositionOnScreen + 50), Color.White, true, 2f);
            this.reloadTime.setPosition(new Vector2(this.xPositionOnScreen + 100, this.yPositionOnScreen + 50));

            this.targetsHit = new AnimatedSprite("Target", new Vector2(x + 32, y + 100), new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("Enemies", "Target"), new Animation(0, 0, 16, 16)), Color.White);
            this.targetsScoreText = SeasideScramble.self.gameFont.ParseString("000", new Vector2(100, this.yPositionOnScreen + 110), Color.White, true, 2f);
            this.targetsScoreText.setPosition(new Vector2(this.xPositionOnScreen + 100, this.yPositionOnScreen + 110));
        }

        public override void update(GameTime time)
        {
            if (this.showHUD == false) return;
            if (SeasideScramble.self.getPlayer(this.playerID) != null)
            {
                this.background.color = SeasideScramble.self.getPlayer(this.playerID).playerColor;
                this.healthDisplayLerp();
                if (this.Player.activeGun.remainingAmmo == SSCGuns.SSCGun.infiniteAmmo)
                {
                    this.playerAmmo.setText("999", SeasideScramble.self.gameFont, Color.White);
                }
                else
                {
                    this.playerAmmo.setText(this.Player.activeGun.remainingAmmo.ToString().PadLeft(3, '0'), SeasideScramble.self.gameFont, Color.White);
                }
                this.reloadTime.setText(((int)this.Player.activeGun.timeRemainingUntilReload).ToString().PadLeft(4, '0'), SeasideScramble.self.gameFont, Color.White);

                if(SeasideScramble.self.currentMap is ShootingGallery)
                {
                    this.targetsScoreText.setText((SeasideScramble.self.currentMap as ShootingGallery).score[this.playerID].ToString(), SeasideScramble.self.gameFont, this.background.color);
                }

            }
            if (this.showFullHeart)
            {
                this.heart.animation.playAnimation("Full");
            }
            else
            {
                this.heart.animation.playAnimation("Empty");
            }
        }

        /// <summary>
        /// Has a small counting lerp for display text.
        /// </summary>
        private void healthDisplayLerp()
        {
            if (this.remainingHealthLerpFrames == 0)
            {
                this.remainingHealthLerpFrames = this.framesToUpdate;
                if (Convert.ToInt32(this.playerHealth.getText()) != this.Player.currentHealth)
                {
                    this.showFullHeart = !this.showFullHeart;
                    int health = Convert.ToInt32(this.playerHealth.getText());
                    health = health - 1;
                    string healthStr = health.ToString();
                    healthStr = healthStr.PadLeft(3, '0');
                    this.playerHealth.setText(healthStr, SeasideScramble.self.gameFont, Color.White);
                }
                else
                {
                    this.showFullHeart = true;
                }
            }
            else
            {
                this.remainingHealthLerpFrames--;
            }
        }

        /// <summary>
        /// Draw the HUD to the screen.
        /// </summary>
        /// <param name="b"></param>
        public override void draw(SpriteBatch b)
        {
            if (this.showHUD == false) return;
            //Draw the HUD background.
            //b.Draw(this.background.texture, new Vector2(100, 100), SeasideScramble.self.camera.getXNARect(), SeasideScramble.self.players[this.playerID].playerColor, 0f, Vector2.Zero, new Vector2(4f, 2f), SpriteEffects.None, 0f);
            this.background.draw(b, this.background.position, new Vector2(8f, 4f), 0f);
            this.playerHealth.draw(b, new Rectangle(0, 0, 16, 16), 0f);
            this.heart.draw(b, 8f, 0f);
            if (this.Player.activeGun.isReloading == false)
            {
                this.gun.draw(b, 4f, 0f);
                this.playerAmmo.draw(b, new Rectangle(0, 0, 16, 16), 0f);
            }
            else
            {
                this.clock.draw(b, 6f, 0f);
                this.reloadTime.draw(b, new Rectangle(0, 0, 16, 16), 0f);
            }
            if(SeasideScramble.self.currentMap is ShootingGallery)
            {
                this.targetsHit.draw(b,4f,0f);
                this.targetsScoreText.draw(b, new Rectangle(0, 0, 16, 16), 0f);
            }
        }

        /// <summary>
        /// Display the HUD.
        /// </summary>
        public void displayHUD()
        {
            this.playerHealth.setText(SeasideScramble.self.getPlayer(this.playerID).currentHealth.ToString(), SeasideScramble.self.gameFont, Color.White);
            this.showHUD = true;
            this.gun.animation = this.Player.activeGun.sprite.animation;
        }

        /// <summary>
        /// Hide the HUD.
        /// </summary>
        public void hideHUD()
        {
            this.showHUD = false;
        }

    }
}
