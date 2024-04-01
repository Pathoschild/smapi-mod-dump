/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace ProducerFrameworkMod.ContentPack;

public abstract class ProducerData
{
    public string ModUniqueID;
    public string ProducerName;
    public string ProducerQualifiedItemId;
    public List<string> AdditionalProducerNames = new();
    public List<string> AdditionalProducerQualifiedItemId = new();
    public List<string> OverrideMod = new();
}