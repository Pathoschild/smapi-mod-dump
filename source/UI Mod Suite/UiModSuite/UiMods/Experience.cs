using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using UiModSuite.Options;


namespace UiModSuite.UiMods {

    //TODO This class is not efficient at all and is really really messy... but functional... so if have time cleanup!

    /* Experience point indexes
     * 
     * Farming = 0
     * Fishing = 1
     * Foraging = 2
     * Mining = 3
     * Combat = 4
     * Luck = 5 
    */
    
    class Experience {

        private int maxBarWidth = 175;

        private int currentLevelIndex = 4;
        private int levelOfCurrentlyDisplayedExp = 0;
        float currentExp = 0;

        List<ExpPointDisplay> expPointDisplays = new List<ExpPointDisplay>();
        GameLocation currentLocation = new GameLocation();

        private const int TIME_BEFORE_EXPERIENCE_BAR_FADE = 8000;
        private int lengthOfLevelUpPause = 2000;

        // New colors created to allow manipulation here
        Color iconColor =  new Color( Color.White.ToVector4() );
        Color expFillColor = new Color( Color.Azure.ToVector4() );        

        Item previousItem = null;

        private bool shouldDrawExperienceBar = false;
        private bool shouldDrawLevelUp = false;
        ClickableTextureComponent test;

        SoundEffectInstance se;
        Timer timerToDissapear = new Timer();

        Rectangle levelUpIconRectangle;

        /// <summary>
        /// This mod  shows an experienceBar, experience gained and plays an animation on level up
        /// </summary>
        public Experience() {

            GraphicsEvents.OnPreRenderHudEvent += onPreRenderEvent;
            LocationEvents.CurrentLocationChanged += removeAllExpPointDisplays;

            try {
                Stream soundfile = TitleContainer.OpenStream( @"Mods\\UiModSuite\\LevelUp.wav" );
                SoundEffect soundEffect = SoundEffect.FromStream( soundfile );
                se = soundEffect.CreateInstance();
                se.Volume = 1;
            } catch( FileNotFoundException e ) {
                ModEntry.Log( "Failed to load LevelUp.wav, please contact Demiacle! \n" + e.StackTrace );
            }
            
            timerToDissapear.Elapsed += stopTimerAndFadeBarOut;
        }

        /// <summary>
        /// Removes any dangling exp displays when changing locations
        /// </summary>
        private void removeAllExpPointDisplays( object sender, EventArgsCurrentLocationChanged e ) {
            expPointDisplays.Clear();
        }

        /// <summary>
        /// Toggles the level up animation on or off
        /// </summary>
        public void togglLevelUpAnimation() {

            PlayerEvents.LeveledUp -= onLevelUp;

            if( ModOptionsPage.getCheckboxValue( ModOptionsPage.Setting.SHOW_LEVEL_UP_ANIMATION ) ) {
                PlayerEvents.LeveledUp += onLevelUp;
                
            }
        }

        /// <summary>
        /// Stop displaying the exp bar
        /// </summary>
        private void stopTimerAndFadeBarOut( object sender, ElapsedEventArgs e ) {
            timerToDissapear.Stop();
            shouldDrawExperienceBar = false;
        }

        /// <summary>
        /// Draw all experience displays
        /// </summary>
        internal void onPreRenderEvent( object sender, EventArgs e ) {
            if( Game1.eventUp ) {
                return;
            }

            Item currentItem = Game1.player.CurrentItem;

            var spriteRectangle = new Rectangle( 10, 428, 10, 10 );
            int currentLevel = 0;

            // Display exp type depending on item
            if( currentItem is FishingRod ) {
                currentLevelIndex = 1;
                spriteRectangle = new Rectangle( 20, 428, 10, 10 );
                currentLevel = Game1.player.fishingLevel;

            } else if( currentItem is Axe ) {
                currentLevelIndex = 2;
                spriteRectangle = new Rectangle( 60, 428, 10, 10 );
                currentLevel = Game1.player.foragingLevel;

            } else if( currentItem is Pickaxe ) {
                currentLevelIndex = 3;
                spriteRectangle = new Rectangle( 30, 428, 10, 10 );
                currentLevel = Game1.player.miningLevel;

            } else if( currentItem is MeleeWeapon && currentItem.Name != "Scythe" ) {
                currentLevelIndex = 4;
                spriteRectangle = new Rectangle( 120, 428, 10, 10 );
                currentLevel = Game1.player.combatLevel;


            // If primary item is not selected
            // Display farming exp or foraging exp depending on current location
            } else {

                if( Game1.currentLocation is Farm) {
                    currentLevelIndex = 0;
                    spriteRectangle = new Rectangle( 10, 428, 10, 10 );
                    currentLevel = Game1.player.farmingLevel;
                } else {
                    currentLevelIndex = 2;
                    spriteRectangle = new Rectangle( 60, 428, 10, 10 );
                    currentLevel = Game1.player.foragingLevel;
                }
            }

            levelOfCurrentlyDisplayedExp = currentLevel;

            if( levelOfCurrentlyDisplayedExp > 9 ) {
                return;
            }
            
            // Sets the exp for next level and the exp that has already been obtained based on current level
            int expRequiredToLevel = getExperienceRequiredToLevel( levelOfCurrentlyDisplayedExp );
            int expAlreadyEarnedFromPreviousLevels = getExperienceGainedFromPreviousLevels( levelOfCurrentlyDisplayedExp );

            float nextExp = Game1.player.experiencePoints[ currentLevelIndex ] - expAlreadyEarnedFromPreviousLevels;

            // Show experience bar if item has changed
            if( previousItem != currentItem ) {
                displayExperienceBar();

            // Show experience bar and exp gain if exp is gained
            } else if( currentExp != nextExp ) {
                displayExperienceBar();

                if( ModOptionsPage.getCheckboxValue( ModOptionsPage.Setting.SHOW_EXP_GAIN ) == true && ( nextExp - currentExp ) > 0 ) {
                    expPointDisplays.Add( new ExpPointDisplay( nextExp - currentExp, Game1.player.getLocalPosition( Game1.viewport ) ) );
                }

            }

            previousItem = currentItem;
            currentExp = nextExp;

            if( ModOptionsPage.getCheckboxValue( ModOptionsPage.Setting.SHOW_EXPERIENCE_BAR ) == false || shouldDrawExperienceBar == false  || levelOfCurrentlyDisplayedExp == 10 ) {
                return;
            }

            int barWidth = Convert.ToInt32( ( currentExp / ( expRequiredToLevel - expAlreadyEarnedFromPreviousLevels ) ) * maxBarWidth );
            float positionX = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Left;

            // Shift display if game view has black borders
            if( Game1.isOutdoorMapSmallerThanViewport() ) {
                int currentMapSize = ( Game1.currentLocation.map.Layers[ 0 ].LayerWidth * Game1.tileSize );
                float blackSpace = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - currentMapSize;
                positionX = positionX + ( blackSpace / 2 );
            }

            // Border
            Game1.drawDialogueBox( (int) positionX, Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 160, 240, 160, false, true );

            // Experience fill
            Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( (int) positionX + 32, Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 63, barWidth, 31 ), expFillColor );
            Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( (int) positionX + 32, Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 63, Math.Min( 4, barWidth), 31 ), Color.LightGray );
            Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( (int) positionX + 32, Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 63, barWidth, 4 ), Color.LightGray );
            Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( (int) positionX + 32, Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 36, barWidth, 4 ), Color.LightGray );

            
            // Hacky way to handle a mouseover
            // Draw experience gained this level and amount needed for next level
            test = new ClickableTextureComponent( "", new Rectangle( ( int ) positionX - 36, Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 80, 260, 100 ), "", "", Game1.mouseCursors, new Rectangle( 0, 0, 0, 0 ), Game1.pixelZoom );
            if( test.containsPoint( Game1.getMouseX(), Game1.getMouseY() ) ) {
                Game1.drawWithBorder( $"{currentExp}/{  expRequiredToLevel - expAlreadyEarnedFromPreviousLevels}", Color.Black, Color.White, new Vector2( positionX + 33, Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 66 ) );
            
            // Draw Level and icon
            } else {
                // Icon
                Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( ( int ) positionX + 54, Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 62 ), spriteRectangle, iconColor, 0.0f, Vector2.Zero, 2.9f, SpriteEffects.None, 0.85f );

                // Draw current Level
                Game1.drawWithBorder( $"{currentLevel}", Color.Black * 0.6f, Color.Azure, new Vector2( positionX + 33, Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 66 ) );
            }

            // Level Up display
            if( shouldDrawLevelUp) {

                // Icon
                Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( Game1.player.getLocalPosition( Game1.viewport ).X - 74, Game1.player.getLocalPosition( Game1.viewport ).Y - 130 ), levelUpIconRectangle, iconColor, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, SpriteEffects.None, 0.85f );
                // Text
                Game1.drawWithBorder( "Level Up", Color.DarkSlateGray, Color.PaleTurquoise, new Vector2( Game1.player.getLocalPosition( Game1.viewport ).X - 28, Game1.player.getLocalPosition( Game1.viewport ).Y - 130 ) );
            }

            foreach( ExpPointDisplay experienceDisplay in expPointDisplays ) {
                experienceDisplay.draw();
            }

            for( int i = expPointDisplays.Count - 1; i >= 0; i-- ) {
                if( expPointDisplays[i].isInvisible() ) {
                    expPointDisplays.RemoveAt( i );
                } else {
                    expPointDisplays[ i ].draw();
                }

            }
        }

        /// <summary>
        /// Gets the required exp needed per level
        /// </summary>
        /// <param name="levelOfCurrentlyDisplayedExp">The level</param>
        /// <returns>Amount needed for next level</returns>
        private int getExperienceRequiredToLevel( int levelOfCurrentlyDisplayedExp ) {
            switch( levelOfCurrentlyDisplayedExp ) {
                case 0:
                    return 100;
                case 1:
                    return 380;
                case 2:
                    return 770;
                case 3:
                    return 1300;
                case 4:
                    return 2150;
                case 5:
                    return 3300;
                case 6:
                    return 4800;
                case 7:
                    return 6900;
                case 8:
                    return 10000;
                case 9:
                    return 15000;
                default:
                // Max level or bug so disable showing exp
                case 10:
                    return 0;
            }; 
        }

        /// <summary>
        /// Displays the exp bar
        /// </summary>
        private void displayExperienceBar() {
            if( ModOptionsPage.getCheckboxValue( ModOptionsPage.Setting.ALLOW_EXPERIENCE_BAR_TO_FADE_OUT ) == true ) {
                timerToDissapear.Interval = TIME_BEFORE_EXPERIENCE_BAR_FADE;
                timerToDissapear.Start();
                shouldDrawExperienceBar = true;
            } else {
                timerToDissapear.Stop();
                shouldDrawExperienceBar = true;
            }
        }

        /// <summary>
        /// Pauses the game, shows Level Up text and plays a chime, and unpauses after some time;
        /// </summary>
        internal void onLevelUp( object sender, EventArgsLevelUp e ) {

            // Fix firing on new day. Exp can never be gained in FarmHouse
            if( Game1.currentLocation.name == "FarmHouse" ) {
                return;
            }

            if( ModOptionsPage.getCheckboxValue( ModOptionsPage.Setting.SHOW_LEVEL_UP_ANIMATION ) == false ) {
                return;
            }
            
            switch( e.Type ) {
                case EventArgsLevelUp.LevelType.Combat:
                    levelUpIconRectangle = new Rectangle( 120, 428, 10, 10 );
                    break;
                case EventArgsLevelUp.LevelType.Farming:
                    levelUpIconRectangle = new Rectangle( 10, 428, 10, 10 );
                    break;
                case EventArgsLevelUp.LevelType.Fishing:
                    levelUpIconRectangle = new Rectangle( 20, 428, 10, 10 );
                    break;
                case EventArgsLevelUp.LevelType.Foraging:
                    levelUpIconRectangle = new Rectangle( 60, 428, 10, 10 );
                    break;
                case EventArgsLevelUp.LevelType.Mining:
                    levelUpIconRectangle = new Rectangle( 30, 428, 10, 10 );
                    break;
            }

            shouldDrawLevelUp = true;

            timerToDissapear.Interval = TIME_BEFORE_EXPERIENCE_BAR_FADE;
            timerToDissapear.Start();
            shouldDrawExperienceBar = true;


            float prevAmbientVolume = Game1.options.ambientVolumeLevel;
            float prevMusicVolume = Game1.options.musicVolumeLevel;

            // Play sound if loaded
            if( se != null ) {
                if( prevMusicVolume > 0.01f ) {
                    se.Volume = Math.Min( 1, ( prevMusicVolume + 0.3f ) );
                } else {
                    se.Volume = 0;
                }
            }

            Task.Factory.StartNew( () => {
                System.Threading.Thread.Sleep( 100 );

                Game1.musicCategory.SetVolume( Math.Max( 0, Game1.options.musicVolumeLevel - 0.3f ) );
                Game1.ambientCategory.SetVolume( Math.Max( 0, Game1.options.ambientVolumeLevel - 0.3f ) );

                if( se != null ) {
                    se.Play();
                }
            } );

            Task.Factory.StartNew( () => {
                System.Threading.Thread.Sleep( lengthOfLevelUpPause );
                shouldDrawLevelUp = false;


                Game1.musicCategory.SetVolume( prevMusicVolume );
                Game1.ambientCategory.SetVolume( prevAmbientVolume );
            } );
          
        }

        /// <summary>
        /// Finds the exp gained for the current level
        /// </summary>
        /// <param name="currentLevel">Players level</param>
        /// <returns>The exp already earned towards current level</returns>
        private int getExperienceGainedFromPreviousLevels( int currentLevel ) {

            int expAlreadyEarnedFromPreviousLevels = 0;

            switch( currentLevel ) {
                case 0:
                    expAlreadyEarnedFromPreviousLevels = 0;
                    break;
                case 1:
                    expAlreadyEarnedFromPreviousLevels = 100;
                    break;
                case 2:
                    expAlreadyEarnedFromPreviousLevels = 380;
                    break;
                case 3:
                    expAlreadyEarnedFromPreviousLevels = 770;
                    break;
                case 4:
                    expAlreadyEarnedFromPreviousLevels = 1300;
                    break;
                case 5:
                    expAlreadyEarnedFromPreviousLevels = 2150;
                    break;
                case 6:
                    expAlreadyEarnedFromPreviousLevels = 3300;
                    break;
                case 7:
                    expAlreadyEarnedFromPreviousLevels = 4800;
                    break;
                case 8:
                    expAlreadyEarnedFromPreviousLevels = 6900;
                    break;
                case 9:
                    expAlreadyEarnedFromPreviousLevels = 10000;
                    break;

                default:
                // Max level or bug so disable showing exp
                case 10:
                    return 0;
            }

            return expAlreadyEarnedFromPreviousLevels;
        }

    }
}
