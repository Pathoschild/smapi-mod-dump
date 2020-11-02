/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jahangmar/StardewValleyMods
**
*************************************************/

// Copyright (c) 2020 Jahangmar
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
using StardewModdingAPI;
using StardewValley.Menus;
using StardewValley;

using AccessibilityForBlind.Menus;
using Microsoft.Xna.Framework;


using Harmony;

namespace AccessibilityForBlind
{
    public class ModEntry : Mod
    {
        private static ModEntry instance;
        private static AccessibilityConfiguration config;

        private AccessMenu ActiveMenu;
        private StardewValley.Menus.IClickableMenu oldMenu;

        public override void Entry(IModHelper helper)
        {
            instance = this;
            config = helper.ReadConfig<AccessibilityConfiguration>();

            TextToSpeech.Init();

            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            Helper.Events.Display.WindowResized += Display_WindowResized;
            Helper.Events.Display.MenuChanged += Display_MenuChanged;
            GameplaySounds.Init();

            ApplyHarmonyPatches();
        }

        void Display_WindowResized(object sender, StardewModdingAPI.Events.WindowResizedEventArgs e)
        {
            if (ActiveMenu != null)
                ActiveMenu = SelectMenu(ActiveMenu.GetStardewMenu());
        }

        public AccessMenu SelectMenu(IClickableMenu menu)
        {
            if (menu is TitleMenu titleMenu)
            {
                ActiveMenu = new AccessTitleMenu(titleMenu);
            }
            else if (menu is CharacterCustomization characterCustomization)
            {
                ActiveMenu = new AccessCharacterCreationMenu(characterCustomization);
            }
            else if (menu is LanguageSelectionMenu languageSelectionMenu && ActiveMenu is AccessTitleMenu)
            {
                ActiveMenu = new AccessLanguageMenu(languageSelectionMenu, ActiveMenu.GetStardewMenu() as TitleMenu);
            }
            else if (menu is LoadGameMenu loadGameMenu) //also for co-op menu
            {
                ActiveMenu = new AccessLoadMenu(loadGameMenu);
            }
            else if (menu is DialogueBox dialogueBox)
            {
                ActiveMenu = new AccessDialogBox(dialogueBox);
            }
            else
            {
                TextToSpeech.Speak("unknown menu");
                ActiveMenu = null;
            }
            return ActiveMenu;
        }

        void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (e.NewMenu != null)
            {
                SelectMenu(e.NewMenu);
                oldMenu = e.NewMenu;
            }
        }

        void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {

            if (Game1.activeClickableMenu == null)
            {
                oldMenu = null;
                ActiveMenu = null;
            }

            if (Game1.activeClickableMenu != oldMenu)
            {
                oldMenu = Game1.activeClickableMenu;
                SelectMenu(Game1.activeClickableMenu);
            }

            if (ActiveMenu != null)
            {
                ActiveMenu.ButtonPressed(e.Button);
            }

            if (ActiveMenu != null && Game1.activeClickableMenu != null && !e.Button.IsActionButton() && !e.Button.IsUseToolButton() && !(Game1.activeClickableMenu is TitleMenu && e.Button == SButton.Escape))
            {
                Helper.Input.Suppress(e.Button);
                Log("suppressed button");
            }
            else
                Log("not suppressed button");
        }

        private void ApplyHarmonyPatches()
        {
            HarmonyInstance harmonyInstance = HarmonyInstance.Create(instance.ModManifest.UniqueID);
            //HarmonyPatches.Farmer_AddItemToInventory.Patch(harmonyInstance);
            HarmonyPatches.Game1_showMessages.Patch(harmonyInstance);
        }

        public static void Log(string text, LogLevel level=LogLevel.Trace)
        {
            instance.Monitor.Log(text, level);
        }

        public static AccessibilityConfiguration GetConfig() => config;

        public static IModHelper GetHelper() => instance.Helper;

        public static ModEntry GetInstance() => instance;

    }
}
