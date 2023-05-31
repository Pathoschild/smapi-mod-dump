/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using xTile.Dimensions;
using StardewValley.Menus;
using StardewValley;
using MultiplayerMod.Framework.Patch.Mobile;

namespace MultiplayerMod.Framework.Mobile.Menus
{
    // Token: 0x020005E8 RID: 1512
    public class SLoadGameMenu : IClickableMenu, IDisposable
    {
        // Token: 0x060053B0 RID: 21424 RVA: 0x0045712B File Offset: 0x0045532B
        public bool IsDoingTask()
        {
            return this._initTask != null || this._deleteTask != null || this.loading || this.deleting;
        }

        // Token: 0x060053B1 RID: 21425 RVA: 0x0045714D File Offset: 0x0045534D
        public override bool readyToClose()
        {
            return true;
        }

        // Token: 0x17000386 RID: 902
        // (get) Token: 0x060053B2 RID: 21426 RVA: 0x0045717A File Offset: 0x0045537A
        // (set) Token: 0x060053B3 RID: 21427 RVA: 0x00457182 File Offset: 0x00455382
        protected virtual List<SLoadGameMenu.MenuSlot> MenuSlots
        {
            get
            {
                return this.menuSlots;
            }           
            set
            {
                this.menuSlots = value;
            }
        }

        // Token: 0x060053B4 RID: 21428 RVA: 0x0045718C File Offset: 0x0045538C
        public SLoadGameMenu(int yTopOffset = 0) : base(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height, true)
        {
            base.initializeUpperRightCloseButton();
            this.resetButtonStatus();
            this.width = Game1.uiViewport.Width;
            this.xPositionOnScreen = Game1Patch.xEdge;
            this.yPositionOnScreen = this.upperRightCloseButton.bounds.Y + this.upperRightCloseButton.bounds.Height;
            this.height = Game1.uiViewport.Height;
            this.widthMod = (float)this.width / 1280f;
            this.heightMod = (float)this.height / 720f;
            this.scrolling = false;
            this.outerBox = new Microsoft.Xna.Framework.Rectangle(Game1Patch.xEdge, this.yPositionOnScreen, this.width - 2 * Game1Patch.xEdge, this.height - this.yPositionOnScreen);
            this.mainBox = new Microsoft.Xna.Framework.Rectangle(Game1Patch.xEdge, this.yPositionOnScreen + yTopOffset, this.width - 2 * Game1Patch.xEdge, this.height - this.yPositionOnScreen - yTopOffset);
            this.clipBox = new Microsoft.Xna.Framework.Rectangle(this.mainBox.X + 16, this.mainBox.Y + 16, this.mainBox.Width - 32, this.mainBox.Height - 32);
            this.newScrollbar = new MobileScrollbar(this.mainBox.X + this.mainBox.Width - 60, this.clipBox.Y, 55, this.clipBox.Height - 4, 0, 24, false);
            this.scrollArea = new MobileScrollbox(this.mainBox.X, this.mainBox.Y, this.mainBox.Width, this.mainBox.Height, 1000, this.clipBox, this.newScrollbar);
            this.itemsPerPage = this.clipBox.Height / this.itemHeight;
            this.positionChildren();
            this.startListPopulation();
        }

        // Token: 0x060053B5 RID: 21429 RVA: 0x004573FD File Offset: 0x004555FD
        protected virtual bool hasDeleteButtons()
        {
            return true;
        }

        // Token: 0x060053B6 RID: 21430 RVA: 0x00457400 File Offset: 0x00455600
        protected virtual void startListPopulation()
        {
            this._initTask = new Task<List<Farmer>>(delegate ()
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                return SLoadGameMenu.FindSaveGames();
            });
            this._initTask.Start();
        }

        // Token: 0x060053B7 RID: 21431 RVA: 0x00457437 File Offset: 0x00455637
        protected virtual void addSaveFiles(List<Farmer> files)
        {
            this.MenuSlots.AddRange(from file in files
                                    select new SLoadGameMenu.SaveFileSlot(this, file));
        }

        // Token: 0x060053B8 RID: 21432 RVA: 0x00457458 File Offset: 0x00455658
        private static List<Farmer> FindSaveGames()
        {
            List<Farmer> list = new List<Farmer>();
            string savesPath = Game1Patch.savesPath;
            if (Directory.Exists(savesPath))
            {
                foreach (string text in Directory.GetDirectories(savesPath))
                {
                    string text2 = Path.Combine(savesPath, text, "SaveGameInfo");
                    Farmer farmer = null;
                    try
                    {
                        using (FileStream fileStream = File.OpenRead(text2))
                        {
                            farmer = (Farmer)SaveGame.farmerSerializer.Deserialize(fileStream);
                            SaveGame.loadDataToFarmer(farmer);
                            farmer.slotName = text.Split(Path.DirectorySeparatorChar, StringSplitOptions.None).Last<string>();
                            list.Add(farmer);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception occured trying to access file '{0}'", text2);
                        Console.WriteLine(ex.GetBaseException().ToString());
                        if (farmer != null)
                        {
                            farmer.unload();
                        }
                    }
                }
            }
            list.Sort();
            return list;
        }

        // Token: 0x060053B9 RID: 21433 RVA: 0x00457550 File Offset: 0x00455750
        public override void receiveGamePadButton(Buttons b)
        {
            if (this.confirmBox != null)
            {
                this.confirmBox.receiveGamePadButton(b);
                return;
            }
            if (this.backupBox != null)
            {
                this.backupBox.receiveGamePadButton(b);
                return;
            }
            if (this._joypadSelectedItemIndex == -1 && (b == Buttons.DPadUp || b == Buttons.LeftThumbstickUp || b == Buttons.DPadDown || b == Buttons.LeftThumbstickDown))
            {
                this._joypadSelectedItemIndex = this.currentItemIndex;
                Game1.playSound("shwip");
                return;
            }
            if (b == Buttons.DPadUp || b == Buttons.LeftThumbstickUp)
            {
                this._joypadSelectedItemIndex--;
                if (this._joypadSelectedItemIndex < 0)
                {
                    this._joypadSelectedItemIndex = 0;
                }
                else
                {
                    this.scrollArea.setYOffsetForScroll(-this._joypadSelectedItemIndex * this.itemHeight);
                    Game1.playSound("shwip");
                }
            }
            else if (b == Buttons.DPadDown || b == Buttons.LeftThumbstickDown)
            {
                this._joypadSelectedItemIndex++;
                if (this._joypadSelectedItemIndex >= this.MenuSlots.Count)
                {
                    this._joypadSelectedItemIndex = this.MenuSlots.Count - 1;
                }
                else
                {
                    if (this._joypadSelectedItemIndex > 1 && this._joypadSelectedItemIndex < this.MenuSlots.Count - 2)
                    {
                        this.scrollArea.setYOffsetForScroll(-this._joypadSelectedItemIndex * this.itemHeight);
                    }
                    Game1.playSound("shwip");
                }
            }
            if (b == Buttons.B && this.upperRightCloseButton != null)
            {
                this.receiveLeftClick(this.upperRightCloseButton.bounds.X, this.upperRightCloseButton.bounds.Y, true);
            }
        }

        // Token: 0x060053BA RID: 21434 RVA: 0x004576DA File Offset: 0x004558DA
        public override void snapToDefaultClickableComponent()
        {
            this.currentlySnappedComponent = base.getComponentWithID(this._joypadSelectedItemIndex);
            this.snapCursorToCurrentSnappedComponent();
        }

        // Token: 0x060053BB RID: 21435 RVA: 0x004576F4 File Offset: 0x004558F4
        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
        }

        // Token: 0x060053BC RID: 21436 RVA: 0x004576F6 File Offset: 0x004558F6
        public override void gameWindowSizeChanged(Microsoft.Xna.Framework.Rectangle oldBounds, Microsoft.Xna.Framework.Rectangle newBounds)
        {
            this.positionChildren();
        }

        // Token: 0x060053BD RID: 21437 RVA: 0x004576FE File Offset: 0x004558FE
        public override void performHoverAction(int x, int y)
        {
        }

        // Token: 0x060053BE RID: 21438 RVA: 0x00457700 File Offset: 0x00455900
        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);
            if (this.confirmBox != null)
            {
                this.confirmBox.leftClickHeld(x, y);
                return;
            }
            if (this.backupBox != null)
            {
                this.backupBox.leftClickHeld(x, y);
                return;
            }
            if (this.MenuSlots.Count > this.itemsPerPage)
            {
                this.scrollArea.leftClickHeld(x, y);
                if (this.newScrollbar.sliderContains(x, y) || this.newScrollbar.sliderRunnerContains(x, y))
                {
                    float num = this.newScrollbar.setY(y);
                    this.scrollArea.setYOffsetForScroll((int)(-num * (float)(this.MenuSlots.Count - this.itemsPerPage) * (float)this.itemHeight / 100f));
                }
            }
        }

        // Token: 0x060053BF RID: 21439 RVA: 0x004577BC File Offset: 0x004559BC
        public override void releaseLeftClick(int x, int y)
        {
            if (this.confirmBox != null)
            {
                this.confirmBox.releaseLeftClick(x, y);
            }
            if (this.backupBox != null)
            {
                this.backupBox.releaseLeftClick(x, y);
            }
            this.scrolling = false;
            if (this.scrollArea == null || (this.scrollArea != null && !this.scrollArea.havePanelScrolled))
            {
                if (this.timerToLoad > 0 || this.loading || this.deleting)
                {
                    return;
                }
                if (!this.deleteConfirmationScreen)
                {
                    for (int i = 0; i < this.slotButtons.Count; i++)
                    {
                        if (this.slotButtons[i].containsPoint(x, y) && i < this.MenuSlots.Count)
                        {
                            foreach (var deleteBtn in deleteButtons)
                            {
                                if (deleteBtn.containsPoint(x, y)) break;
                            }
                            SLoadGameMenu.SaveFileSlot saveFileSlot = this.MenuSlots[this.currentItemIndex + i] as SLoadGameMenu.SaveFileSlot;
                            if (saveFileSlot == null || saveFileSlot.versionComparison > -1)
                            {
                                this.timerToLoad = 2150;
                                this.loading = true;
                                Game1.playSound("select");
                                this.selected = i;
                                return;
                            }
                            saveFileSlot.redTimer = Game1.currentGameTime.TotalGameTime.TotalSeconds + 1.0;
                            Game1.playSound("cancel");
                        }
                    }
                }
                if (this.deleteConfirmationScreen)
                {
                    if (this._cancelSelected)
                    {
                        this.deleteConfirmationScreen = false;
                        this.selectedForDelete = -1;
                        Game1.playSound("smallSelect");
                        if (Game1.options.snappyMenus && Game1.options.gamepadControls)
                        {
                            this.currentlySnappedComponent = base.getComponentWithID(0);
                            this.snapCursorToCurrentSnappedComponent();
                            return;
                        }
                    }
                    else if (this._okSelected)
                    {
                        this.deleting = true;
                        this._deleteTask = new Task(delegate ()
                        {
                            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                            this.deleteFile(this.selectedForDelete);
                        });
                        this._deleteTask.Start();
                        this.deleteConfirmationScreen = false;
                        if (Game1.options.snappyMenus && Game1.options.gamepadControls)
                        {
                            this.currentlySnappedComponent = base.getComponentWithID(0);
                            this.snapCursorToCurrentSnappedComponent();
                        }
                        Game1.playSound("trashcan");
                    }
                    return;
                }
            }
            if (this.scrollArea != null)
            {
                this.scrollArea.releaseLeftClick(x, y);
            }
            this.currentItemIndex = Math.Max(0, Math.Min(this.MenuSlots.Count - this.itemsPerPage, this.currentItemIndex));
            this.scrolling = false;
        }

        // Token: 0x060053C0 RID: 21440 RVA: 0x00457A0B File Offset: 0x00455C0B
        protected void setScrollBarToCurrentIndex()
        {
        }

        // Token: 0x060053C1 RID: 21441 RVA: 0x00457A0D File Offset: 0x00455C0D
        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            this.scrollArea.receiveScrollWheelAction(direction);
        }

        // Token: 0x060053C2 RID: 21442 RVA: 0x00457A22 File Offset: 0x00455C22
        private void downArrowPressed()
        {
            this.downArrow.scale = this.downArrow.baseScale;
            this.currentItemIndex++;
            Game1.playSound("shwip");
            this.setScrollBarToCurrentIndex();
        }

        // Token: 0x060053C3 RID: 21443 RVA: 0x00457A58 File Offset: 0x00455C58
        private void upArrowPressed()
        {
            this.upArrow.scale = this.upArrow.baseScale;
            this.currentItemIndex--;
            Game1.playSound("shwip");
            this.setScrollBarToCurrentIndex();
        }

        // Token: 0x060053C4 RID: 21444 RVA: 0x00457A90 File Offset: 0x00455C90
        private void deleteFile(int which)
        {
            SLoadGameMenu.SaveFileSlot saveFileSlot = this.MenuSlots[which] as SLoadGameMenu.SaveFileSlot;
            if (saveFileSlot == null)
            {
                return;
            }
            string slotName = saveFileSlot.Farmer.slotName;
            Path.Combine(new string[]
            {
                Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley"), "Saves"), slotName)
            });
            string text = Path.Combine(Game1Patch.savesPath, slotName);
            string text2 = Path.Combine(text, "SaveGameInfo");
            string text3 = Path.Combine(text, slotName);
            ModUtilities.ModMonitor.Log(string.Concat(new string[]
            {
                "SLoadGameMenu.deleteFile saveDir:",
                text,
                " saveGameInfoPath:",
                text2,
                " dataPath:",
                text3
            }));
            File.Delete(text2);
            File.Delete(text3);
            Directory.Delete(text, true);
            SaveGamePatch.deleteEmergencySaveIfCalled(slotName);
        }

        // Token: 0x060053C5 RID: 21445 RVA: 0x00457B60 File Offset: 0x00455D60
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.confirmBox != null)
            {
                this.confirmBox.receiveLeftClick(x, y, true);
                return;
            }
            if (this.backupBox != null)
            {
                this.backupBox.receiveLeftClick(x, y, true);
                return;
            }
            if (this.MenuSlots.Count > this.itemsPerPage)
            {
                this.scrollArea.receiveLeftClick(x, y);
                if (this.newScrollbar.sliderContains(x, y) || this.newScrollbar.sliderRunnerContains(x, y))
                {
                    this.scrolling = true;
                }
            }
            base.receiveLeftClick(x, y, playSound);
            if (this.MenuSlots == null)
            {
                return;
            }
            if (this.selected == -1)
            {
                for (int i = 0; i < this.deleteButtons.Count; i++)
                {
                    if (this.deleteButtons[i].containsPoint(x, y) && i < this.MenuSlots.Count && !this.deleteConfirmationScreen)
                    {
                        this.deleteConfirmationScreen = true;
                        Game1.playSound("drumkit6");
                        this.selectedForDelete = this.currentItemIndex + i;
                        this.confirmBox = new ConfirmationDialog(Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11023", (this.MenuSlots[this.selectedForDelete] as SLoadGameMenu.SaveFileSlot).Farmer.Name), new ConfirmationDialog.behavior(this.okSelected), new ConfirmationDialog.behavior(this.cancelSelected));
                        this.resetButtonStatus();
                        if (Game1.options.snappyMenus && Game1.options.gamepadControls)
                        {
                            this.currentlySnappedComponent = base.getComponentWithID(803);
                            this.snapCursorToCurrentSnappedComponent();
                        }
                        return;
                    }
                }
            }
        }

        // Token: 0x060053C6 RID: 21446 RVA: 0x00457CF5 File Offset: 0x00455EF5
        protected virtual void saveFileScanComplete()
        {
        }

        // Token: 0x060053C7 RID: 21447 RVA: 0x00457CF8 File Offset: 0x00455EF8
        protected virtual bool checkListPopulation()
        {
            if (!this.deleteConfirmationScreen)
            {
                this._updatesSinceLastDeleteConfirmScreen++;
            }
            else
            {
                this._updatesSinceLastDeleteConfirmScreen = 0;
            }
            if (this._initTask != null)
            {
                if (this._initTask.IsCanceled || this._initTask.IsCompleted || this._initTask.IsFaulted)
                {
                    if (this._initTask.IsCompleted)
                    {
                        this.addSaveFiles(this._initTask.Result);
                        this.saveFileScanComplete();
                    }
                    this._initTask = null;
                }
                return true;
            }
            return false;
        }

        // Token: 0x060053C8 RID: 21448 RVA: 0x00457D84 File Offset: 0x00455F84
        public override void update(GameTime time)
        {
            base.update(time);


            try
            {
                if (this.storedSaves != this.MenuSlots.Count)
                {
                    this.positionChildren();
                    this.storedSaves = this.MenuSlots.Count;
                }

            }
            catch (Exception e)
            {
                ModUtilities.ModMonitor.Log(e.ToString(), StardewModdingAPI.LogLevel.Error);
            }
            if (this.scrollArea != null)
            {
                this.recalculateSlots();
                this.scrollArea.update(time);
                bool havePanelScrolled = this.scrollArea.havePanelScrolled;
                this.scrollArea.setMaxYOffset((this.MenuSlots.Count - this.itemsPerPage) * this.itemHeight);
                this.scrollArea.havePanelScrolled = havePanelScrolled;
                int yoffsetForScroll = this.scrollArea.getYOffsetForScroll();
                if (this.deleteButtons.Count > this.itemsPerPage)
                {
                    for (int i = 0; i < this.deleteButtons.Count; i++)
                    {
                        this.deleteButtons[i].bounds.X = this.clipBox.X + this.clipBox.Width - 96 - 24;
                        this.deleteButtons[i].bounds.Y = i * this.itemHeight + this.clipBox.Y + (this.itemHeight - 102) / 2 + yoffsetForScroll;
                    }
                }
            }
            if (this.checkListPopulation())
            {
                return;
            }
            if (this._deleteTask != null)
            {
                if (this._deleteTask.IsCanceled || this._deleteTask.IsCompleted || this._deleteTask.IsFaulted)
                {
                    if (!this._deleteTask.IsCompleted)
                    {
                        this.selectedForDelete = -1;
                    }
                    this._deleteTask = null;
                    this.deleting = false;
                }
                return;
            }
            if (this.selectedForDelete >= 0 && this.selectedForDelete < this.MenuSlots.Count && !this.deleteConfirmationScreen && !this.deleting)
            {
                SLoadGameMenu.SaveFileSlot saveFileSlot = this.MenuSlots[this.selectedForDelete] as SLoadGameMenu.SaveFileSlot;
                if (saveFileSlot != null)
                {
                    saveFileSlot.Farmer.unload();
                    this.MenuSlots.RemoveAt(this.selectedForDelete);
                    this.selectedForDelete = -1;
                    this.positionChildren();
                }
            }
            if (this.timerToLoad > 0)
            {
                this.timerToLoad -= time.ElapsedGameTime.Milliseconds;
                if (this.timerToLoad <= 0 && this.selected >= 0 && this.selected < this.MenuSlots.Count)
                {
                    if (this.MenuSlots[this.selected] is SFarmhandMenu.FarmhandSlot || this.MenuSlots[this.selected] is SCoopGameMenu.HostNewFarmSlot || this.MenuSlots[this.selected] is SCoopGameMenu.LanSlot || this.MenuSlots[this.selected] is SCoopGameMenu.HostFileSlot)
                    {
                        this.MenuSlots[this.selected].Activate();
                        return;
                    }

                    bool autoSave = ModUtilities.Helper.Reflection.GetField<bool>(Game1.options, "autoSave").GetValue();
                    if (autoSave && SaveGamePatch.newerBackUpExists((this.MenuSlots[this.selected] as SLoadGameMenu.SaveFileSlot).Farmer.slotName) != null)
                    {
                        this.backupBox = new ConfirmationDialog(Game1.content.LoadString("Strings\\UI:question_restore_backup"), new ConfirmationDialog.behavior(this.backupSelected), new ConfirmationDialog.behavior(this.mainSelected));
                        return;
                    }
                    this.MenuSlots[this.selected].Activate();
                }
            }
        }

        // Token: 0x060053C9 RID: 21449 RVA: 0x00458058 File Offset: 0x00456258
        protected virtual string getStatusText()
        {
            if (this._initTask != null)
            {
                return Game1.content.LoadString("Strings\\UI:SLoadGameMenu_LookingForSavedGames");
            }
            if (this.deleting)
            {
                return Game1.content.LoadString("Strings\\UI:SLoadGameMenu_Deleting");
            }
            if (this.MenuSlots.Count == 0)
            {
                return Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11022");
            }
            return null;
        }

        // Token: 0x060053CA RID: 21450 RVA: 0x004580B3 File Offset: 0x004562B3
        protected virtual void drawExtra(SpriteBatch b)
        {
        }

        // Token: 0x060053CB RID: 21451 RVA: 0x004580B8 File Offset: 0x004562B8
        protected virtual void drawSlotBackground(SpriteBatch b, int i, SLoadGameMenu.MenuSlot slot)
        {
            Color color = Color.White;
            if (((this.currentItemIndex + i == this.selected && this.timerToLoad % 150 > 75 && this.timerToLoad > 1000) || (this.selected == -1 && this.slotButtons[i].scale > 1f && !this.scrolling && !this.deleteConfirmationScreen)) && (this.deleteButtons.Count <= i || !this.deleteButtons[i].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY())))
            {
                color = Color.Wheat;
            }
            if (this._joypadSelectedItemIndex == i)
            {
                color = Color.Wheat;
            }
            IClickableMenuPatch.drawTextureBox(b, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(384, 396, 15, 15), this.slotButtons[i].bounds.X, this.slotButtons[i].bounds.Y, this.slotButtons[i].bounds.Width, this.slotButtons[i].bounds.Height, color, 4f, false, -1f, false);

        }

        // Token: 0x060053CC RID: 21452 RVA: 0x004581EC File Offset: 0x004563EC
        protected virtual void drawBefore(SpriteBatch b)
        {
        }

        // Token: 0x060053CD RID: 21453 RVA: 0x004581F0 File Offset: 0x004563F0
        protected virtual void drawStatusText(SpriteBatch b)
        {
            string statusText = this.getStatusText();
            if (statusText != null)
            {
                SpriteText.drawStringHorizontallyCenteredAt(b, statusText, Game1.graphics.GraphicsDevice.Viewport.Bounds.Center.X, Game1.graphics.GraphicsDevice.Viewport.Bounds.Center.Y, 999999, -1, 999999, 1f, 0.88f, false, -1, 99999);
            }
        }

        // Token: 0x060053CE RID: 21454 RVA: 0x00458274 File Offset: 0x00456474
        public override void draw(SpriteBatch b)
        {
            IClickableMenu.drawTextureBox(b, this.outerBox.X, this.outerBox.Y, this.outerBox.Width, this.outerBox.Height, Color.White);
            if (this.outerBox.Y != this.mainBox.Y)
            {
                // base.drawMobileHorizontalPartition(b, this.mainBox.X + 8, this.mainBox.Y - 8, this.mainBox.Width - 16, false);
                ModUtilities.Helper.Reflection.GetMethod(this, "drawMobileHorizontalPartition").Invoke(b, this.mainBox.X + 8, this.mainBox.Y - 8, this.mainBox.Width - 16, false);
            }
            if (this.scrollArea != null)
            {
                this.scrollArea.setUpForScrollBoxDrawing(b, 1f);
            }
            if (this.selectedForDelete == -1 || !this.deleting || this.deleteConfirmationScreen)
            {
                for (int i = 0; i < this.slotButtons.Count; i++)
                {
                    if (this.currentItemIndex + i < this.MenuSlots.Count)
                    {
                        this.drawSlotBackground(b, i, this.MenuSlots[this.currentItemIndex + i]);
                        this.MenuSlots[this.currentItemIndex + i].Draw(b, i);
                        if (this.deleteButtons.Count > i && !this.MenuSlots[this.currentItemIndex + i].isLabelledSlot())
                        {
                            this.deleteButtons[i].draw(b, Color.White * 0.75f, 0.08f, 0);
                            b.Draw(Game1.mouseCursors, this.deleteButtons[i].getVector2() + new Vector2(-1f, 0f) * 4f, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(564, 129, 18, 10)), Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.081f);
                        }
                    }
                }
            }
            if (this.scrollArea != null)
            {
                this.scrollArea.finishScrollBoxDrawing(b, 1f);
            }
            this.drawStatusText(b);
            if (this.MenuSlots.Count > this.itemsPerPage)
            {
                this.newScrollbar.draw(b);
            }
            if (this.deleteConfirmationScreen && this.MenuSlots[this.selectedForDelete] is SLoadGameMenu.SaveFileSlot && this.confirmBox != null)
            {
                this.confirmBox.draw(b);
            }
            base.draw(b);
            if (this.backupBox != null)
            {
                this.backupBox.draw(b);
            }
            this.drawn = true;
        }

        // Token: 0x060053CF RID: 21455 RVA: 0x004584FE File Offset: 0x004566FE
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        // Token: 0x060053D0 RID: 21456 RVA: 0x00458500 File Offset: 0x00456700
        private void recalculateSlots()
        {
            int num = this.clipBox.Width - ((this.MenuSlots.Count > this.itemsPerPage) ? 32 : 0);
            int num2 = this.itemHeight;
            int x = this.clipBox.X;
            int num3 = this.clipBox.Y;
            this.deleteButtons.Clear();
            this.slotButtons.Clear();
            if (this.scrollArea != null)
            {
                
               
                try
                {
                    num3 += this.scrollArea.getYOffsetForScroll();

                }
                catch (Exception e)
                {
                    ModUtilities.ModMonitor.Log("getYOffsetForScroll", StardewModdingAPI.LogLevel.Error);
                    ModUtilities.ModMonitor.Log(e.ToString(), StardewModdingAPI.LogLevel.Error);
                }
                try
                {
                    num3 = UtilityPatch.To4(num3);

                }
                catch (Exception e)
                {
                    ModUtilities.ModMonitor.Log("To4", StardewModdingAPI.LogLevel.Error);
                    ModUtilities.ModMonitor.Log(e.ToString(), StardewModdingAPI.LogLevel.Error);
                }
            }
            for (int i = 0; i < this.MenuSlots.Count; i++)
            {
                Microsoft.Xna.Framework.Rectangle bounds = new Microsoft.Xna.Framework.Rectangle
                {
                    X = x,
                    Y = num3,
                    Width = num,
                    Height = num2
                };
                try
                {
                    this.slotButtons.Add(new ClickableComponent(bounds, i.ToString() ?? "")
                    {
                        myID = i,
                        region = 900,
                        downNeighborID = ((i < this.itemsPerPage - 1) ? -99998 : -7777),
                        upNeighborID = ((i > 0) ? -99998 : -7777),
                        rightNeighborID = -99998,
                        fullyImmutable = true
                    });

                }
                catch (Exception e)
                {
                    ModUtilities.ModMonitor.Log("slotButtons", StardewModdingAPI.LogLevel.Error);
                    ModUtilities.ModMonitor.Log(e.ToString(), StardewModdingAPI.LogLevel.Error);
                }
                
                try
                {
                    this.deleteButtons.Add(new ClickableTextureComponent("", new Microsoft.Xna.Framework.Rectangle(x + num - 96, UtilityPatch.To4(num3 + (num2 - 52) / 2), 64, 104), "", Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.10994"), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(564, 102, 18, 26), 4f, false)
                    {
                        myID = i + 100,
                        region = 901,
                        leftNeighborID = -99998,
                        downNeighborImmutable = true,
                        downNeighborID = -99998,
                        upNeighborImmutable = true,
                        upNeighborID = ((i > 0) ? -99998 : -1),
                        rightNeighborID = -99998
                    });

                }
                catch (Exception e)
                {
                    ModUtilities.ModMonitor.Log("deleteButtons", StardewModdingAPI.LogLevel.Error);
                    ModUtilities.ModMonitor.Log(e.ToString(), StardewModdingAPI.LogLevel.Error);
                }
                
                num3 += num2;
            }
        }

        // Token: 0x060053D1 RID: 21457 RVA: 0x00458708 File Offset: 0x00456908
        private void positionChildren()
        {
            int num = this.clipBox.Width;
            int num2 = this.clipBox.Height;
            xTile.Dimensions.Rectangle uiViewport = Game1.uiViewport;
            int num3 = (uiViewport.Width - num) / 2;
            int num4 = (uiViewport.Height - num2) / 2;
            this.scrollArea.setYOffsetForScroll(0);
            this.newScrollbar.setPercentage(0f);
            int num5 = 12;
            Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
            rectangle.Inflate(-num5, -num5);
            this.recalculateSlots();
            
        }

        // Token: 0x060053D2 RID: 21458 RVA: 0x0045879A File Offset: 0x0045699A
        public void backupSelected(Farmer who)
        {
            this.backupBox = null;
            SaveGamePatch.Load((this.MenuSlots[this.selected] as SLoadGameMenu.SaveFileSlot).Farmer.slotName, false, true);
            Game1.exitActiveMenu();
        }

        // Token: 0x060053D3 RID: 21459 RVA: 0x004587CF File Offset: 0x004569CF
        public void mainSelected(Farmer who)
        {
            this.backupBox = null;
            this.MenuSlots[this.selected].Activate();
        }

        // Token: 0x060053D4 RID: 21460 RVA: 0x004587EE File Offset: 0x004569EE
        public void okSelected(Farmer who)
        {
            this._okSelected = true;
            this.confirmBox = null;
        }

        // Token: 0x060053D5 RID: 21461 RVA: 0x004587FE File Offset: 0x004569FE
        public void cancelSelected(Farmer who)
        {
            this._cancelSelected = true;
            this.confirmBox = null;
        }

        // Token: 0x060053D6 RID: 21462 RVA: 0x00458810 File Offset: 0x00456A10
        public void resetButtonStatus()
        {
            this._okSelected = (this._cancelSelected = false);
        }

        // Token: 0x060053D7 RID: 21463 RVA: 0x00458830 File Offset: 0x00456A30
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    if (this.MenuSlots != null)
                    {
                        foreach (SLoadGameMenu.MenuSlot menuSlot in this.MenuSlots)
                        {
                            menuSlot.Dispose();
                        }
                        this.MenuSlots.Clear();
                    }
                    if (this._initTask != null)
                    {
                        this._initTask = null;
                    }
                    if (this._deleteTask != null)
                    {
                        this._deleteTask = null;
                    }
                }
                this.disposedValue = true;
            }
        }

        // Token: 0x060053D8 RID: 21464 RVA: 0x004588CC File Offset: 0x00456ACC
        ~SLoadGameMenu()
        {
            this.Dispose(false);
        }

        // Token: 0x060053D9 RID: 21465 RVA: 0x004588FC File Offset: 0x00456AFC
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Token: 0x040031EE RID: 12782
        public MobileScrollbox scrollArea;

        // Token: 0x040031EF RID: 12783
        public MobileScrollbar newScrollbar;

        // Token: 0x040031F0 RID: 12784
        private Microsoft.Xna.Framework.Rectangle mainBox;

        // Token: 0x040031F1 RID: 12785
        private Microsoft.Xna.Framework.Rectangle clipBox;

        // Token: 0x040031F2 RID: 12786
        private Microsoft.Xna.Framework.Rectangle outerBox;

        // Token: 0x040031F3 RID: 12787
        private new int width;

        // Token: 0x040031F4 RID: 12788
        private new int height;

        // Token: 0x040031F5 RID: 12789
        private int windowInset;

        // Token: 0x040031F6 RID: 12790
        private float widthMod;

        // Token: 0x040031F7 RID: 12791
        private float heightMod;

        // Token: 0x040031F8 RID: 12792
        private bool sliderVisible;

        // Token: 0x040031F9 RID: 12793
        private bool _okSelected;

        // Token: 0x040031FA RID: 12794
        private bool _cancelSelected;

        // Token: 0x040031FB RID: 12795
        private int itemsPerPage = 4;

        // Token: 0x040031FC RID: 12796
        private int storedSaves;

        // Token: 0x040031FD RID: 12797
        private int itemHeight = 200;

        // Token: 0x040031FE RID: 12798
        private SpriteFont mainFont;

        // Token: 0x040031FF RID: 12799
        private ConfirmationDialog confirmBox;

        // Token: 0x04003200 RID: 12800
        private ConfirmationDialog backupBox;

        // Token: 0x04003201 RID: 12801
        private int _joypadSelectedItemIndex = -1;

        // Token: 0x04003202 RID: 12802
        public const int region_upArrow = 800;

        // Token: 0x04003203 RID: 12803
        public const int region_downArrow = 801;

        // Token: 0x04003204 RID: 12804
        public const int region_okDelete = 802;

        // Token: 0x04003205 RID: 12805
        public const int region_cancelDelete = 803;

        // Token: 0x04003206 RID: 12806
        public const int region_slots = 900;

        // Token: 0x04003207 RID: 12807
        public const int region_deleteButtons = 901;

        // Token: 0x04003208 RID: 12808
        public const int region_navigationButtons = 902;

        // Token: 0x04003209 RID: 12809
        public const int region_deleteConfirmations = 903;

        // Token: 0x0400320A RID: 12810
        public List<ClickableComponent> slotButtons { get; } = new List<ClickableComponent>();

        // Token: 0x0400320B RID: 12811
        public List<ClickableTextureComponent> deleteButtons { get; } = new List<ClickableTextureComponent>();

        // Token: 0x0400320C RID: 12812
        protected int currentItemIndex;

        // Token: 0x0400320D RID: 12813
        protected int timerToLoad;

        // Token: 0x0400320E RID: 12814
        protected int selected = -1;

        // Token: 0x0400320F RID: 12815
        protected int selectedForDelete = -1;

        // Token: 0x04003210 RID: 12816
        public ClickableTextureComponent upArrow;

        // Token: 0x04003211 RID: 12817
        public ClickableTextureComponent downArrow;

        // Token: 0x04003212 RID: 12818
        public ClickableTextureComponent scrollBar;

        // Token: 0x04003213 RID: 12819
        public ClickableTextureComponent okDeleteButton;

        // Token: 0x04003214 RID: 12820
        public ClickableTextureComponent cancelDeleteButton;

        // Token: 0x04003215 RID: 12821
        public ClickableComponent backButton;

        // Token: 0x04003216 RID: 12822
        public bool scrolling;

        // Token: 0x04003217 RID: 12823
        public bool deleteConfirmationScreen = false;

        // Token: 0x04003218 RID: 12824
        protected List<SLoadGameMenu.MenuSlot> menuSlots { get; set; } = new List<SLoadGameMenu.MenuSlot>();

        // Token: 0x04003219 RID: 12825
        private Microsoft.Xna.Framework.Rectangle scrollBarRunner;

        // Token: 0x0400321A RID: 12826
        private string hoverText = "";

        // Token: 0x0400321B RID: 12827
        protected bool loading;

        // Token: 0x0400321C RID: 12828
        protected bool drawn;

        // Token: 0x0400321D RID: 12829
        private bool deleting;

        // Token: 0x0400321E RID: 12830
        private int _updatesSinceLastDeleteConfirmScreen;

        // Token: 0x0400321F RID: 12831
        private Task<List<Farmer>> _initTask;

        // Token: 0x04003220 RID: 12832
        private Task _deleteTask;

        // Token: 0x04003221 RID: 12833
        private bool disposedValue;

        // Token: 0x020007E2 RID: 2018
        public abstract class MenuSlot : IDisposable
        {
            // Token: 0x06005F1D RID: 24349 RVA: 0x004DD259 File Offset: 0x004DB459
            public MenuSlot(SLoadGameMenu menu)
            {
                this.menu = menu;
                if (Game1.viewport.Width - Game1Patch.xEdge * 2 < 1200)
                {
                    this.mainSlotFont = Game1.smallFont;
                    return;
                }
                this.mainSlotFont = Game1.dialogueFont;
            }

            // Token: 0x06005F1E RID: 24350 RVA: 0x004DD298 File Offset: 0x004DB498
            public void setFont(SpriteFont font)
            {
                this.mainSlotFont = font;
            }

            // Token: 0x06005F1F RID: 24351 RVA: 0x004DD2A1 File Offset: 0x004DB4A1
            public virtual bool isLabelledSlot()
            {
                return false;
            }

            // Token: 0x06005F20 RID: 24352
            public abstract void Activate();

            // Token: 0x06005F21 RID: 24353
            public abstract void Draw(SpriteBatch b, int i);

            // Token: 0x06005F22 RID: 24354 RVA: 0x004DD2A4 File Offset: 0x004DB4A4
            public virtual void Dispose()
            {
            }

            // Token: 0x04003F4E RID: 16206
            public int ActivateDelay;

            // Token: 0x04003F4F RID: 16207
            protected SLoadGameMenu menu;

            // Token: 0x04003F50 RID: 16208
            public SpriteFont mainSlotFont;
        }

        // Token: 0x020007E3 RID: 2019
        public class SaveFileSlot : SLoadGameMenu.MenuSlot
        {
            // Token: 0x06005F23 RID: 24355 RVA: 0x004DD2A8 File Offset: 0x004DB4A8
            public SaveFileSlot(SLoadGameMenu menu, Farmer farmer) : base(menu)
            {
                this.Farmer = farmer;
                this.versionComparison = UtilityPatch.CompareGameVersions(Game1.version, farmer.gameVersion, true, true);
            }

            // Token: 0x06005F24 RID: 24356 RVA: 0x004DD2F6 File Offset: 0x004DB4F6
            public override void Activate()
            {
                SaveGamePatch.Load(this.Farmer.slotName, false, false);
                Game1.exitActiveMenu();
            }

            // Token: 0x06005F25 RID: 24357 RVA: 0x004DD310 File Offset: 0x004DB510
            protected virtual void drawSlotSaveNumber(SpriteBatch b, int i)
            {
                string s = (this.menu.currentItemIndex + i + 1).ToString() + ".";
                int x = this.menu.slotButtons[i].bounds.X + 28 + 32 - SpriteText.getWidthOfString((this.menu.currentItemIndex + i + 1).ToString() + ".", 999999) / 2;
                int y = this.menu.slotButtons[i].bounds.Y + 36;
                SpriteText.drawString(b, s, x, y, 999999, -1, 999999, 1f, 0.88f, false, -1, "", -1, SpriteText.ScrollTextAlignment.Left);
            }

            // Token: 0x06005F26 RID: 24358 RVA: 0x004DD3D5 File Offset: 0x004DB5D5
            protected virtual string slotName()
            {
                return this.Farmer.Name;
            }

            // Token: 0x06005F27 RID: 24359 RVA: 0x004DD3E4 File Offset: 0x004DB5E4
            protected virtual void drawSlotName(SpriteBatch b, int i)
            {
                SpriteText.drawString(b, this.slotName(), this.menu.slotButtons[i].bounds.X + 128 + 36, this.menu.slotButtons[i].bounds.Y + 36, 999999, -1, 999999, 1f, 0.88f, false, -1, "", -1, SpriteText.ScrollTextAlignment.Left);
            }

            // Token: 0x06005F28 RID: 24360 RVA: 0x004DD460 File Offset: 0x004DB660
            protected virtual void drawSlotShadow(SpriteBatch b, int i)
            {
                Vector2 vector = this.portraitOffset();
                b.Draw(Game1.shadowTexture, new Vector2((float)this.menu.slotButtons[i].bounds.X + vector.X + 32f, (float)(this.menu.slotButtons[i].bounds.Y + 128 + 16)), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0f, new Vector2((float)Game1.shadowTexture.Bounds.Center.X, (float)Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, 0.8f);
            }

            // Token: 0x06005F29 RID: 24361 RVA: 0x004DD52B File Offset: 0x004DB72B
            protected virtual Vector2 portraitOffset()
            {
                return new Vector2(92f, 20f);
            }

            // Token: 0x06005F2A RID: 24362 RVA: 0x004DD53C File Offset: 0x004DB73C
            protected virtual void drawSlotFarmer(SpriteBatch b, int i)
            {
                Vector2 vector = this.portraitOffset();
                FarmerRenderer.isDrawingForUI = true;
                this.Farmer.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(0, 0, false, false, null, false), 0, new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 32), new Vector2((float)this.menu.slotButtons[i].bounds.X + vector.X, (float)this.menu.slotButtons[i].bounds.Y + vector.Y), Vector2.Zero, 0.8f, 2, Color.White, 0f, 1f, this.Farmer);
                FarmerRenderer.isDrawingForUI = false;
            }

            // Token: 0x06005F2B RID: 24363 RVA: 0x004DD5F0 File Offset: 0x004DB7F0
            protected virtual void drawSlotDate(SpriteBatch b, int i)
            {
                string text;
                if (this.Farmer.dayOfMonthForSaveGame != null && this.Farmer.seasonForSaveGame != null && this.Farmer.yearForSaveGame != null)
                {
                    text = Utility.getDateStringFor(this.Farmer.dayOfMonthForSaveGame.Value, this.Farmer.seasonForSaveGame.Value, this.Farmer.yearForSaveGame.Value);
                }
                else
                {
                    text = this.Farmer.dateStringForSaveGame;
                }
                Utility.drawTextWithShadow(b, text, this.mainSlotFont, new Vector2((float)(this.menu.slotButtons[i].bounds.X + 128 + 36), (float)(this.menu.slotButtons[i].bounds.Y + 64 + 40)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            }

            // Token: 0x06005F2C RID: 24364 RVA: 0x004DD6E7 File Offset: 0x004DB8E7
            protected virtual string slotSubName()
            {
                return Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11019", this.Farmer.farmName);
            }

            // Token: 0x06005F2D RID: 24365 RVA: 0x004DD704 File Offset: 0x004DB904
            protected virtual void drawSlotSubName(SpriteBatch b, int i)
            {
                string text = this.slotSubName();
                this._position = new Vector2((float)(this.menu.slotButtons[i].bounds.X + this.xBinOffset + this.menu.width) - Game1.dialogueFont.MeasureString(text).X - 98f, (float)(this.menu.slotButtons[i].bounds.Y + 38));
                Utility.drawTextWithShadow(b, text, Game1.dialogueFont, this._position, Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            }

            // Token: 0x06005F2E RID: 24366 RVA: 0x004DD7B4 File Offset: 0x004DB9B4
            protected virtual void drawSlotMoney(SpriteBatch b, int i)
            {
                string text = Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", Utility.getNumberWithCommas(this.Farmer.Money));
                if (this.Farmer.Money == 1 && LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt)
                {
                    text = text.Substring(0, text.Length - 1);
                }
                int num = (int)this.mainSlotFont.MeasureString(text).X;
                this._position.X = this._position.X - (float)(num + 100);
                Utility.drawWithShadow(b, Game1.mouseCursors, this._position, new Microsoft.Xna.Framework.Rectangle(193, 373, 9, 9), Color.White, 0f, Vector2.Zero, 4f, false, 1f, -1, -1, 0.35f);
                this._position.X = this._position.X + 40f;
                if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
                {
                    this._position.Y = this._position.Y + 5f;
                }
                this._position.Y = this._position.Y - 4f;
                Utility.drawTextWithShadow(b, text, this.mainSlotFont, this._position, Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            }

            // Token: 0x06005F2F RID: 24367 RVA: 0x004DD8E0 File Offset: 0x004DBAE0
            protected virtual void drawSlotTimer(SpriteBatch b, int i)
            {
                string hoursMinutesStringFromMilliseconds = Utility.getHoursMinutesStringFromMilliseconds(this.Farmer.millisecondsPlayed);
                int num = (int)this.mainSlotFont.MeasureString(hoursMinutesStringFromMilliseconds).X;
                Vector2 position = new Vector2((float)(this.xBinOffset + this.menu.slotButtons[i].bounds.X + this.menu.width - num - 136), (float)(this.menu.slotButtons[i].bounds.Y + 64 + 36));
                Utility.drawWithShadow(b, Game1.mouseCursors, position, new Microsoft.Xna.Framework.Rectangle(595, 1748, 9, 11), Color.White, 0f, Vector2.Zero, 4f, false, 1f, -1, -1, 0.35f);
                position.X += 40f;
                position.Y += 8f;
                if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
                {
                    position.Y += 5f;
                }
                position.Y -= 4f;
                Utility.drawTextWithShadow(b, hoursMinutesStringFromMilliseconds, this.mainSlotFont, position, Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                position.Y += 4f;
                this._position = position;
            }

            // Token: 0x06005F30 RID: 24368 RVA: 0x004DDA34 File Offset: 0x004DBC34
            public virtual void drawVersionMismatchSlot(SpriteBatch b, int i)
            {
                this.drawSlotSaveNumber(b, i);
                this.drawSlotName(b, i);
                this.drawSlotSubName(b, i);
                string text = this.Farmer.gameVersion;
                if (text == "-1")
                {
                    text = "<1.4";
                }
                string text2 = Game1.content.LoadString("Strings\\UI:VersionMismatch", text);
                Color color = Game1.textColor;
                if (Game1.currentGameTime.TotalGameTime.TotalSeconds < this.redTimer && (int)((this.redTimer - Game1.currentGameTime.TotalGameTime.TotalSeconds) / 0.25) % 2 == 1)
                {
                    color = Color.Red;
                }
                Utility.drawTextWithShadow(b, text2, Game1.dialogueFont, new Vector2((float)(this.menu.slotButtons[i].bounds.X + 128 + 36), (float)(this.menu.slotButtons[i].bounds.Y + 64 + 40)), color, 1f, -1f, -1, -1, 1f, 3);
            }

            // Token: 0x06005F31 RID: 24369 RVA: 0x004DDB44 File Offset: 0x004DBD44
            public override void Draw(SpriteBatch b, int i)
            {
                if (i >= 0 && i < this.menu.slotButtons.Count)
                {
                    if (this.versionComparison < 0)
                    {
                        this.drawVersionMismatchSlot(b, i);
                        return;
                    }
                    this.drawSlotSaveNumber(b, i);
                    this.drawSlotName(b, i);
                    this.drawSlotShadow(b, i);
                    this.drawSlotFarmer(b, i);
                    this.drawSlotDate(b, i);
                    this.drawSlotSubName(b, i);
                    this.drawSlotTimer(b, i);
                    this.drawSlotMoney(b, i);
                }
            }

            // Token: 0x06005F32 RID: 24370 RVA: 0x004DDBBA File Offset: 0x004DBDBA
            public new void Dispose()
            {
                this.Farmer.unload();
            }

            // Token: 0x04003F51 RID: 16209
            public Farmer Farmer;

            // Token: 0x04003F52 RID: 16210
            public double redTimer;

            // Token: 0x04003F53 RID: 16211
            public int versionComparison;

            // Token: 0x04003F54 RID: 16212
            private int xBinOffset = -64 - Game1Patch.xEdge * 2;

            // Token: 0x04003F55 RID: 16213
            private Vector2 _position;
        }
    }
}