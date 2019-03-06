using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using SDVSocialPage = StardewValley.Menus.SocialPage;

namespace GiftTasteHelper.Framework
{
    internal class SocialPage
    {
        /*********
        ** Properties
        *********/
        /// <summary>The underlying social menu.</summary>
        private SDVSocialPage NativeSocialPage;

        /// <summary>Simplifies access to private game code.</summary>
        private IReflectionHelper Reflection;

        private List<ClickableTextureComponent> FriendSlots;
        private List<object> Names; // Other player names are ints, NPC names are strings.

        private SVector2 Offset;
        private float SlotHeight;
        private float Zoom;
        private Rectangle PageBounds;
        private int LastSlotIndex;


        /*********
        ** Public methods
        *********/
        public void Init(SDVSocialPage nativePage, IReflectionHelper reflection)
        {
            this.Reflection = reflection;
            this.OnResize(nativePage);
        }

        public void OnResize(SDVSocialPage nativePage)
        {
            this.NativeSocialPage = nativePage;
            this.FriendSlots = this.Reflection.GetField<List<ClickableTextureComponent>>(this.NativeSocialPage, "sprites").GetValue();
            this.Names = this.Reflection.GetField<List<object>>(this.NativeSocialPage, "names").GetValue();

            // Mostly arbitrary since there's no nice way (that i know of) to get the slots positioned correctly...
            this.Offset = new SVector2(Game1.tileSize / 4, Game1.tileSize / 8);
            this.Zoom = Game1.options.zoomLevel;
            this.SlotHeight = this.GetSlotHeight();
            this.LastSlotIndex = -1; // Invalidate
        }

        public string GetCurrentlyHoveredNpc(SVector2 mousePos)
        {
            int slotIndex = this.GetSlotIndex();
            if (slotIndex < 0 || slotIndex >= this.FriendSlots.Count)
            {
                Utils.DebugLog("SlotIndex is invalid", LogLevel.Error);
                return string.Empty;
            }

            // Remake the page bounds if the slot index has changed
            // TODO: we can probably just do this once on resize with slot 0
            if (slotIndex != this.LastSlotIndex)
            {
                this.PageBounds = this.MakeBounds(slotIndex);
                this.LastSlotIndex = slotIndex;
            }

            // Early out if the mouse isn't within the page bounds
            Point mousePoint = mousePos.ToPoint();
            if (!this.PageBounds.Contains(mousePoint))
            {
                return string.Empty;
            }

            // Find the slot containing the cursor among the currently visible slots
            string hoveredFriendName = string.Empty;
            for (int i = slotIndex; i < slotIndex + SDVSocialPage.slotsOnPage; ++i)
            {
                var friend = this.FriendSlots[i];
                var bounds = this.MakeSlotBounds(friend);

                if (bounds.Contains(mousePoint) && Utils.Ensure(i < this.Names.Count, "Name index out of range"))
                {
                    hoveredFriendName = this.Names[i] as string;
                    break;
                }
            }

            return hoveredFriendName ?? string.Empty;
        }

        private int GetSlotIndex()
        {
            if (this.NativeSocialPage != null)
                return this.Reflection.GetField<int>(this.NativeSocialPage, "slotPosition").GetValue();
            return -1;
        }

        private float GetSlotHeight()
        {
            if (this.FriendSlots.Count > 1)
            {
                return (this.FriendSlots[1].bounds.Y - this.FriendSlots[0].bounds.Y);
            }
            return -1f;
        }

        private Rectangle MakeBounds(int slotIndex)
        {
            // Subtrace tilesize from the width so it's not too wide. Sucks but not easy way around it
            float x = (this.FriendSlots[slotIndex].bounds.X - this.Offset.X) * this.Zoom;
            float y = (this.FriendSlots[slotIndex].bounds.Y - this.Offset.Y) * this.Zoom;
            float width = (this.FriendSlots[slotIndex].bounds.Width - Game1.tileSize) * this.Zoom;
            float height = (this.SlotHeight * SDVSocialPage.slotsOnPage) * this.Zoom;
            return Utils.MakeRect(x, y, width, height);
        }

        private Rectangle MakeSlotBounds(ClickableTextureComponent slot)
        {
            return Utils.MakeRect(
                (slot.bounds.X - this.Offset.X) * this.Zoom,
                (slot.bounds.Y - this.Offset.Y) * this.Zoom,
                (slot.bounds.Width - Game1.tileSize) * this.Zoom,
                this.SlotHeight * this.Zoom
            );
        }
    }
}
