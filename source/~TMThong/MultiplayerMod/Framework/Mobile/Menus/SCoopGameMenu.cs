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
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using StardewValley.Network;
using StardewValley.SDKs;
using StardewValley.Menus;
using StardewValley;
using MultiplayerMod.Framework.Patch.Mobile;
using MultiplayerMod.Framework.Network;
using MultiplayerMod.Framework.Mobile.Menus;
using LidgrenClient = MultiplayerMod.Framework.Network.LidgrenClient;

namespace MultiplayerMod.Framework.Mobile.Menus
{
    // Token: 0x020002B5 RID: 693
    public class SCoopGameMenu : SLoadGameMenu
    {
        // Token: 0x0600267B RID: 9851 RVA: 0x002C9BD0 File Offset: 0x002C7DD0
        public SCoopGameMenu(bool isHostMenu) : base(0)
        {
            this.widthMod = (float)Game1.viewport.Width / 1280f;
            this.heightMod = (float)Game1.viewport.Height / 720f;
            this.isHostMenu = isHostMenu;
            this.initializeUpperRightCloseButton();
        }

        // Token: 0x0600267C RID: 9852 RVA: 0x002C9C2F File Offset: 0x002C7E2F
        public override bool readyToClose()
        {
            return base.readyToClose();
        }

        // Token: 0x0600267D RID: 9853 RVA: 0x00002DD5 File Offset: 0x00000FD5
        protected override bool hasDeleteButtons()
        {
            return true;
        }

        // Token: 0x170002F6 RID: 758
        // (get) Token: 0x0600267E RID: 9854 RVA: 0x002C9C41 File Offset: 0x002C7E41
        // (set) Token: 0x0600267F RID: 9855 RVA: 0x002C9C58 File Offset: 0x002C7E58
        protected override List<SLoadGameMenu.MenuSlot> MenuSlots
        {
            get
            {
                if (this.isHostMenu)
                {
                    return this.hostSlots;
                }
                return this.menuSlots;
            }
            set
            {
                if (this.isHostMenu)
                {
                    this.hostSlots = value;
                    return;
                }
                this.menuSlots = value;
            }
        }

        // Token: 0x06002680 RID: 9856 RVA: 0x002C9C71 File Offset: 0x002C7E71
        protected override void startListPopulation()
        {

            this.connectionFinished();

        }


        // Token: 0x06002681 RID: 9857 RVA: 0x002C9C88 File Offset: 0x002C7E88
        protected virtual void connectionFinished()
        {
            this.isSetUp = true;
            string text = Game1.content.LoadString("Strings\\UI:CoopMenu_Refresh");
            int width = (int)Game1.dialogueFont.MeasureString(text).X + 64;
            Vector2 vector = new Vector2(100f, 100f);
            this.refreshButton = new ClickableComponent(new Rectangle((int)vector.X, (int)vector.Y, width, 96), "", text);
            this.hostSlots.Add(new SCoopGameMenu.HostNewFarmSlot(this));
            this.menuSlots.Add(new SCoopGameMenu.LanSlot(this));
            base.startListPopulation();
        }

        // Token: 0x06002682 RID: 9858 RVA: 0x002C9D50 File Offset: 0x002C7F50
        public override void update(GameTime time)
        {
            this.updateCounter++;
            if (!this.isSetUp)
            {

                this.connectionFinishedTimer += time.ElapsedGameTime.TotalSeconds;
                if (this.connectionFinishedTimer >= 2.0)
                {
                    this.connectionFinished();
                }

                return;
            }
            base.update(time);
        }

        // Token: 0x06002683 RID: 9859 RVA: 0x000020A3 File Offset: 0x000002A3
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
        }

        // Token: 0x06002684 RID: 9860 RVA: 0x002C9DBC File Offset: 0x002C7FBC
        protected override void saveFileScanComplete()
        {

        }

        // Token: 0x06002685 RID: 9861 RVA: 0x002C9E14 File Offset: 0x002C8014
        protected virtual SCoopGameMenu.FriendFarmData readLobbyFarmData(object lobby)
        {
            return null;
        }

        // Token: 0x06002686 RID: 9862 RVA: 0x002C9EC9 File Offset: 0x002C80C9
        protected virtual bool checkFriendFarmCompatibility(SCoopGameMenu.FriendFarmData farm)
        {
            return farm.FarmType >= 0 && farm.FarmType <= 5 && farm.ProtocolVersion == Multiplayer.protocolVersion;
        }

        // Token: 0x06002687 RID: 9863 RVA: 0x002C9EF4 File Offset: 0x002C80F4
        protected virtual void onLobbyUpdate(object lobby)
        {

        }

        // Token: 0x06002688 RID: 9864 RVA: 0x002CA14C File Offset: 0x002C834C
        protected override void addSaveFiles(List<Farmer> files)
        {
            this.hostSlots.AddRange(from file in files
                                    where file.slotCanHost
                                    select new SCoopGameMenu.HostFileSlot(this, file));
        }

        // Token: 0x06002689 RID: 9865 RVA: 0x002BA6D6 File Offset: 0x002B88D6
        protected virtual void setMenu(IClickableMenu menu)
        {
            if (Game1.activeClickableMenu is TitleMenu)
            {
                TitleMenu.subMenu = menu;
                return;
            }
            Game1.activeClickableMenu = menu;
        }

        // Token: 0x0600268A RID: 9866 RVA: 0x002CA19C File Offset: 0x002C839C
        private void enterIPPressed()
        {
            string title = Game1.content.LoadString("Strings\\UI:CoopMenu_EnterIP");
            TitleTextInputMenu titleTextInputMenu = new TitleTextInputMenu(title, (address) =>
            {
                enterIP(address);
            }, ModUtilities.ModConfig.LastIP);
            setMenu(titleTextInputMenu);
        }
        public void enterIP(string address)
        {

            try
            {
                StartupPreferences startupPreferences2 = new StartupPreferences();
                startupPreferences2.loadPreferences(false, false);
                startupPreferences2.lastEnteredIP = address;
                startupPreferences2.savePreferences(false, false);
            }
            catch (Exception)
            {
            }

            ModUtilities.ModConfig.LastIP = address;
            ModUtilities.Helper.WriteConfig(ModUtilities.ModConfig);
            //setMenu(new SFarmhandMenu(ModUtilities.multiplayer.InitClient(new ModClient(ModUtilities.ModConfig, address))));
            setMenu(new SFarmhandMenu(ModUtilities.multiplayer.InitClient(new LidgrenClient(address))));
        }



        // Token: 0x0600268B RID: 9867 RVA: 0x002CA1E4 File Offset: 0x002C83E4
        private void enterInviteCodePressed()
        {

        }

        // Token: 0x0600268C RID: 9868 RVA: 0x002CA238 File Offset: 0x002C8438
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!this.isSetUp)
            {
                return;
            }
            if (this.refreshButton.visible && this.refreshButton.containsPoint(x, y))
            {
                Game1.playSound("bigDeSelect");
                this.setMenu(new SCoopGameMenu(this.isHostMenu));
                return;
            }
            base.receiveLeftClick(x, y, playSound);
        }

        // Token: 0x0600268D RID: 9869 RVA: 0x002CA290 File Offset: 0x002C8490
        public override void performHoverAction(int x, int y)
        {
            if (!this.isSetUp)
            {
                return;
            }
            if (this.refreshButton.visible && this.refreshButton.containsPoint(x, y))
            {
                this.refreshButton.scale = 1f;
            }
            else
            {
                this.refreshButton.scale = 0f;
            }
            base.performHoverAction(x, y);
        }

        // Token: 0x0600268E RID: 9870 RVA: 0x0013137F File Offset: 0x0012F57F
        protected override string getStatusText()
        {
            return null;
        }

        // Token: 0x0600268F RID: 9871 RVA: 0x002CA2EC File Offset: 0x002C84EC
        protected override void drawBefore(SpriteBatch b)
        {
            base.drawBefore(b);
            bool flag = this.isSetUp;
        }

        // Token: 0x06002690 RID: 9872 RVA: 0x002CA2FC File Offset: 0x002C84FC
        protected override void drawExtra(SpriteBatch b)
        {
            base.drawExtra(b);
            if (!this.isSetUp)
            {
                return;
            }
            if (this.refreshButton.visible)
            {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(432, 439, 9, 9), this.refreshButton.bounds.X, this.refreshButton.bounds.Y, this.refreshButton.bounds.Width, this.refreshButton.bounds.Height, (this.refreshButton.scale > 0f) ? Color.Wheat : Color.White, 4f, true, 0f);
                Utility.drawTextWithShadow(b, this.refreshButton.label, Game1.dialogueFont, new Vector2((float)this.refreshButton.bounds.Center.X, (float)(this.refreshButton.bounds.Center.Y + 4)) - Game1.dialogueFont.MeasureString(this.refreshButton.label) / 2f, Game1.textColor, 1f, -1f, -1, -1, 0f, 3);
            }
        }

        // Token: 0x06002691 RID: 9873 RVA: 0x002CA430 File Offset: 0x002C8630
        protected override void drawStatusText(SpriteBatch b)
        {
            if (this.getStatusText() != null)
            {
                base.drawStatusText(b);
                return;
            }
            if (!this.isSetUp)
            {
                int num = 1 + 0;
                int num2 = this.updateCounter / 5 % num;
                string value = Game1.content.LoadString("Strings\\UI:CoopMenu_ConnectingOnlineServices");
                this._stringBuilder.Clear();
                this._stringBuilder.Append(value);
                for (int i = 0; i < num2; i++)
                {
                    this._stringBuilder.Append(".");
                }
                string s = this._stringBuilder.ToString();
                for (int j = num2; j < num; j++)
                {
                    this._stringBuilder.Append(".");
                }
                int widthOfString = SpriteText.getWidthOfString(this._stringBuilder.ToString(), 999999);
                SpriteText.drawString(b, s, Game1.graphics.GraphicsDevice.Viewport.Bounds.Center.X - widthOfString / 2, Game1.graphics.GraphicsDevice.Viewport.Bounds.Center.Y, 999999, -1, 999999, 1f, 0.088f, false, -1, "", -1, SpriteText.ScrollTextAlignment.Left);
            }
        }

        // Token: 0x06002692 RID: 9874 RVA: 0x002CA574 File Offset: 0x002C8774
        protected override void Dispose(bool disposing)
        {

            this.lobbyUpdateListener = null;
            base.Dispose(disposing);
        }

        // Token: 0x04001E70 RID: 7792
        public const int region_refresh = 810;

        // Token: 0x04001E71 RID: 7793
        protected List<SLoadGameMenu.MenuSlot> hostSlots = new List<SLoadGameMenu.MenuSlot>();

        // Token: 0x04001E72 RID: 7794
        public ClickableComponent refreshButton;

        // Token: 0x04001E73 RID: 7795
        public ClickableComponent joinTab;

        // Token: 0x04001E74 RID: 7796
        public ClickableComponent hostTab;

        // Token: 0x04001E75 RID: 7797
        private LobbyUpdateListener lobbyUpdateListener;

        // Token: 0x04001E76 RID: 7798
        private bool isSetUp;

        // Token: 0x04001E77 RID: 7799
        private int updateCounter;

        // Token: 0x04001E78 RID: 7800
        private double connectionFinishedTimer;

        // Token: 0x04001E79 RID: 7801
        public bool isHostMenu;

        // Token: 0x04001E7A RID: 7802
        private float widthMod;

        // Token: 0x04001E7B RID: 7803
        private float heightMod;

        // Token: 0x04001E7C RID: 7804
        private StringBuilder _stringBuilder = new StringBuilder();

        // Token: 0x02000470 RID: 1136
        public abstract class SCoopGameMenuSlot : SLoadGameMenu.MenuSlot
        {
            // Token: 0x060035A2 RID: 13730 RVA: 0x003B49E3 File Offset: 0x003B2BE3
            public SCoopGameMenuSlot(SCoopGameMenu menu) : base(menu)
            {
                this.menu = menu;
            }

            // Token: 0x04002F41 RID: 12097
            protected new SCoopGameMenu menu;
        }

        // Token: 0x02000471 RID: 1137
        public abstract class LabeledSlot : SCoopGameMenu.SCoopGameMenuSlot
        {
            // Token: 0x060035A3 RID: 13731 RVA: 0x003B49F3 File Offset: 0x003B2BF3
            public LabeledSlot(SCoopGameMenu menu, string message) : base(menu)
            {
                this.message = message;
            }

            // Token: 0x060035A4 RID: 13732 RVA: 0x0000507A File Offset: 0x0000327A
            public override bool isLabelledSlot()
            {
                return true;
            }

            // Token: 0x060035A5 RID: 13733
            public abstract override void Activate();

            // Token: 0x060035A6 RID: 13734 RVA: 0x003B4A04 File Offset: 0x003B2C04
            public override void Draw(SpriteBatch b, int i)
            {
                int widthOfString = SpriteText.getWidthOfString(this.message, 999999);
                int heightOfString = SpriteText.getHeightOfString(this.message, 999999);
                Rectangle bounds = this.menu.slotButtons[i].bounds;
                int x = bounds.X + (bounds.Width - widthOfString) / 2;
                int y = bounds.Y + (bounds.Height - heightOfString) / 2;
                SpriteText.drawString(b, this.message, x, y, 999999, -1, 999999, 1f, 0.088f, false, -1, "", -1, SpriteText.ScrollTextAlignment.Left);
            }

            // Token: 0x04002F42 RID: 12098
            private string message;
        }

        // Token: 0x02000472 RID: 1138
        public class LanSlot : SCoopGameMenu.LabeledSlot
        {
            // Token: 0x060035A7 RID: 13735 RVA: 0x003B4A9C File Offset: 0x003B2C9C
            public LanSlot(SCoopGameMenu menu) : base(menu, ModUtilities.Helper.Translation.Get("client.join"))
            {
            }

            // Token: 0x060035A8 RID: 13736 RVA: 0x003B4AB4 File Offset: 0x003B2CB4
            public override void Activate()
            {
                this.menu.enterIPPressed();
            }
        }

        // Token: 0x02000473 RID: 1139
        protected class InviteCodeSlot : SCoopGameMenu.LabeledSlot
        {
            // Token: 0x060035A9 RID: 13737 RVA: 0x003B4AC1 File Offset: 0x003B2CC1
            public InviteCodeSlot(SCoopGameMenu menu) : base(menu, Game1.content.LoadString("Strings\\UI:CoopMenu_EnterInviteCode"))
            {
            }

            // Token: 0x060035AA RID: 13738 RVA: 0x003B4AD9 File Offset: 0x003B2CD9
            public override void Activate()
            {

            }
        }

        // Token: 0x02000474 RID: 1140
        public class HostNewFarmSlot : SCoopGameMenu.LabeledSlot
        {
            // Token: 0x060035AB RID: 13739 RVA: 0x003B4AE6 File Offset: 0x003B2CE6
            public HostNewFarmSlot(SCoopGameMenu menu) : base(menu, Game1.content.LoadString("Strings\\UI:CoopMenu_HostNewFarm"))
            {

            }

            // Token: 0x060035AC RID: 13740 RVA: 0x003B4B09 File Offset: 0x003B2D09
            public override void Activate()
            {
                Game1.resetPlayer();

                TitleMenu.subMenu = new SCharacterCustomization(source: CharacterCustomization.Source.HostNewFarm);
                Game1.changeMusicTrack("CloudCountry", false, Game1.MusicContext.Default);

            }
        }

        

        // Token: 0x02000475 RID: 1141
        public class HostFileSlot : SLoadGameMenu.SaveFileSlot
        {
            // Token: 0x060035AD RID: 13741 RVA: 0x003B4B1D File Offset: 0x003B2D1D
            public HostFileSlot(SCoopGameMenu menu, Farmer farmer) : base(menu, farmer)
            {
                this.menu = menu;
                this.ActivateDelay = 2150;
            }

            // Token: 0x060035AE RID: 13742 RVA: 0x003B4428 File Offset: 0x003B2628
            public override void Activate()
            {
                Game1.multiplayerMode = 2;
                base.Activate();
            }

            // Token: 0x060035AF RID: 13743 RVA: 0x000020A3 File Offset: 0x000002A3
            protected override void drawSlotSaveNumber(SpriteBatch b, int i)
            {
            }

            // Token: 0x060035B0 RID: 13744 RVA: 0x003B4436 File Offset: 0x003B2636
            protected override string slotName()
            {
                return Game1.content.LoadString("Strings\\UI:CoopMenu_HostFile", this.Farmer.Name, this.Farmer.farmName.Value);
            }

            // Token: 0x060035B1 RID: 13745 RVA: 0x003B4462 File Offset: 0x003B2662
            protected override string slotSubName()
            {
                return this.Farmer.Name;
            }

            // Token: 0x060035B2 RID: 13746 RVA: 0x003B446F File Offset: 0x003B266F
            protected override Vector2 portraitOffset()
            {
                return base.portraitOffset() - new Vector2(32f, 0f);
            }

            // Token: 0x04002F43 RID: 12099
            protected new SCoopGameMenu menu;
        }

        // Token: 0x02000476 RID: 1142
        protected class FriendFarmData
        {
            // Token: 0x04002F44 RID: 12100
            public object Lobby;

            // Token: 0x04002F45 RID: 12101
            public string OwnerName;

            // Token: 0x04002F46 RID: 12102
            public string FarmName;

            // Token: 0x04002F47 RID: 12103
            public int FarmType;

            // Token: 0x04002F48 RID: 12104
            public WorldDate Date;

            // Token: 0x04002F49 RID: 12105
            public bool PreviouslyJoined;

            // Token: 0x04002F4A RID: 12106
            public string ProtocolVersion;
        }

        // Token: 0x02000477 RID: 1143
        protected class FriendFarmSlot : SCoopGameMenu.SCoopGameMenuSlot
        {
            // Token: 0x060035B4 RID: 13748 RVA: 0x003B4B2E File Offset: 0x003B2D2E
            public FriendFarmSlot(SCoopGameMenu menu, SCoopGameMenu.FriendFarmData farm) : base(menu)
            {
                this.Farm = farm;
            }

            // Token: 0x060035B5 RID: 13749 RVA: 0x003B4B3E File Offset: 0x003B2D3E
            public bool MatchAddress(object Lobby)
            {
                return object.Equals(this.Farm.Lobby, Lobby);
            }

            // Token: 0x060035B6 RID: 13750 RVA: 0x003B4B51 File Offset: 0x003B2D51
            public void Update(SCoopGameMenu.FriendFarmData newData)
            {
                this.Farm = newData;
            }

            // Token: 0x060035B7 RID: 13751 RVA: 0x003B4B5A File Offset: 0x003B2D5A
            public override void Activate()
            {

            }

            // Token: 0x060035B8 RID: 13752 RVA: 0x003B4B88 File Offset: 0x003B2D88
            protected virtual string slotName()
            {
                return "";
            }

            // Token: 0x060035B9 RID: 13753 RVA: 0x003B4BC8 File Offset: 0x003B2DC8
            protected virtual void drawSlotName(SpriteBatch b, int i)
            {
                SpriteText.drawString(b, this.slotName(), this.menu.slotButtons[i].bounds.X + 128 + 36, this.menu.slotButtons[i].bounds.Y + 36, 999999, -1, 999999, 1f, 0.088f, false, -1, "", -1, SpriteText.ScrollTextAlignment.Left);
            }

            // Token: 0x060035BA RID: 13754 RVA: 0x003B4C44 File Offset: 0x003B2E44
            protected virtual void drawSlotDate(SpriteBatch b, int i)
            {
                Utility.drawTextWithShadow(b, this.Farm.Date.Localize(), Game1.dialogueFont, new Vector2((float)(this.menu.slotButtons[i].bounds.X + 128 + 32), (float)(this.menu.slotButtons[i].bounds.Y + 64 + 40)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            }

            // Token: 0x060035BB RID: 13755 RVA: 0x003B4CD0 File Offset: 0x003B2ED0
            protected virtual void drawSlotFarm(SpriteBatch b, int i)
            {
                Rectangle rectangle = new Rectangle(22 * this.Farm.FarmType, 324, 22, 20);
                Texture2D mouseCursors = Game1.mouseCursors;
                Rectangle rectangle2 = new Rectangle(this.menu.slotButtons[i].bounds.X, this.menu.slotButtons[i].bounds.Y, 160, this.menu.slotButtons[i].bounds.Height);
                Rectangle destinationRectangle = new Rectangle(rectangle2.X + (rectangle2.Width - rectangle.Width * 4) / 2, rectangle2.Y + (rectangle2.Height - rectangle.Height * 4) / 2, rectangle.Width * 4, rectangle.Height * 4);
                b.Draw(mouseCursors, destinationRectangle, new Rectangle?(rectangle), Color.White);
            }

            // Token: 0x060035BC RID: 13756 RVA: 0x003B4DB8 File Offset: 0x003B2FB8
            protected virtual void drawSlotOwnerName(SpriteBatch b, int i)
            {
                Utility.drawTextWithShadow(b, this.Farm.OwnerName, Game1.dialogueFont, new Vector2((float)(this.menu.slotButtons[i].bounds.X + this.menu.width - 128) - Game1.dialogueFont.MeasureString(this.Farm.OwnerName).X, (float)(this.menu.slotButtons[i].bounds.Y + 44)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            }

            // Token: 0x060035BD RID: 13757 RVA: 0x003B4E5F File Offset: 0x003B305F
            public override void Draw(SpriteBatch b, int i)
            {
                this.drawSlotName(b, i);
                this.drawSlotDate(b, i);
                this.drawSlotFarm(b, i);
                this.drawSlotOwnerName(b, i);
            }

            // Token: 0x04002F4B RID: 12107
            public SCoopGameMenu.FriendFarmData Farm;
        }

        // Token: 0x02000478 RID: 1144

    }
}
