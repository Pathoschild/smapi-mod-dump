/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using StardewModdingAPI;

namespace QuestFramework.Framework.ContentPacks
{
    internal interface ITranslatable<T>
    {
        T Translate(ITranslationHelper translation);
    }
}
