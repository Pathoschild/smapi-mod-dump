namespace GiftTasteHelper.Framework
{
    /// <summary>Delegate for the GiftGiven event.</summary>
    /// <param name="npc">Name of the NPC the gift was given to.</param>
    /// <param name="itemId">The item id (parentSheetIndex) of the item that was given.</param>
    delegate void GiftGivenDelegate(string npc, int itemId);

    /// <summary>Monitors and alerts when a gift is given to an npc.</summary>
    internal interface IGiftMonitor
    {
        /// <summary>Invoked when a gift is given to an npc.</summary>
        event GiftGivenDelegate GiftGiven;

        bool IsHoldingValidGift { get; }

        /// <summary>Initializes the GiftMonitor.</summary>
        void Load();

        /// <summary>Resets the tracking of who has been given gifts today. Should be called after load.</summary>
        void Reset();

        /// <summary>Sets the internally held item to what the player is currently holding. Should be called when right click it pressed.</summary>
        void UpdateHeldGift();

        /// <summary>Checks if a gift has been given to an npc and invokes the GiftGiven event if so. Should be called on right mouse up.</summary>
        void CheckGiftGiven();
    }
}
