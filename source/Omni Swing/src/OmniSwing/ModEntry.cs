/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AlejandroAkbal/Stardew-Valley-OmniSwing-Mod
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace OmniSwing
{
    public class ModEntry : Mod
    {
        public static Mod Instance;

        public static new IModHelper Helper;

        public static ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            // Provide a global reference to this mod's SMAPI utilities
            Instance = this;

            Helper = helper;

            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
        }

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsPlayerFree)
            {
                ModLogger.Trace("Player is not free to act!");
                return;
            }

            foreach (SButton button in e.Held)
            {
                if (!button.IsUseToolButton())
                    return;

                //ModLogger.Debug($"OnButtonsChanged called with '{button}'.");

                OmniSwing.AutoSwing();
            }
        }
    }
}