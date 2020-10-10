/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

namespace Igorious.StardewValley.DynamicApi2.Utils
{
    public interface IConstructor<out TClass>
    {
        TClass Invoke();
    }

    public interface IConstructor<in TArg, out TClass>
    {
        TClass Invoke(TArg arg);
    }

    public interface IConstructor<in TArg1, in TArg2, out TClass>
    {
        TClass Invoke(TArg1 arg1, TArg2 arg2);
    }
}