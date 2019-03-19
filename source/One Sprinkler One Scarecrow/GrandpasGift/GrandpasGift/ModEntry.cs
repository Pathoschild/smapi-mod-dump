using System;
using System.Linq;
using StardewValley;
using StardewModdingAPI;
using GrandpasGift.Framework;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Menus;
using StardewValley.Tools;
using StardewValley.Objects;

namespace GrandpasGift
{
    public class ModEntry : Mod
    {
        private ModConfig _config;
        private ITranslationHelper _i18n;
        private SDate sd;
        public override void Entry(IModHelper helper)
        {
            //Initialize the config
            _config = helper.ReadConfig<ModConfig>();

            //Initialize the translations
            _i18n = helper.Translation;

             
            //Events
            helper.Events.GameLoop.DayStarted += this.DayStarted;
            //helper.Events.Display.MenuChanged += this.MenuChanged;
        }


        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            sd = SDate.Now();
            if (!Context.IsWorldReady || sd.DaysSinceStart != 1 || !_config.ModActive)
                return;
            //Everything passed. Now we can proceed
            Monitor.Log("Running the DayStarted method.", LogLevel.Warn);
            //ShowLetter();
            //Set up initial dialogue box
            DialogueBox dBox = new DialogueBox(_i18n.Get("grandpas_initial"));// {exitFunction = ShowLetter};
            Helper.Events.Display.MenuChanged += this.MenuChanged;
            Game1.activeClickableMenu = dBox;
            Game1.activeClickableMenu.exitFunction = ShowLetter;

        }

        private void MenuChanged(object sender, MenuChangedEventArgs e)
        {
            
            if ((e.OldMenu is LetterViewerMenu || e.OldMenu is DialogueBox) && e.OldMenu != null)
            {
                e.OldMenu.exitFunction.Invoke();
                Helper.Events.Display.MenuChanged -= this.MenuChanged;
                Monitor.Log("NewMenu ran.", LogLevel.Alert);
            }
        }
        private void ShowLetter()
        {
            Monitor.Log("Running the ShowLetter method.", LogLevel.Warn);
            LetterViewerMenu grampsLetter =
                new LetterViewerMenu(_i18n.Get("grandpas_letter", new {name = Game1.player.Name}));//{exitFunction = GiveItems};
            Game1.activeClickableMenu = grampsLetter;
            Game1.activeClickableMenu.exitFunction = GiveItems;
        }
        private void GiveItems()
        {
            Monitor.Log("Running the GiveItems method.", LogLevel.Warn);
            int FarmType = Game1.whichFarm;

            //Just for initial release Will add more later
            MeleeWeapon grampsWeapon = new MeleeWeapon(20);
            grampsWeapon.minDamage.Value = 5000;
            grampsWeapon.maxDamage.Value = 10000;
            grampsWeapon.critChance.Value = 1f;
            grampsWeapon.critMultiplier.Value = 2f;
            //Give item to player
            Game1.player.addItemByMenuIfNecessary(grampsWeapon);
            Game1.player.addItemByMenuIfNecessary(new Chest(true));
            Monitor.Log("Should have given items........", LogLevel.Warn);
        }
        //Test
        /*
        private void UpdateBuff()
        {
            Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == this.BuffUniqueID);
            if (buff == null)
            {
                buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, this.Config.MagneticRadius, this.Config.TractorSpeed, 0, 0, 1, "Tractor Power", this.Translation.Get("buff.name")) { which = this.BuffUniqueID };
                
                Game1.buffsDisplay.addOtherBuff(buff);
            }
            buff.millisecondsDuration = 100;
        }*/
    }
}
