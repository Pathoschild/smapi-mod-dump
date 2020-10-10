/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

namespace Entoarox.Framework
{
    public interface IConditionHelper
    {
        bool ValidateConditions(string conditions, char separator = ',');
        bool ValidateConditions(string[] conditions);
        bool ValidateCondition(string condition);
    }
}
