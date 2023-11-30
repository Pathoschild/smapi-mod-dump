/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;

namespace RealtimeFramework;

public interface IRealtimeAPI
{
    /// <summary>
    /// Returns whether the given holiday exists.
    /// </summary>
    /// <param name="holiday"></param>
    /// <returns></returns>
    bool IsHoliday(string holiday);

    /// <summary>
    /// Returns whether the given holiday is upcoming.
    /// </summary>
    /// <param name="holiday"></param>
    /// <returns></returns>
    bool IsComingHoliday(string holiday);

    /// <summary>
    /// Returns whether the given holiday is today.
    /// </summary>
    /// <param name="holiday"></param>
    /// <returns></returns>
    bool IsCurrentHoliday(string holiday);

    /// <summary>
    /// Returns whether the given holiday has just passed.
    /// </summary>
    /// <param name="holiday"></param>
    /// <returns></returns>
    bool IsPassingHoliday(string holiday);

    /// <summary>
    /// Returns an enumeration of all holidays.
    /// </summary>
    /// <returns></returns>
    IEnumerable<string> GetAllHolidays();

    /// <summary>
    /// Returns an enumeration of upcoming holidays.
    /// </summary>
    /// <returns></returns>
    IEnumerable<string> GetComingHolidays();

    /// <summary>
    /// Returns an enumeration of holidays happening today.
    /// </summary>
    /// <returns></returns>
    IEnumerable<string> GetCurrentHolidays();

    /// <summary>
    /// Returns an enumeration of holidays that have just passed.
    /// </summary>
    /// <returns></returns>
    IEnumerable<string> GetPassingHolidays();

    /// <summary>
    /// Returns the localization of a holiday name.
    /// </summary>
    /// <param name="holiday"></param>
    /// <returns></returns>
    string GetLocalName(string holiday);
}
