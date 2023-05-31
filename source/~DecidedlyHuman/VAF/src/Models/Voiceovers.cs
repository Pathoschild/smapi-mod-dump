/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Permissions;

namespace VAF.Models;

public class Voiceovers : Dictionary<string, VoLines>
{
    public bool ContainsVoiceover(string assetName, string key)
    {
        if (this.ContainsKey(assetName))
        {
            if (this[assetName].ContainsDialogueKey(key))
            {
                return true;
            }
        }

        return false;
    }

    public bool TryGetFileForDialogue(string assetName, string key, out string filePath)
    {
        filePath = "";

        try
        {
            if (this[assetName].ContainsDialogueKey(key))
            {
                if (this[assetName].TryGetFilePath(key, out filePath))
                {
                    return true;
                }
            }
        }
        catch (Exception e)
        {
            return false;
        }

        return false;
    }
}
