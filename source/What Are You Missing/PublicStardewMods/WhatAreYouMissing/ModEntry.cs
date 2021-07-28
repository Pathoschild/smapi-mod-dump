/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LukeSeewald/PublicStardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace WhatAreYouMissing
{
    public class ModEntry : Mod
    {
        private SButton ButtonToBringUpInterface;
        private IReflectionHelper Reflection;
        public static ModConfig modConfig;
        public static Logger Logger;
        public static ITranslationHelper Translator;
        public static IModHelper HelperInstance;

        //These are initialized only once (per button press to open menu) as they are performance heavy
        public static MissingItems MissingItems;
        public static RecipeIngredients RecipesIngredients;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            //ModEntry is called once before SDV loads.In 3.0 its called even earlier than 2.x - things like Game1.objectInformation 
            //aren't ready yet. If you need to run stuff when a save is ready use the save loaded event
            Translator = helper.Translation;
            Reflection = Helper.Reflection;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            HelperInstance = Helper;
            modConfig = Helper.ReadConfig<ModConfig>();
            ButtonToBringUpInterface = modConfig.button;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (e.Button.Equals(ButtonToBringUpInterface))
            {
                Logger = new Logger(this);

                //Initialize the performance heavy operations
                MissingItems = new MissingItems(Reflection);
                RecipesIngredients = new RecipeIngredients();

                Menu whatIsMissingMenu = new Menu();
                Game1.activeClickableMenu = whatIsMissingMenu;
            }
        }

    }
}
