/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/GiftTasteHelper
**
*************************************************/

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
        private SDVSocialPage? NativeSocialPage;

        /// <summary>Simplifies access to private game code.</summary>
        private IReflectionHelper? Reflection;

        private List<ClickableTextureComponent> FriendSlots = new();

        private int FirstCharacterIndex;
        private SVector2 SlotBoundsOffset = SVector2.Zero;
        private float SlotHeight;
        private Rectangle PageBounds;
        private int LastSlotIndex;

        /// <summary>Fires when the current slot index changes due to scrolling the list.</summary>
        public delegate void SlotIndexChangedDelegate();
        public event SlotIndexChangedDelegate? OnSlotIndexChanged;


        /*********
        ** Public methods
        *********/
        public void Init(SDVSocialPage nativePage, IReflectionHelper reflection)
        {
            this.Reflection = reflection;
            this.OnResize(nativePage);
        }

        public void OnResize(SDVSocialPage? nativePage)
        {
            this.NativeSocialPage = nativePage;
            if (this.NativeSocialPage is null)
            {
                return;
            }

            this.FriendSlots = this.Reflection?.GetField<List<ClickableTextureComponent>>(this.NativeSocialPage, "sprites").GetValue() ?? new();

            // Find the first NPC character slot
            this.FirstCharacterIndex = this.NativeSocialPage.SocialEntries.FindIndex(entry => !entry.IsPlayer);
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
                OnSlotIndexChanged?.Invoke();
                this.LastSlotIndex = slotIndex;
            }
        }

        public string GetCurrentlyHoveredNpc(SVector2 mousePos)
        {
            int slotIndex = this.GetSlotIndex();
            if (slotIndex < 0 || slotIndex >= this.FriendSlots.Count || this.NativeSocialPage is null)
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

                if (bounds.Contains(mousePoint) && Utils.Ensure(i < this.NativeSocialPage.SocialEntries.Count, "Name index out of range"))
                {
                    hoveredFriendName = this.NativeSocialPage.SocialEntries[i].InternalName;
                    break;
                }
            }

            return hoveredFriendName ?? string.Empty;
        }

        private int GetSlotIndex()
        {
            if (this.NativeSocialPage is null || this.Reflection is null)
            {
                return 0;
            }
            return this.Reflection.GetField<int>(this.NativeSocialPage, "slotPosition").GetValue();
        }

        private float GetSlotHeight()
        {
            return (this.FriendSlots[this.FirstCharacterIndex + 1].bounds.Y - this.FriendSlots[this.FirstCharacterIndex].bounds.Y);
        }

        // Creates the bounds around all the slots on the screen within the page border.
        private Rectangle MakePageBounds()
        {
            var rect = MakeSlotBounds(this.FriendSlots[this.FirstCharacterIndex]);
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
