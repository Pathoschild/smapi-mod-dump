/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

namespace ClimatesOfFerngillRebuild.Integrations
{
    public interface ILunarDisturbancesAPI
    {
        string GetCurrentMoonPhase();
        bool IsSolarEclipse();
        int GetMoonRise();
        int GetMoonSet();
        bool IsMoonUp(int time);
    }
}
