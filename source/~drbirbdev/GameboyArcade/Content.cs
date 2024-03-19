/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Text.Json.Serialization;
using BirbCore.Attributes;
using StardewModdingAPI;
// ReSharper disable InconsistentNaming

namespace GameboyArcade;

[SContent("content.json", false, true)]
public class Content
{
    public string Name;
    public string FilePath;
    public bool EnableEvents = false;
    public string SaveStyle = "LOCAL";
    public string LinkStyle = "NONE";
    public string SoundStyle = "NONE";

    [JsonIgnore]
    [SContent.UniqueId]
    public string UniqueID;
    [JsonIgnore]
    [SContent.ModId]
    public string ModID;
    [JsonIgnore]
    [SContent.ContentId]
    public string GameID;
    [JsonIgnore]
    [SContent.ContentPack]
    public IContentPack ContentPack;
}
