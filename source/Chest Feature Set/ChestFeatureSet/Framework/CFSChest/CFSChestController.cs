/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack20136/ChestFeatureSet
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using static StardewValley.Objects.Chest;

namespace ChestFeatureSet.Framework.CFSChest
{
    /// <summary>
    /// Used for drawing and selecting chests
    /// </summary>
    public class CFSChestController
    {
        private readonly IModEvents Events;

        private CFSChestManager CFSChestManager { get; set; }

        private readonly ClickableTextureComponent SelectedComponent;
        private readonly ClickableTextureComponent DeselectedComponent;
        private readonly int PosNum;

        private readonly HashSet<Chest> Chests;
        private Chest OpenedChest { get; set; }

        /// <summary>
        /// New ChestController
        /// </summary>
        /// <param name="saveFileName">The file save name. EX:CraftFromChests.json </param>
        /// <param name="textures">This is a struct that contains all the textures used by the mod.</param>
        /// <param name="posNum">The button's pos. (Default is 1)</param>
        public CFSChestController(ModEntry modEntry, string saveFileName, Dictionary<string, Texture2D> textures, Rectangle sourceRect, int posNum = 1)
        {
            this.Events = modEntry.Helper.Events;

            this.CFSChestManager = new CFSChestManager(this, modEntry, saveFileName);

            this.Events.GameLoop.DayStarted += this.CFSChestManager.OnDayStarted;
            this.Events.GameLoop.Saving += this.CFSChestManager.OnSaving;

            this.SelectedComponent = new ClickableTextureComponent(sourceRect, textures["SelectedComponent"], sourceRect, 1f);
            this.DeselectedComponent = new ClickableTextureComponent(sourceRect, textures["DeselectedComponent"], sourceRect, 1f);
            this.PosNum = posNum;

            this.Chests = new HashSet<Chest>();
            this.OpenedChest = null;

            this.Events.Display.RenderedActiveMenu += this.OnRenderedActiveMenu;
            this.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        public void Deactivate()
        {
            this.Events.GameLoop.DayStarted -= this.CFSChestManager.OnDayStarted;
            this.Events.GameLoop.Saving -= this.CFSChestManager.OnSaving;

            this.Events.Display.RenderedActiveMenu -= this.OnRenderedActiveMenu;
            this.Events.Input.ButtonPressed -= this.OnButtonPressed;
        }

        /// <summary>
        /// Draw the icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            this.DrawIcon(e.SpriteBatch);
        }

        /// <summary>
        /// Handle clicking of the fridge icons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button == SButton.MouseLeft)
                this.HandleClick(e.Cursor);
        }

        /// <summary>
        /// Gets the chest that is currently open or null if no chest is open.
        /// </summary>
        /// <returns>The chest that is open or null</returns>
        private Chest GetOpenChest()
        {
            var clickableMenu = Game1.activeClickableMenu;
            var itemGrabMenu = clickableMenu as ItemGrabMenu;

            if (itemGrabMenu?.behaviorOnItemGrab?.Target is Chest chest)
                return chest;

            return null;
        }

        /// <summary>
        /// Handle the click event if it was on the fridge icon.
        /// </summary>
        /// <param name="cursor">The current cursor position.</param>
        public void HandleClick(ICursorPosition cursor)
        {
            var chest = this.GetOpenChest();
            if (chest == null)
                return;

            var screenPixels = Utility.ModifyCoordinatesForUIScale(cursor.ScreenPixels);

            if (!SelectedComponent.containsPoint((int)screenPixels.X, (int)screenPixels.Y))
                return;

            Game1.playSound("smallSelect");

            if (this.Chests.Contains(chest))
                this.Chests.Remove(chest);
            else
                this.Chests.Add(chest);
        }

        /// <summary>
        /// Draw the icon.
        /// </summary>
        /// <param name="e"></param>
        public void DrawIcon(SpriteBatch SpriteBatch)
        {
            var openChest = this.OpenedChest;
            if (openChest == null || Game1.activeClickableMenu == null || !openChest.playerChest.Value)
                return;

            if (openChest == Game1.getLocationFromName("farmHouse").GetFridge())
                return;

            var openChestSpecialChestType = openChest.SpecialChestType;
            if (!(openChestSpecialChestType == SpecialChestTypes.None || openChestSpecialChestType == SpecialChestTypes.BigChest))
                return;

            this.UpdatePos(this.PosNum, openChestSpecialChestType);

            if (this.Chests.Contains(openChest))
                this.SelectedComponent.draw(SpriteBatch, Color.White, 0f);
            else
                this.DeselectedComponent.draw(SpriteBatch, Color.White, 0f);

            // Let MouseCursor above LockIcon.
            Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()),
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0f, Vector2.Zero,
                4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Returns the chests currently selected
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Chest> GetChests()
        {
            return this.Chests;
        }

        /// <summary>
        /// Update the chests
        /// </summary>
        /// <param name="chests"></param>
        public void SetChests(IEnumerable<Chest> chests)
        {
            this.Chests.Clear();
            foreach (var chest in chests)
            {
                this.Chests.Add(chest);
            }
        }

        /// <summary>
        /// Remove chest from list
        /// </summary>
        public void AddChest(Chest chest)
        {
            if (chest != null)
                this.Chests.Add(chest);
        }

        /// <summary>
        /// Remove chest from list
        /// </summary>
        public void RemoveChest(Chest chest)
        {
            if (chest != null)
                this.Chests.Remove(chest);
        }

        /// <summary>
        /// Update the OpenedChest
        /// </summary>
        public void UpdateOpenedChest()
        {
            this.OpenedChest = this.GetOpenChest();
        }

        /// <summary>
        /// Update the position of the button based on the settings in the config
        /// </summary>
        private void UpdatePos(int PosNum, SpecialChestTypes SpecialChestType)
        {
            var menu = Game1.activeClickableMenu;
            if (menu == null)
                return;

            var xOffset = 0.0;
            var yOffset = 0.0;

            if (SpecialChestType == SpecialChestTypes.BigChest)
                xOffset = -1.0;

            switch (PosNum)
            {
                case 1:
                    yOffset = 0.925;
                    break;
                case 2:
                    yOffset = 2.1;
                    break;
                case 3:
                    break;
            }


            var xScaledOffset = (int)(xOffset * Game1.tileSize);
            var yScaledOffset = (int)(yOffset * Game1.tileSize);

            var screenX = menu.xPositionOnScreen - 17 * Game1.pixelZoom + xScaledOffset;
            var screenY = menu.yPositionOnScreen + yScaledOffset + Game1.pixelZoom * 5;

            var rectangle = new Rectangle(screenX, screenY, 16 * Game1.pixelZoom, 16 * Game1.pixelZoom);

            this.SelectedComponent.bounds = this.DeselectedComponent.bounds = rectangle;
        }
    }
}
