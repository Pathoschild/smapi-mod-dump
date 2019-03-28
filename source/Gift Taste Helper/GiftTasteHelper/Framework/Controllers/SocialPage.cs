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

        private SVector2 SlotBoundsOffset;
        private float SlotHeight;
        private Rectangle PageBounds;
        private int LastSlotIndex;

        /// <summary>Fires when the current slot index changes due to scrolling the list.</summary>
        public delegate void SlotIndexChangedDelegate();
        public event SlotIndexChangedDelegate OnSlotIndexChanged;


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

            if (this.FriendSlots.Count == 0)
            {
                Utils.DebugLog("Failed to init SocialPage: No friend slots found.", LogLevel.Error);
                return;
            }

            // The slot bounds begin after a small margin on the top and left side, likely to make it easier to align
            // the slot contents. We need to offset by this margin so that when you mouse over where the slot actually begins
            // it's correctly detected.
            // These offset values are kind of magic based on what looked right as I couldn't find a nice way to get them.
            this.SlotBoundsOffset = new SVector2(Game1.tileSize / 4, Game1.tileSize / 8);
            this.SlotHeight = this.GetSlotHeight();
            this.PageBounds = this.MakePageBounds();
            LastSlotIndex = this.GetSlotIndex();
        }

        public void OnUpdate()
        {
            int slotIndex = this.GetSlotIndex();
            if (slotIndex != this.LastSlotIndex)
            {
                OnSlotIndexChanged.Invoke();
                this.LastSlotIndex = slotIndex;
            }
        }

        public string GetCurrentlyHoveredNpc(SVector2 mousePos)
        {
            int slotIndex = this.GetSlotIndex();
            if (slotIndex < 0 || slotIndex >= this.FriendSlots.Count)
            {
                Utils.DebugLog("SlotIndex is invalid", LogLevel.Error);
                return string.Empty;
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
            return this.Reflection.GetField<int>(this.NativeSocialPage, "slotPosition").GetValue();
        }

        private float GetSlotHeight()
        {
            return (this.FriendSlots[1].bounds.Y - this.FriendSlots[0].bounds.Y);
        }

        // Creates the bounds around all the slots on the screen within the page border.
        private Rectangle MakePageBounds()
        {
            var rect = MakeSlotBounds(this.FriendSlots[0]);
            rect.Height = (int)this.SlotHeight * SDVSocialPage.slotsOnPage;
            return rect;
        }

        private Rectangle MakeSlotBounds(ClickableTextureComponent slot)
        {
            return Utils.MakeRect(
                (slot.bounds.X - this.SlotBoundsOffset.X),
                (slot.bounds.Y - this.SlotBoundsOffset.Y),
                (slot.bounds.Width - Game1.tileSize),
                this.SlotHeight - this.SlotBoundsOffset.Y // account for border between each slot
            );
        }
    }
}
