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
    /// <summary>Sets clipboard contents on Linux.</summary>
    internal class LinuxClipboardManager : UnixClipboardManager
    {
        protected override string FileName => "xclip";
        protected override string SetArguments => "-selection clipboard";
    }
}
