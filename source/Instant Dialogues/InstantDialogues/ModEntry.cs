/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/idermailer/InstantDialogues
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace InstantDialogues
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        private ModConfig? config;
        public bool modOn;
        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<ModConfig>();
            modOn = config.EnableOnLaunchWhenUseToggleKey;

            if (config.EntireModEnabled)
            {
                helper.Events.Display.MenuChanged += Display_MenuChanged;
                if (config.UseToggleKey)
                    helper.Events.Input.ButtonsChanged += Input_ButtonsChanged;
            }
        }

        private void Display_MenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is DialogueBox NewMenu)
            {
                if (modOn) {
                    NewMenu.showTyping = false;
                    if (config != null)
                        NewMenu.safetyTimer = config.SkipPreventTimeWhileModOn;
                }
            }
        }

        private void Input_ButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (config != null && config.ToggleKey.JustPressed())
            {
                modOn = !modOn;
                string message;
                if (modOn)
                {
                    message = Helper.Translation.Get("modOn");
                }
                else
                {
                    message = Helper.Translation.Get("modOff");
                }

                HUDMessage notify = new(message)
                {
                    noIcon = true,
                    timeLeft = 3000f
                };
                Game1.addHUDMessage(notify);
            }
        }
    }
}