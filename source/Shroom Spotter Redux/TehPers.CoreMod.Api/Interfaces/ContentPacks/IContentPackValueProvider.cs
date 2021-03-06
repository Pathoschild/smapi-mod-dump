/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using TehPers.CoreMod.Api.ContentPacks.Tokens;

namespace TehPers.CoreMod.Api.ContentPacks {
    public interface IContentPackValueProvider<out T> : IContextSpecific {
        T GetValue(ITokenHelper helper);
    }
}