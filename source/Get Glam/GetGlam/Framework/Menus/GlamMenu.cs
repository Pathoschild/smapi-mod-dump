/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MartyrPher/GetGlam
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace GetGlam.Framework.Menus
{
    /// <summary>
    /// Class that draws the custom menu and allows the player to change appearance.
    /// <summary>
    public class GlamMenu : IClickableMenu
    {
        // Instance of ModEntry
        private ModEntry Entry;

        // The mods config
        public ModConfig Config;

        // Instance of ContentPackHelper
        public ContentPackHelper PackHelper;

        // Instance of DresserHandler
        public DresserHandler Dresser;

        // Instance of PlayerLoader
        public CharacterLoader PlayerLoader;

        // Instance of PlayerChanger
        public PlayerChanger PlayerChanger;

        // Components used in the Glam Menu
        private GlamMenuComponents MenuComponents;

        // The index of the nose
        public int NoseIndex = 0;

        // The index of the face
        public int FaceIndex = 0;

        // The index of the base
        public int BaseIndex = 0;

        // The index of the base, there is always going to be 1 dresser
        public int DresserIndex = 1;

        // The indes of the shoe
        public int ShoeIndex = 0;

        // Whether the player is bald
        public bool IsBald = false;

        // Snapshot of the Farmer before making changes
        private int[] FarmerSnapshot = new int[11] {0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0};

        // Snapshot of haircolor
        private Color HairColorSnapshot;

        // Snapshot of eyecolor
        private Color EyeColorSnapshot;

        /// <summary>
        /// Glam Menu's Constructor.
        /// </summary>
        /// <param name="entry">Instance of <see cref="ModEntry"/></param>
        /// <param name="packHelper">Instance of <see cref="ContentPackHelper"/></param>
        /// <param name="dresser">Instance of <see cref="DresserHandler"/></param>
        /// <param name="playerLoader">Instance of <seealso cref="CharacterLoader"/></param>
        public GlamMenu(ModEntry entry, ModConfig config, ContentPackHelper packHelper, DresserHandler dresser, CharacterLoader playerLoader, PlayerChanger changer)
            : base((int)Utility.getTopLeftPositionForCenteringOnScreen(712, 712, 0, 0).X, (int)Utility.getTopLeftPositionForCenteringOnScreen(712, 712, 0, 0).Y - IClickableMenu.borderWidth, 712, 712, false)
        {
            Entry = entry;
            Config = config;
            PackHelper = packHelper;
            Dresser = dresser;
            PlayerLoader = playerLoader;
            PlayerChanger = changer;
            MenuComponents = new GlamMenuComponents(Entry, this, packHelper);

            CheckPlayerForHat();
            MenuComponents.SetUpMenuComponents();    
        }

        /// <summary>
        /// Checks if the player is wearing a hat.
        /// </summary>
        private void CheckPlayerForHat()
        {
            if (Game1.player.hat.Value != null)
            {
                // Get the draw type value and change the HairFix button as selected
                if (Game1.player.hat.Value.hairDrawType.Get() == 0)
                    MenuComponents.IsHatFixSelected = true;
            }
        }

        /// <summary>
        /// Update the indexes on the menu when the player loads a layout.
        /// </summary>
        /// <param name="baseindex">The base index</param>
        /// <param name="faceIndex">The face index</param>
        /// <param name="noseIndex">The nose index</param>
        /// <param name="shoeIndex">The shoe index</param>
        /// <param name="dresserIndex">The dresser index</param>
        /// <param name="isBald">Whether the player is bald</param>
        /// <remarks>This is only used when the player loads a layout from a json file</remarks>
        public void UpdateIndexes(int baseindex, int faceIndex, int noseIndex, int shoeIndex, bool isBald, int dresserIndex = 1)
        {
            BaseIndex = baseindex;
            FaceIndex = faceIndex;
            NoseIndex = noseIndex;
            ShoeIndex = shoeIndex;
            IsBald = isBald;

            if (dresserIndex != 1)
                DresserIndex = dresserIndex;
        }

        /// <summary>
        /// Takes a snapshot of the farmer when opening favorites/glam menu<.
        /// /summary>
        public void TakeSnapshot()
        {
            FarmerSnapshot[0] = Game1.player.isMale ? 0 : 1;
            FarmerSnapshot[1] = BaseIndex;
            FarmerSnapshot[2] = Game1.player.skin.Get();
            FarmerSnapshot[3] = Game1.player.hair.Get();
            FarmerSnapshot[4] = FaceIndex;
            FarmerSnapshot[5] = NoseIndex;
            FarmerSnapshot[6] = ShoeIndex;
            FarmerSnapshot[7] = Game1.player.accessory.Get();
            FarmerSnapshot[8] = IsBald ? 0 : 1;

            HairColorSnapshot = Game1.player.hairstyleColor.Get();
            EyeColorSnapshot = Game1.player.newEyeColor.Get();
        }

        /// <summary>
        /// Restores a snapshot that was taken.
        /// </summary>
        public void RestoreSnapshot()
        {
            Game1.player.changeGender(FarmerSnapshot[0] == 0 ? true : false);
            Game1.player.changeSkinColor(FarmerSnapshot[2], true);
            Game1.player.hair.Set(FarmerSnapshot[3]);
            Game1.player.changeAccessory(FarmerSnapshot[7]);

            Game1.player.newEyeColor.Set(EyeColorSnapshot);
            Game1.player.FarmerRenderer.recolorEyes(EyeColorSnapshot);
            Game1.player.changeHairColor(HairColorSnapshot);

            PlayerChanger.ChangePlayerBase(FarmerSnapshot[0] == 0 ? true : false, FarmerSnapshot[1], FarmerSnapshot[4], FarmerSnapshot[5], FarmerSnapshot[6], FarmerSnapshot[8] == 0 ? true : false);
        }

        /// <summary>
        /// Override to change the menu when the window size changes.
        /// </summary>
        /// <param name="oldBounds">The old bounds</param>
        /// <param name="newBounds">The new bounds</param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            // Call the base version and update the x and y position of the menu
            base.gameWindowSizeChanged(oldBounds, newBounds);
            xPositionOnScreen = Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
            yPositionOnScreen = Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - 64;

            MenuComponents.SetUpMenuComponents();
        }

        /// <summary>
        /// Override that checks if the mouse is above a certain element.
        /// </summary>
        /// <param name="x">The x position</param>
        /// <param name="y">The y position</param>
        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            MenuComponents.OnHover(x, y);
        }

        /// <summary>
        /// Override that handles recieving a left click.
        /// </summary>
        /// <param name="x">The x position</param>
        /// <param name="y">The y position</param>
        /// <param name="playSound">Whether to play the sounds</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            MenuComponents.LeftClick(x, y);
            MenuComponents.LeftClickColorPicker(x, y);
        }

        /// <summary>Override to handle the left click being held</summary>
        /// <param name="x">The x position of the mouse</param>
        /// <param name="y">The y position of the mouse</param>
        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);
            MenuComponents.LeftClickHeld(x, y);
        }

        /// <summary>Override to handle the left click being released</summary>
        /// <param name="x">The x position of the mouse</param>
        /// <param name="y">The y position of the mouse</param>
        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
            MenuComponents.LeftClickReleased(x, y);
        }

        /// <summary>Override to handles updating a menu per tick</summary>
        /// <param name="time">The amount of GameTime</param>
        public override void update(GameTime time)
        {
            base.update(time);
        }

        /// <summary>Override to draw the different menu parts</summary>
        /// <param name="b">The games spritebatch</param>
        public override void draw(SpriteBatch b)
        {
            // Draw the dialogue box or else Minerva will haunt my dreams
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);

            // Draw Menu Components
            MenuComponents.Draw(b);

            //Draw the Farmer!!!
            Game1.player.FarmerRenderer.draw(b, Game1.player.FarmerSprite.CurrentAnimationFrame, Game1.player.FarmerSprite.CurrentFrame, Game1.player.FarmerSprite.SourceRect, new Vector2((this.xPositionOnScreen + this.width / 2 - 32), (this.yPositionOnScreen + this.height / 2 - 32)), Vector2.Zero, 0.8f, Color.White, 0f, 1f, Game1.player);

            //Lastly, draw the mouse if they're not using the hardware cursor
            if (Game1.activeClickableMenu == this && !Game1.options.hardwareCursor)
                drawMouse(b);
        }
    }
}
