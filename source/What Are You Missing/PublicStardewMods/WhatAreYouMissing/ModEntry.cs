using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace WhatAreYouMissing
{
    public class ModEntry : Mod
    {
        private SButton buttonToBringUpInterface;
        public static ModConfig modConfig;
        public static MissingItems MissingItems;
        public static RecipeIngredients RecipesIngredients;
        public static Logger Logger;
        public static ITranslationHelper Translator;
        public static IModHelper HelperInstance;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            //ModEntry is called once before SDV loads.In 3.0 its called even earlier than 2.x - things like Game1.objectInformation 
            //aren't ready yet. If you need to run stuff when a save is ready use the save loaded event
            Translator = helper.Translation;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            HelperInstance = Helper;
            modConfig = Helper.ReadConfig<ModConfig>();
            buttonToBringUpInterface = modConfig.button;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (e.Button.Equals(buttonToBringUpInterface))
            {
                Logger = new Logger(this);

                MissingItems = new MissingItems();
                RecipesIngredients = new RecipeIngredients();

                Menu whatIsMissingMenu = new Menu();
                Game1.activeClickableMenu = whatIsMissingMenu;
            }
        }

    }
}
