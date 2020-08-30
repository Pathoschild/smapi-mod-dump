using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SunscreenMod.Flags;
using Object = StardewValley.Object;

namespace SunscreenMod
{
    class Lotions
    {
        protected static IModHelper Helper => ModEntry.Instance.Helper;
        protected static IMonitor Monitor => ModEntry.Instance.Monitor;
        private static ModConfig Config => ModConfig.Instance;


        protected static ITranslationHelper i18n = Helper.Translation;

        protected static JsonAssets.IApi JA = ModEntry.Instance.JA;

        public int[] LotionIDs { get; private set; }

        //If character has already applied aloe gel today
        public bool HasAppliedAloeToday
        {
            get
            {
                if (_hasAppliedAloeToday == null) _hasAppliedAloeToday = false;
                return (bool)_hasAppliedAloeToday;
            }
            set
            {
                if (value == true)
                {
                    AddFlag("HasAppliedAloeToday");
                }
                else
                {
                    RemoveFlag("HasAppliedAloeToday");
                }
                _hasAppliedAloeToday = value;
            }
        }
        private bool? _hasAppliedAloeToday = null;

        //Lotion types this mod adds to the game
        public bool IsLotion(Item item)
        {
            if (item is StardewValley.Object &&
                !(item as StardewValley.Object).bigCraftable.Value &&
                !(item is Wallpaper) &&
                !(item is Furniture))
            {
                LotionIDs = new int[] {
                    JA.GetObjectId("SPF60 Sunscreen"),
                    JA.GetObjectId("Aloe Vera Gel")
                };
                if (LotionIDs.Contains(item.ParentSheetIndex))
                {
                    return true;
                }
            }
            return false;
        }

        //Called when a user clicks on their player while holding a lotion bottle
        public void ApplyQuestion(Item item)
        {
            //Check if it can be applied (Aloe Vera Gel can only be applied once a day)
            if (item.ParentSheetIndex == JA.GetObjectId("Aloe Vera Gel") && HasAppliedAloeToday)
            {
                Game1.drawDialogueNoTyping(i18n.Get("Error.AlreadyUsedLotionToday", new { lotionName = item.DisplayName }));
                return;
            }

            //Check if it can be applied (Sunscreen can be applied once every 30 minutes, unless it washed off)
            if (item.ParentSheetIndex == JA.GetObjectId("SPF60 Sunscreen") && ModEntry.Instance.Sunscreen.AppliedSunscreenRecently())
            {
                Game1.drawDialogueNoTyping(i18n.Get("Error.UsedLotionVeryRecently", new { lotionName = item.DisplayName }));
                return;
            }

            string question = i18n.Get("Question.ApplyLotion", new { lotionName = item.DisplayName });
            Response[] yesNoResponses = Game1.currentLocation.createYesNoResponses();
            GameLocation.afterQuestionBehavior afterAnswer = ApplyLotionAnswer;

            Game1.currentLocation.createQuestionDialogue(question, yesNoResponses, afterAnswer);
        }

        public void ApplyLotionAnswer(Farmer who, string whichAnswer)
        {
            if (Config.DebugMode) Monitor.Log($"Player chose answer: {whichAnswer}", LogLevel.Debug);
            if (whichAnswer == "No")
            {
                return; //Don't use any lotion
            }

            if (Game1.player.ActiveObject.ParentSheetIndex == JA.GetObjectId("SPF60 Sunscreen"))
            {
                ApplySunscreen();
            }
            else if (Game1.player.ActiveObject.ParentSheetIndex == JA.GetObjectId("Aloe Vera Gel"))
            {
                ApplyAloeGel();
            }
            //remove one from the stack
            who.reduceActiveItemByOne();
            who.jump(4.0f); //Default jump is 8f
            DelayedAction.playSoundAfterDelay("slimedead", 100);
            //sound effect and/or animation? OOH slimeHit, slimedead, cavedrip, shadowHit, killAnimal, fishSlap, harvest, hitEnemy, dropItemInWater, pullItemFromWater, bob, waterSlosh, slosh, 
            //Candidates: slimeHit, slimedead, fishSlap, bob
        }

        public void ApplySunscreen()
        {
            //apply the sunscreen's effects
            ModEntry.Instance.Sunscreen.ApplySunscreen();
            Game1.addHUDMessage(new HUDMessage(i18n.Get("Apply.Sunscreen", new { hours = Config.SunscreenDuration }), 2)); //Exclamation mark message type

            //TODO: make skin briefly white????
        }

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
                Game1.addHUDMessage(new HUDMessage(i18n.Get("Apply.AloeGel", new { lotionName = gel.DisplayName }), 4)); //stamina_type HUD message
                burn.DisplaySunburnStatus(); //Info: new sunburn level, or healed if healed
            }

            //TODO: make skin briefly green????
        }
    }
}
