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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using StardewValley.Network;
using StardewValley.Menus;
using StardewValley;
namespace MultiplayerMod.Framework.Mobile.Menus
{
    // Token: 0x020002A5 RID: 677
    public class SFarmhandMenu : SLoadGameMenu
    {
        // Token: 0x060025B7 RID: 9655 RVA: 0x002BEFE3 File Offset: 0x002BD1E3
        public SFarmhandMenu() : this(null)
        {
        }

        // Token: 0x060025B8 RID: 9656 RVA: 0x002BEFEC File Offset: 0x002BD1EC
        public SFarmhandMenu(Client client) : base(0)
        {
            this.initializeUpperRightCloseButton();
            this.client = client;
            if (client != null)
            {
                this.gettingFarmhands = true;
            }
        }


        public void receiveLeftClick1(int x, int y, bool playSound = true)
        {

            if (upperRightCloseButton.bounds.Contains(x, y))
            {
                if (Game1.activeClickableMenu is TitleMenu titleMenu)
                {
                    TitleMenu.subMenu = null;
                }
                else
                {
                    Game1.activeClickableMenu = null;
                }
                return;
            }

            base.receiveLeftClick(x, y, playSound);
        }

        public void releaseLeftClick1(int x, int y)
        {
            if (upperRightCloseButton.bounds.Contains(x, y))
            {
                if (Game1.activeClickableMenu is TitleMenu titleMenu)
                {
                    TitleMenu.subMenu = null;
                }
                else
                {
                    Game1.activeClickableMenu = null;
                }
                return;
            }
            base.releaseLeftClick(x, y);
        }

        // Token: 0x060025B9 RID: 9657 RVA: 0x0000507A File Offset: 0x0000327A
        public override bool readyToClose()
        {
            return this.client.timedOut;
        }

        // Token: 0x060025BA RID: 9658 RVA: 0x00002DD5 File Offset: 0x00000FD5
        protected override bool hasDeleteButtons()
        {
            return false;
        }

        // Token: 0x060025BB RID: 9659 RVA: 0x000020A3 File Offset: 0x000002A3
        protected override void startListPopulation()
        {

        }

        // Token: 0x060025BC RID: 9660 RVA: 0x002BF028 File Offset: 0x002BD228
        protected override bool checkListPopulation()
        {
            if (this.client != null && (this.gettingFarmhands || this.approvingFarmhand) && (this.client.availableFarmhands != null || this.client.connectionMessage != null))
            {
                this.timerToLoad = 0;
                this.selected = -1;
                this.loading = false;
                this.gettingFarmhands = false;
                if (this.menuSlots == null)
                {
                    this.menuSlots = new List<SLoadGameMenu.MenuSlot>();
                }
                else
                {
                    this.menuSlots.Clear();
                }
                if (this.client.availableFarmhands == null)
                {
                    this.approvingFarmhand = true;
                }
                else
                {
                    this.approvingFarmhand = false;
                    this.menuSlots.AddRange(from farmer in this.client.availableFarmhands
                                            select new SFarmhandMenu.FarmhandSlot(this, farmer));
                }
                if (Game1.activeClickableMenu is TitleMenu)
                {
                    Game1.gameMode = 0;
                }
                else
                {
                    Game1.gameMode = 3;
                }
            }
            return false;
        }

        // Token: 0x060025BD RID: 9661 RVA: 0x002BF10C File Offset: 0x002BD30C
        public override void update(GameTime time)
        {
            if (this.client != null)
            {
                if (!this.client.connectionStarted && this.drawn)
                {
                    this.client.connect();
                }
                if (this.client.connectionStarted)
                {
                    this.client.receiveMessages();
                }
                if (this.client.readyToPlay)
                {
                    Game1.gameMode = 3;
                    this.loadClientOptions();
                    if (Game1.activeClickableMenu is SFarmhandMenu || (Game1.activeClickableMenu is TitleMenu && TitleMenu.subMenu is SFarmhandMenu))
                    {
                        Game1.exitActiveMenu();
                    }
                    var property = Game1.currentLocation.GetType().GetProperty("tapToMove");
                    object TapToMove = typeof(IClickableMenu).Assembly.GetType("StardewValley.Mobile.TapToMove").CreateInstance<object>(new object[] { Game1.currentLocation });
                    property.SetValue(Game1.currentLocation, TapToMove);
                    Game1.multiplayerMode = Game1.multiplayerClient;
                }
                else if (this.client.timedOut)
                {
                    if (this.approvingFarmhand)
                    {
                        ModUtilities.multiplayer.clientRemotelyDisconnected(Multiplayer.IsTimeout(this.client.pendingDisconnect) ? Multiplayer.DisconnectType.Timeout_FarmhandSelection : this.client.pendingDisconnect);
                    }
                    else
                    {
                        this.menuSlots.RemoveAll((SLoadGameMenu.MenuSlot slot) => slot is SFarmhandMenu.FarmhandSlot);
                    }
                }
            }
            base.update(time);
        }

        // Token: 0x060025BE RID: 9662 RVA: 0x002BF217 File Offset: 0x002BD417
        private void loadClientOptions()
        {
            new Task(delegate ()
            {
                StartupPreferences startupPreferences = new StartupPreferences();
                ModUtilities.Helper.Reflection.GetMethod(startupPreferences, "loadPreferences").Invoke(false, false, false);
                Game1.options = startupPreferences.clientOptions;
                Game1.initializeVolumeLevels();
            }).Start();
        }

        // Token: 0x060025BF RID: 9663 RVA: 0x002BF244 File Offset: 0x002BD444
        protected override string getStatusText()
        {
            try
            {
                if (this.client == null)
                {
                    return Game1.content.LoadString("Strings\\UI:CoopMenu_NoInvites");
                }
                if (this.client.timedOut)
                {
                    return Game1.content.LoadString("Strings\\UI:CoopMenu_Failed");
                }
                if (this.client.connectionMessage != null)
                {
                    return Game1.content.LoadString(this.client.connectionMessage);
                }
                if (this.gettingFarmhands || this.approvingFarmhand)
                {
                    return Game1.content.LoadString("Strings\\UI:CoopMenu_Connecting");
                }
                if (this.menuSlots.Count == 0)
                {
                    return Game1.content.LoadString("Strings\\UI:CoopMenu_NoSlots");
                }
            }
            catch (Exception ex)
            {
                if (this.client == null)
                {
                    return "Strings\\UI:CoopMenu_NoInvites";
                }
                if (this.client.timedOut)
                {
                    return "Strings\\UI:CoopMenu_Failed";
                }
                if (this.client.connectionMessage != null)
                {
                    return this.client.connectionMessage;
                }
                if (this.gettingFarmhands || this.approvingFarmhand)
                {
                    return "Strings\\UI:CoopMenu_Connecting";
                }
                if (this.menuSlots.Count == 0)
                {
                    return "Strings\\UI:CoopMenu_NoSlots";
                }
            }
            return null;
        }

        // Token: 0x060025C0 RID: 9664 RVA: 0x002BF2E8 File Offset: 0x002BD4E8
        protected override void Dispose(bool disposing)
        {
            if (this.client != null && disposing && Game1.client != this.client)
            {
                Multiplayer.LogDisconnect(Multiplayer.IsTimeout(this.client.pendingDisconnect) ? Multiplayer.DisconnectType.Timeout_FarmhandSelection : Multiplayer.DisconnectType.ExitedToMainMenu_FromFarmhandSelect);
                this.client.disconnect(true);
            }
            base.Dispose(disposing);
        }

        // Token: 0x04001DBF RID: 7615
        public bool gettingFarmhands;

        // Token: 0x04001DC0 RID: 7616
        public bool approvingFarmhand;

        // Token: 0x04001DC1 RID: 7617
        public Client client;

        // Token: 0x0200046B RID: 1131
        public class FarmhandSlot : SLoadGameMenu.SaveFileSlot
        {
            // Token: 0x06003596 RID: 13718 RVA: 0x003B4816 File Offset: 0x003B2A16
            public FarmhandSlot(SFarmhandMenu menu, Farmer farmer) : base(menu, farmer)
            {
                this.menu = menu;
            }

            // Token: 0x06003597 RID: 13719 RVA: 0x003B4828 File Offset: 0x003B2A28
            public override void Activate()
            {
                if (this.menu.client != null)
                {
                    Game1.loadForNewGame(false);
                    Game1.player = this.Farmer;
                    this.menu.client.availableFarmhands = null;
                    this.menu.client.sendPlayerIntroduction();
                    this.menu.menuSlots.Clear();
                    this.menu.approvingFarmhand = true;
                    Game1.gameMode = 6;
                }
            }

            // Token: 0x06003598 RID: 13720 RVA: 0x003B48A4 File Offset: 0x003B2AA4
            protected override void drawSlotName(SpriteBatch b, int i)
            {
                if (this.Farmer.isCustomized)
                {
                    base.drawSlotName(b, i);
                    return;
                }
                string s = Game1.content.LoadString("Strings\\UI:CoopMenu_NewFarmhand");
                SpriteText.drawString(b, s, this.menu.slotButtons[i].bounds.X + 128 + 36, this.menu.slotButtons[i].bounds.Y + 36, 999999, -1, 999999, 1f, 0.088f, false, -1, "", -1, SpriteText.ScrollTextAlignment.Left);
            }

            // Token: 0x06003599 RID: 13721 RVA: 0x003B4943 File Offset: 0x003B2B43
            protected override void drawSlotShadow(SpriteBatch b, int i)
            {
                if (this.Farmer.isCustomized)
                {
                    base.drawSlotShadow(b, i);
                }
            }

            // Token: 0x0600359A RID: 13722 RVA: 0x003B495F File Offset: 0x003B2B5F
            protected override void drawSlotFarmer(SpriteBatch b, int i)
            {
                if (this.Farmer.isCustomized)
                {
                    base.drawSlotFarmer(b, i);
                }
            }

            // Token: 0x0600359B RID: 13723 RVA: 0x003B497B File Offset: 0x003B2B7B
            protected override void drawSlotTimer(SpriteBatch b, int i)
            {
                if (this.Farmer.isCustomized)
                {
                    base.drawSlotTimer(b, i);
                }
            }

            // Token: 0x0600359C RID: 13724 RVA: 0x000020A3 File Offset: 0x000002A3
            protected override void drawSlotMoney(SpriteBatch b, int i)
            {
            }

            // Token: 0x04002F2E RID: 12078
            protected new SFarmhandMenu menu;
        }
    }
}
