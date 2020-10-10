/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

namespace CopyInviteCode.ClipboardManagers
{
    /// <summary>Sets clipboard contents on Mac.</summary>
    internal class MacClipboardManager : UnixClipboardManager
    {
        protected override string FileName => "pbcopy";
        protected override string SetArguments => "-pboard general -Prefer txt";
    }
}
