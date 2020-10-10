/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

namespace Bookcase.Events {

    /// <summary>
    /// Events implemented by Bookcase specifically - using EventBus<T> to allow for priorities. Registration is atypical of C# events, requiring you to manually register the handler via a method rather than using the event notation.
    /// </summary>
    public static class BookcaseEvents {

        /// <summary>
        /// This event is fired when an item tooltip is displayed.
        /// </summary>
        public static EventBus<ItemTooltipEvent> OnItemTooltip = new EventBus<ItemTooltipEvent>();

        /// <summary>
        /// This event is fired when a fish/junk is successfully caught by the player.
        /// </summary>
        public static EventBus<FarmerCaughtFishEvent> FishCaughtInfo = new EventBus<FarmerCaughtFishEvent>();

        /// <summary>
        /// This event is fired when a player gains EXP for a skill.
        /// </summary>
        public static EventBus<FarmerGainExperienceEvent> OnSkillEXPGain = new EventBus<FarmerGainExperienceEvent>();

        /// <summary>
        /// This event is fired when the nightly farm event is selected.
        /// </summary>
        public static EventBus<SelectFarmEvent> SelectFarmEvent = new EventBus<SelectFarmEvent>();

        /// <summary>
        /// This event is fired when the nightly personal event is selected.
        /// </summary>
        public static EventBus<SelectFarmEvent> SelectPersonalEvent = new EventBus<SelectFarmEvent>();

        /// <summary>
        /// This event is fired when a shop menu has been setup. Including inventory and prices.
        /// </summary>
        public static EventBus<ShopSetupEvent> ShopSetupEvent = new EventBus<ShopSetupEvent>();

        /// <summary>
        /// The draw event for StardewValley.Menus.CollectionsPage, can be used to alter the tooltip text.
        /// </summary>
        public static EventBus<CollectionsPageDrawEvent> CollectionsPageDrawEvent = new EventBus<CollectionsPageDrawEvent>();

        /// <summary>
        /// Fired after JunimoNotesMenu.setupBundleSpecificPage. Used to append any logic to the end of the bundle setup. Caution advised due to specifics of method.
        /// </summary>
        public static EventBus<PostBundleSetupEvent> PostBundleSpecificPageSetup = new EventBus<PostBundleSetupEvent>();

        /// <summary>
        /// Fired just before a player starts a new day.
        /// </summary>
        public static EventBus<FarmerStartDayEvent> FarmerStartDayEventPre = new EventBus<FarmerStartDayEvent>();

        /// <summary>
        /// Fired after a player has started a new day.
        /// </summary>
        public static EventBus<FarmerStartDayEvent> FarmerStartDayEventPost = new EventBus<FarmerStartDayEvent>();

        /// <summary>
        /// Fired at start of NPC Gift receive method. (When an NPC is gifted an item)
        /// </summary>
        public static EventBus<NPCReceiveGiftEvent> NPCReceiveGiftPre = new EventBus<NPCReceiveGiftEvent>();

        #region SMAPI Events
        /// <summary>
        /// Stardew Valley's launch tick - fired once per game start.
        /// </summary>
        public static EventBus<Event> FirstGameTick = new EventBus<Event>();

        /// <summary>
        /// Wrapper of SMAPI's GameEvents.QuaterSecondTick - is fired every 250ms.
        /// </summary>
        public static EventBus<Event> GameQuaterSecondTick = new EventBus<Event>();

        /// <summary>
        /// Wrapper of SMAPI's GameEvents.HalfSecondTick - fired every 500ms.
        /// </summary>
        public static EventBus<Event> GameHalfSecondTick = new EventBus<Event>();

        /// <summary>
        /// Wrapper of SMAPI's GameEvents.SecondTick - fired every 1000ms.
        /// </summary>
        public static EventBus<Event> GameFullSecondTick = new EventBus<Event>();
        #endregion
    }
}