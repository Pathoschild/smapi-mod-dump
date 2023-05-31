/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;

namespace VAF.Models;

public class VoLine
{
    private IAssetName dialogueAssetName;
    private string key;
    private string filePath;
    private IManifest owningMod;
    private CueDefinition cue;

    public VoLine(IAssetName dialogueAssetName, string key, string filePath, IManifest owningMod)
    {
        this.dialogueAssetName = dialogueAssetName;
        this.key = key;
        this.filePath = filePath;
        this.owningMod = owningMod;
    }
}
