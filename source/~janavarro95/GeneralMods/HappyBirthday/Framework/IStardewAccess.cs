/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;


namespace Omegasis.HappyBirthday
{
    /// <summary>Stardew-Access screenreader API interface for accessibility.</summary>
    public interface IStardewAccessApi
    {
        public void SayWithMenuChecker(String text, Boolean interrupt);
    }
}
