/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Let-Me-Rest
**
*************************************************/

using System.Linq;
using StardewModdingAPI;
using StardewValley;
using LetMeRest.Framework.Lists;

namespace LetMeRest.Framework.Common
{
    public class Check
    {
        public static int resetMovingTimer = 2;
        public static bool playingSound;
        private static int movingTimer;

        public static bool IsPaused()
        {
            if (!Context.IsWorldReady) return false;
            if (Context.IsMultiplayer)
            {
                Farmer[] farmers = Game1.getAllFarmers().ToArray();
                if (farmers.Length <= 1)
                {
                    if (!Context.IsPlayerFree) return true;
                    if (!Game1.game1.IsActiveNoOverlay) return true;
                }
            }
            else
            {
                if (!Context.IsPlayerFree) return true;
                if (!Game1.game1.IsActiveNoOverlay) return true;
            }
            return false;
        }

        public static void IsRecoveringStamina()
        {
            if (Game1.player.IsSitting())
            {
                if (Context.IsMultiplayer)
                {
                    if (ModEntry.data.SittingVerification)
                    {
                        float secretMultiplier = Secrets.CheckForSecrets();
                        if (ModEntry.data.EnableSecrets) Status.IncreaseStamina(1f, secretMultiplier);
                        else Status.IncreaseStamina(1f, 1);
                    }
                }
                else
                {
                    if (ModEntry.config.SittingVerification)
                    {
                        float secretMultiplier = Secrets.CheckForSecrets();
                        if (ModEntry.config.EnableSecrets) Status.IncreaseStamina(1f, secretMultiplier);
                        else Status.IncreaseStamina(1f, 1);
                    }
                }
            }

            if (Game1.player.isRidingHorse())
            {
                if (Context.IsMultiplayer)
                {
                    if (ModEntry.data.RidingVerification)
                    {
                        Status.IncreaseStamina(0.25f, 1);
                    }
                }
                else
                {
                    if (ModEntry.config.RidingVerification)
                    {
                        Status.IncreaseStamina(0.25f, 1);
                    }
                }
            }

            if (!Game1.player.isMoving() &&
                !Game1.player.IsSitting() &&
                !Game1.player.isRidingHorse() &&
                !Game1.player.UsingTool)
            {
                if (Context.IsMultiplayer)
                {
                    if (ModEntry.data.StandingVerification)
                    {
                        if (movingTimer > 0) movingTimer--;
                        else Status.IncreaseStamina(0.25f, 1);
                    }
                }
                else
                {
                    if (ModEntry.config.StandingVerification)
                    {
                        if (movingTimer > 0) movingTimer--;
                        else Status.IncreaseStamina(0.25f, 1);
                    }
                }
            }
            else
            {
                IsStaying();
            }

            if (Game1.player.isMoving() || Game1.player.usingTool)
            {
                Status.canUpdateQuantity = true;
            }
        }

        public static void IsUsingTool()
        {
            if (!Context.IsWorldReady) return;
            if (!Game1.player.UsingTool) return;

            if (Context.IsMultiplayer)
            {
                if (!ModEntry.data.StandingVerification) return;
                movingTimer = resetMovingTimer * 60;
                playingSound = false;
            }
            else
            {
                if (!ModEntry.config.StandingVerification) return;
                movingTimer = resetMovingTimer * 60;
                playingSound = false;
            }
        }

        public static void IsStaying()
        {
            if (Context.IsMultiplayer)
            {
                if (!ModEntry.data.StandingVerification) return;
                movingTimer = resetMovingTimer * 60;
                playingSound = false;
            }
            else
            {
                if (!ModEntry.config.StandingVerification) return;
                movingTimer = resetMovingTimer * 60;
                playingSound = false;
            }
        }

        public static bool AreInEvent()
        {
            if (Game1.CurrentEvent != null) return true;
            else return false;
        }
    }
}
