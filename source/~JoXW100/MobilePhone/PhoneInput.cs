/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;

namespace MobilePhone
{
    public class PhoneInput 
    {
        private static IMonitor Monitor;
        private static IModHelper Helper;
        private static ModConfig Config;

        // call this method from your Entry class
        public static void Initialize(IModHelper helper, IMonitor monitor, ModConfig config)
        {
            Monitor = monitor;
            Helper = helper;
            Config = config;
        }

        public static void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.activeClickableMenu == null && ModEntry.phoneOpen && e.Button == SButton.Escape)
            {
                PhoneUtils.TogglePhone();
                Helper.Input.Suppress(SButton.Escape);
                return;
            }

            if (e.Button == Config.OpenPhoneKey && Config.EnableOpenPhoneKey)
            {
                PhoneUtils.TogglePhone();
                return;
            }
            if(e.Button == Config.RotatePhoneKey && Config.EnableRotatePhoneKey)
            {
                PhoneUtils.RotatePhone();
                return;
            }

            float ratio = Game1.options.zoomLevel != 1f ? 1f : 1f / Game1.options.uiScale;
            Point mousePos = Game1.getMousePosition();

            if (!ModEntry.phoneOpen)
            {
                if(Config.ToggleIncomingCallsKey != SButton.None && Game1.activeClickableMenu is null &&  e.Button == Config.ToggleIncomingCallsKey)
                {
                    Config.EnableIncomingCalls = Config.EnableIncomingCalls;
                    Monitor.Log($"Incoming Calls Enabled: {Config.EnableIncomingCalls}");
                    Helper.WriteConfig(Config);
                }
                else if (e.Button == SButton.MouseLeft && Game1.displayHUD && Config.ShowPhoneIcon && PhoneUtils.ScaleRect(ModEntry.phoneIconPosition.X, ModEntry.phoneIconPosition.Y, Config.PhoneIconWidth, Config.PhoneIconHeight, ratio).Contains(mousePos.ToVector2() * ratio))
                {
                    Helper.Input.Suppress(SButton.MouseLeft);
                    ModEntry.clickingPhoneIcon = true;
                    ModEntry.draggingPhoneIcon = false;
                    ModEntry.lastMousePosition = mousePos;
                }
                return;
            }

            if (e.Button == SButton.MouseLeft)
            {
                if (Game1.activeClickableMenu == null && !ModEntry.inCall && !ModEntry.appRunning && !PhoneUtils.ScaleRect(ModEntry.phoneRect, ratio).Contains(mousePos.ToVector2() * ratio))
                {
                    Monitor.Log($"pressing mouse outside of opened phone");
                    Helper.Input.Suppress(SButton.MouseLeft);
                    PhoneUtils.TogglePhone();
                    return;
                }

                if (PhoneUtils.ScaleRect(ModEntry.phoneRect, ratio).Contains(mousePos) && !PhoneUtils.ScaleRect(ModEntry.screenRect, ratio).Contains(mousePos.ToVector2() * ratio))
                {
                    Monitor.Log($"pressing mouse key on phone border");
                    Helper.Input.Suppress(SButton.MouseLeft);
                    ModEntry.clicking = true;
                    ModEntry.draggingPhone = true;
                    ModEntry.lastMousePosition = mousePos;
                    return;
                }


                if(ModEntry.callingNPC != null && PhoneUtils.ScaleRect(ModEntry.screenRect, ratio).Contains(mousePos.ToVector2() * ratio))
                {
                    Monitor.Log($"pressing mouse key in phone while calling");
                    Helper.Input.Suppress(SButton.MouseLeft);

                    ModEntry.clicking = true;
                    return;
                }

                if (!ModEntry.appRunning && PhoneUtils.ScaleRect(ModEntry.screenRect, ratio).Contains(mousePos.ToVector2() * ratio))
                {
                    Monitor.Log($"pressing mouse key in phone");
                    Helper.Input.Suppress(SButton.MouseLeft);

                    ModEntry.clicking = true;
                    ModEntry.lastMousePosition = mousePos;
                    for (int i = 0; i < ModEntry.appOrder.Count; i++)
                    {
                        Vector2 pos = PhoneUtils.GetAppPos(i);
                        Rectangle r = PhoneUtils.ScaleRect(pos.X, pos.Y, Config.IconWidth, Config.IconHeight, ratio);
                        if (r.Contains(mousePos.ToVector2() * ratio))
                        {
                            ModEntry.clickingApp = i;
                            ModEntry.clickingTicks = 0;
                            break;
                        }
                    }
                }
            }

        }

        public static void PressKey(MobileApp app)
        {
            if(app.closePhone)
            {
                PhoneUtils.TogglePhone(false);
            }

            if (!Enum.TryParse(app.keyPress, out SButton keyPress))
            {
                Monitor.Log($"Error on app invoke: {app.keyPress} isn't a valid key", LogLevel.Error);
                return;
            }

            // get SMAPI's input handler
            var input = Game1.input;

            // get OverrideButton method
            var method = input.GetType().GetMethod("OverrideButton");
            if (method == null)
            {
                Monitor.Log("Can't find 'OverrideButton' method on SMAPI's input class.", LogLevel.Error);
                return;
            }

            // call method
            // The arguments are the button to override, and whether to mark the button pressed (true) or raised (false)
            method.Invoke(input, new object[] { keyPress, true });
            return;
        }

    }
}
