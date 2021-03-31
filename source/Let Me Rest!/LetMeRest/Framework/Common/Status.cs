/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Let-Me-Rest
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using LetMeRest.Framework.Lists;

namespace LetMeRest.Framework.Common
{
    public class Status
    {
        private static int radius = 6;

        public static void IncreaseStamina(float value, float secretMultiplier)
        {
            if (Game1.player.Stamina < Game1.player.MaxStamina)
            {
                value /= 60;

                float decorationMultiplier = AmbientInformation.Infos(radius, DataBase.ItemDataBase)[0];
                float waterMultiplier = AmbientInformation.Infos(radius, DataBase.ItemDataBase)[1];
                float paisageMultiplier = AmbientInformation.Infos(radius, DataBase.ItemDataBase)[2];

                if (Context.IsMultiplayer)
                {
                    Game1.player.Stamina += (value * ModEntry.data.Multiplier) *
                        ((decorationMultiplier * 1.25f) * ModEntry.data.Multiplier) *
                        (waterMultiplier * ModEntry.data.Multiplier) *
                        (paisageMultiplier * ModEntry.data.Multiplier) *
                        (secretMultiplier * ModEntry.data.Multiplier);
                }
                else
                {
                    Game1.player.Stamina += (value * ModEntry.config.Multiplier) *
                        ((decorationMultiplier * 1.25f) * ModEntry.config.Multiplier) *
                        (waterMultiplier * ModEntry.config.Multiplier) *
                        (paisageMultiplier * ModEntry.config.Multiplier) *
                        (secretMultiplier * ModEntry.config.Multiplier);
                }

                Buffs.SetBuff("Restoring");
            }
        }
    }
}
