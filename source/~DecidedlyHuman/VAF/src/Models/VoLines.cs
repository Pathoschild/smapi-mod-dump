/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace VAF.Models;

public class VoLines : List<VoLine>
{
    private List<VoLine> voLines;
    private string assetName;

    public List<VoLine> Lines
    {
        get => this.voLines;
    }

    public bool ContainsDialogueKey(string key)
    {
        foreach (VoLine line in this)
        {
            if (line.IsForDialogueKey(key))
                return true;
        }

        return false;
    }

    public bool TryGetFilePath(string key, out string filePath)
    {
        filePath = "";

        foreach (VoLine line in this)
        {
            if (line.IsForDialogueKey(key))
            {
                filePath = line.GetFilePath();

                return true;
            }
        }

        return false;
    }

    public VoLines(string assetName)
    {
        this.voLines = new List<VoLine>();
        this.assetName = assetName;
    }

    public string GetAssetName()
    {
        return this.assetName;
    }

    public override string ToString()
    {
        return this.assetName;
    }

    // public void Add(VoLine line)
    // {
    //     this.voLines.Add(line);
    // }
    //
    // public void Remove(VoLine line)
    // {
    //     this.voLines.Remove(line);
    // }
}
