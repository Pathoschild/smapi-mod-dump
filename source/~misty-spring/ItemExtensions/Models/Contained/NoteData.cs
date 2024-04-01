/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

namespace ItemExtensions.Models.Contained;

public class NoteData
{
    //mutually exclusive w/ everything else
    public string MailId { get; } = null;
    public string Image { get; } = null;
    public string LetterTexture { get; } = "0";
    public string Message { get; } = null;
    public string ImagePosition { get; } = "down";
}