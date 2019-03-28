using System;
using System.Collections.Generic;
using System.Diagnostics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using SDVSocialPage = StardewValley.Menus.SocialPage;

namespace GiftTasteHelper.Framework
{
    internal class SocialPageGiftHelper : GiftHelper
    {
        /*********
        ** Properties
        *********/
        private readonly SocialPage SocialPage = new SocialPage();
        private string LastHoveredNpc = string.Empty;


        /*********
        ** Public methods
        *********/
        public SocialPageGiftHelper(IGiftDataProvider dataProvider, GiftConfig config, IReflectionHelper reflection, ITranslationHelper translation)
            : base(GiftHelperType.SocialPage, dataProvider, config, reflection, translation)
        {
            SocialPage.OnSlotIndexChanged += OnSlotIndexChanged;
        }

        public override bool OnOpen(IClickableMenu menu)
        {
            // reset
            this.LastHoveredNpc = string.Empty;

            SDVSocialPage nativeSocialPage = this.GetNativeSocialPage(menu);
            if (nativeSocialPage != null)
                this.SocialPage.Init(nativeSocialPage, this.Reflection);
            return base.OnOpen(menu);
        }

        public override void OnResize(IClickableMenu menu)
        {
            base.OnResize(menu);
            this.SocialPage.OnResize(this.GetNativeSocialPage(menu));
        }

        public override bool CanTick()
        {
            // we don't have a tab-changed event so don't tick when the social tab isn't open
            return this.IsCorrectMenuTab(Game1.activeClickableMenu) && base.CanTick();
        }

        public override void OnCursorMoved(CursorMovedEventArgs e)
        {
            Debug.Assert(this.IsCorrectMenuTab(Game1.activeClickableMenu));
            if (!Utils.Ensure(this.SocialPage != null, "Social Page is null!"))
            {
                return;
            }

            SVector2 mousePos = new SVector2(e.NewPosition.ScreenPixels.X, e.NewPosition.ScreenPixels.Y);
            UpdateHoveredNPC(mousePos);
        }

        public override bool WantsUpdateEvent()
        {
            return true;
        }

        public override void OnPostUpdate(UpdateTickedEventArgs e)
        {
            this.SocialPage.OnUpdate();
        }

        /*********
        ** Protected methods
        *********/
        protected override void AdjustTooltipPosition(ref int x, ref int y, int width, int height, int viewportW, int viewportHeight)
        {
            // Prevent the tooltip from going off screen if we're at the edge
            if (x + width > viewportW)
                x = viewportW - width;
        }

        private bool IsCorrectMenuTab(IClickableMenu menu)
        {
            if (menu is GameMenu gameMenu)
            {
                return gameMenu.currentTab == GameMenu.socialTab;
            }
            return false;
        }

        private SDVSocialPage GetNativeSocialPage(IClickableMenu menu)
        {
            try
            {
                var tabs = this.Reflection.GetField<List<IClickableMenu>>(menu, "pages").GetValue();
                IClickableMenu tab = tabs[GameMenu.socialTab];
                return (SDVSocialPage)tab;
            }
            catch (Exception ex)
            {
                Utils.DebugLog("Failed to get native social page: " + ex, LogLevel.Warn);
                return null;
            }
        }

        private void UpdateHoveredNPC(SVector2 mousePos)
        {
            string hoveredNpc = this.SocialPage.GetCurrentlyHoveredNpc(mousePos);
            if (hoveredNpc == string.Empty)
            {
                this.DrawCurrentFrame = false;
                return;
            }

            if (hoveredNpc != this.LastHoveredNpc)
            {
                if (this.GiftDrawDataProvider.HasDataForNpc(hoveredNpc) &&
                    SetSelectedNPC(hoveredNpc))
                {
                    this.DrawCurrentFrame = true;
                    this.LastHoveredNpc = hoveredNpc;
                }
                else
                {
                    this.DrawCurrentFrame = false;
                    this.LastHoveredNpc = string.Empty;
                }
            }
            else
            {
                this.LastHoveredNpc = string.Empty;
            }
        }

        private void OnSlotIndexChanged()
        {
            // We currently only check if the hovered npc changed during mouse move events, so if the user
            // scrolls the list without moving the mouse the tooltip won't change and it will be incorrect.
            // Listening for when the slot index changes fixes this.
            SVector2 mouse = new SVector2(Game1.getMouseX(), Game1.getMouseY());
            UpdateHoveredNPC(mouse);
        }
    }
}
