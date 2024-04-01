/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;
using System.Collections.Generic;
using System.Linq;

namespace HorseOverhaul
{
    internal class ButtonHandling
    {
        private static readonly List<SButton> mouseButtons = new() { SButton.MouseLeft, SButton.MouseRight, SButton.MouseMiddle, SButton.MouseX1, SButton.MouseX2 };

        internal static void OnButtonPressed(HorseOverhaul mod, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsPlayerFree || mod.Config.DisableMainSaddleBagAndFeedKey)
            {
                return;
            }

            if (e.Button.IsUseToolButton())
            {
                bool ignoreMousePosition = !mouseButtons.Contains(e.Button);
                Point cursorPosition = Game1.getMousePosition();

                bool interacted = Feeding.CheckHorseInteraction(mod, Game1.player, cursorPosition.X + Game1.viewport.X, cursorPosition.Y + Game1.viewport.Y, ignoreMousePosition);

                if (!interacted)
                {
                    Feeding.CheckPetInteraction(mod, Game1.player, cursorPosition.X + Game1.viewport.X, cursorPosition.Y + Game1.viewport.Y, ignoreMousePosition);
                }
            }
        }

        internal static void OnButtonsChanged(HorseOverhaul mod, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsPlayerFree)
            {
                return;
            }

            Point cursorPosition = Game1.getMousePosition();
            var mouseX = cursorPosition.X + Game1.viewport.X;
            var mouseY = cursorPosition.Y + Game1.viewport.Y;

            // this is done in buttonsChanged instead of buttonPressed as recommended
            // in the documentation: https://stardewcommunitywiki.com/Modding:Modder_Guide/APIs/Input#KeybindList
            if (mod.Config.HorseMenuKey.JustPressed() || mod.Config.PetMenuKey.JustPressed())
            {
                var keyBindToCheck = mod.Config.HorseMenuKey.JustPressed() ? mod.Config.HorseMenuKey : mod.Config.PetMenuKey;

                bool isControllerInput = false;

                foreach (Keybind keybind in keyBindToCheck.Keybinds)
                {
                    if (keybind.GetState() != SButtonState.Pressed)
                    {
                        continue;
                    }

                    foreach (var button in keybind.Buttons)
                    {
                        if (button.TryGetController(out _))
                        {
                            isControllerInput = true;
                            break;
                        }
                    }
                }

                if (keyBindToCheck == mod.Config.HorseMenuKey)
                {
                    OpenHorseMenu(mod, Game1.player, mouseX, mouseY, isControllerInput);
                }
                else
                {
                    OpenPetMenu(mod, Game1.player, mouseX, mouseY, isControllerInput);
                }

                return;
            }

            if (mod.Config.AlternateSaddleBagAndFeedKey.JustPressed())
            {
                bool interacted = Feeding.CheckHorseInteraction(mod, Game1.player, 0, 0, true);

                if (!interacted)
                {
                    Feeding.CheckPetInteraction(mod, Game1.player, 0, 0, true);
                }
            }
        }

        private static void OpenHorseMenu(HorseOverhaul mod, Farmer who, int mouseX, int mouseY, bool ignoreMousePosition)
        {
            HorseWrapper horse;

            if (who.isRidingHorse() && !who.mount.IsTractor())
            {
                horse = mod.Horses.Where(h => h?.Horse?.HorseId == who.mount.HorseId).FirstOrDefault();

                if (horse != null)
                {
                    Game1.activeClickableMenu = new HorseMenu(mod, horse);
                    return;
                }
            }

            if (who.currentLocation != null)
            {
                foreach (NPC npc in who.currentLocation.characters)
                {
                    if (npc is not Horse nearbyHorse)
                    {
                        continue;
                    }

                    if (ignoreMousePosition && !Utility.withinRadiusOfPlayer((int)nearbyHorse.Position.X, (int)nearbyHorse.Position.Y, 1, who))
                    {
                        continue;
                    }

                    if (!nearbyHorse.MouseOrPlayerIsInRange(who, mouseX, mouseY, ignoreMousePosition))
                    {
                        continue;
                    }

                    horse = mod.Horses.Where(h => h?.Horse?.HorseId == nearbyHorse.HorseId).FirstOrDefault();

                    if (horse != null)
                    {
                        Game1.activeClickableMenu = new HorseMenu(mod, horse);
                        return;
                    }
                }
            }

            // get the exact first horse you got
            horse = mod.Horses.Where(h => h?.Horse?.getOwner() == who && h?.Horse?.getName() == who.horseName.Value).FirstOrDefault();

            if (horse != null)
            {
                Game1.activeClickableMenu = new HorseMenu(mod, horse);
            }
        }

        private static void OpenPetMenu(HorseOverhaul mod, Farmer who, int mouseX, int mouseY, bool ignoreMousePosition)
        {
            if (who.currentLocation != null)
            {
                foreach (NPC npc in who.currentLocation.characters)
                {
                    if (npc is not Pet pet)
                    {
                        continue;
                    }

                    if (ignoreMousePosition && !Utility.withinRadiusOfPlayer((int)pet.Position.X, (int)pet.Position.Y, 1, who))
                    {
                        continue;
                    }

                    if (!pet.MouseOrPlayerIsInRange(who, mouseX, mouseY, ignoreMousePosition))
                    {
                        continue;
                    }

                    Game1.activeClickableMenu = new PetMenu(mod, pet);
                    return;
                }
            }

            if (who.hasPet())
            {
                Pet pet = who.getPet();

                if (pet != null)
                {
                    Game1.activeClickableMenu = new PetMenu(mod, pet);
                }
            }
        }
    }
}