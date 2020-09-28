using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Linq;
using static SunscreenMod.Flags;

namespace SunscreenMod
{
    /// <summary>Manages use and application of sunscreen and aloe gel lotions.</summary>
    public class Lotions
    {
        static IModHelper Helper => ModEntry.Instance.Helper;
        static IMonitor Monitor => ModEntry.Instance.Monitor;
        static ModConfig Config => ModConfig.Instance;


        static readonly ITranslationHelper i18n = Helper.Translation;

        /// <summary>Slimmed-down Json Assets API</summary>
        static readonly JsonAssets.IApi JA = ModEntry.Instance.JA;

        /// <summary>The item IDs of all lotions added by this mod.</summary>
        public int[] LotionIDs { get; private set; }

        /// <summary>The item name for sunscreen (matches JA content pack).</summary>
        const string SUNSCREEN_ITEM_NAME = "SPF60 Sunscreen";
        /// <summary>The item name for aloe gel (matches JA content pack)</summary>
        const string ALOE_GEL_ITEM_NAME = "Aloe Vera Gel";

        /// <summary>The mail flag to store data on whether aloe gel has already been applied.</summary>
        const string APPLIED_ALOE_FLAG = "HasAppliedAloeToday";

        /// <summary>If character has already applied aloe gel today. Getter handles initialization, setter handles mail flags.</summary>
        public bool HasAppliedAloeToday
        {
            get
            {
                if (_hasAppliedAloeToday == null)
                {
                    _hasAppliedAloeToday = HasFlag(APPLIED_ALOE_FLAG);
                }
                return (bool)_hasAppliedAloeToday;
            }
            set
            {
                if (value)
                {
                    AddFlag(APPLIED_ALOE_FLAG);
                }
                else
                {
                    RemoveFlag(APPLIED_ALOE_FLAG);
                }
                _hasAppliedAloeToday = value;
            }
        }
        private bool? _hasAppliedAloeToday = null;

        /// <summary>Checks if an item is a lotion type added by this mod.</summary>
        /// <param name="item">The Item (or SDV Object) to check</param>
        /// <returns>true if lotion, false otherwise</returns>
        public bool IsLotion(Item item)
        {
            if (item is StardewValley.Object &&
                !(item as StardewValley.Object).bigCraftable.Value &&
                !(item is Wallpaper) &&
                !(item is Furniture))
            {
                LotionIDs = new int[] {
                    JA.GetObjectId(SUNSCREEN_ITEM_NAME),
                    JA.GetObjectId(ALOE_GEL_ITEM_NAME)
                };
                if (LotionIDs.Contains(item.ParentSheetIndex))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Prompt the player to apply a lotion item, or display a message if not allowed.</summary>
        /// <param name="item">The lotion item to check or apply</param>
        public void ApplyQuestion(Item item)
        {
            //Check if it can be applied (Aloe Vera Gel can only be applied once a day)
            if (item.ParentSheetIndex == JA.GetObjectId(ALOE_GEL_ITEM_NAME) && HasAppliedAloeToday)
            {
                string messagetext = i18n.Get("Error.AlreadyUsedLotionToday", new { lotionName = item.DisplayName });
                Game1.drawDialogueNoTyping(messagetext);
                Monitor.Log($"Can't use item: {messagetext}", LogLevel.Info);
                return;
            }

            //Check if it can be applied (Sunscreen can be applied once every 30 minutes, unless it washed off)
            if (item.ParentSheetIndex == JA.GetObjectId(SUNSCREEN_ITEM_NAME) && ModEntry.Instance.Sunscreen.AppliedSunscreenRecently())
            {
                string messagetext = i18n.Get("Error.UsedLotionVeryRecently", new { lotionName = item.DisplayName });
                Game1.drawDialogueNoTyping(messagetext);
                Monitor.Log($"Can't use item: {messagetext}", LogLevel.Info);
                return;
            }

            string question = i18n.Get("Question.ApplyLotion", new { lotionName = item.DisplayName });
            Response[] yesNoResponses = Game1.currentLocation.createYesNoResponses();
            GameLocation.afterQuestionBehavior afterAnswer = ApplyLotionAnswer;

            Game1.currentLocation.createQuestionDialogue(question, yesNoResponses, afterAnswer);
        }

        /// <summary>Delegate function to use a lotion item on a player if they respond "Yes" to the prompt.</summary>
        /// <param name="who">The player who should use the lotion</param>
        /// <param name="whichAnswer">The answer chosen at the ApplyQuestion prompt</param>
        public void ApplyLotionAnswer(Farmer who, string whichAnswer)
        {
            if (Config.DebugMode) Monitor.Log($"Player chose answer: {whichAnswer}", LogLevel.Debug);
            if (whichAnswer == "No")
            {
                return; //Don't use any lotion
            }

            if (Game1.player.ActiveObject.ParentSheetIndex == JA.GetObjectId(SUNSCREEN_ITEM_NAME))
            {
                ApplySunscreen();
            }
            else if (Game1.player.ActiveObject.ParentSheetIndex == JA.GetObjectId(ALOE_GEL_ITEM_NAME))
            {
                ApplyAloeGel();
            }
            //remove one from the stack
            who.reduceActiveItemByOne();
            who.jump(4.0f); //Default jump is 8f
            DelayedAction.playSoundAfterDelay("slimedead", 100);
        }

        /// <summary>Apply sunscreen protection to the current player, including an HUD message.</summary>
        public void ApplySunscreen()
        {
            //apply the sunscreen's effects
            ModEntry.Instance.Sunscreen.ApplySunscreen();
            string messagetext = i18n.Get("Apply.Sunscreen", new { hours = Config.SunscreenDuration });
            Game1.addHUDMessage(new HUDMessage(messagetext, 2)); //Exclamation mark message type
            Monitor.Log($"Applied sunscreen: {messagetext}", LogLevel.Info);

            //TODO: make skin briefly white????
        }

        /// <summary>Apply aloe vera gel to the specified player, reducing burn level and healing accordingly with an HUD message.</summary>
        /// <param name="who">The player to be treated with aloe gel</param>
        public void ApplyAloeGel(Farmer who = null)
        {
            who = who ?? Game1.player;
            Sunburn burn = ModEntry.Instance.Burn;

            int initialLevel = burn.SunburnLevel;

            //apply the gel's effects
            burn.SunburnLevel -= 1;

            //if the gel had an effect
            if (initialLevel != burn.SunburnLevel)
            {
                who.health = Math.Min(who.maxHealth, who.health + Config.HealthLossPerLevel);
                who.Stamina += (float)Config.EnergyLossPerLevel;
                if (Config.DebugMode) Monitor.Log($"Current health: {who.health}/{who.maxHealth} | Current stamina: {who.Stamina}/{who.MaxStamina}", LogLevel.Info);

                Item gel = Game1.player.ActiveObject;
                string messagetext = i18n.Get("Apply.AloeGel", new { lotionName = gel.DisplayName });
                Game1.addHUDMessage(new HUDMessage(messagetext, 4)); //stamina_type HUD message
                Monitor.Log($"Applied aloe gel: {messagetext}", LogLevel.Info);

                burn.DisplaySunburnStatus(); //Info: new sunburn level, or healed if healed
            }
            else
            {
                string messagetext = i18n.Get("Apply.AloeNoEffect");
                Game1.addHUDMessage(new HUDMessage(messagetext, 2)); //Exclamation mark message type
                Monitor.Log($"Applied aloe gel: {messagetext}", LogLevel.Info);
            }

            //TODO: make skin briefly green????

            //Prevent using aloe again today
            HasAppliedAloeToday = true;
        }
    }
}