/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Survivalistic
**
*************************************************/

using StardewModdingAPI;

namespace Survivalistic.Framework.Common.Affection
{
    public static class Benefits
    {
        public static bool VerifyBenefits()
        {
            if (!Context.IsWorldReady)
            {
                return false;
            }

            else if (ModEntry.data.actual_hunger >= 80 || ModEntry.data.actual_thirst >= 80)
            {
                if (ModEntry.data.actual_hunger >= 80)
                    CallSettingBuff(Buffs.FullnessBuffName);

                else
                    CallSettingBuff(Buffs.HydratedBuffName);

                return true;
            }

            else
            {
                return false;
            }
        }

        private static void CallSettingBuff(string buffName) =>
                            Buffs.SetBuff(buffName);
    }
}
