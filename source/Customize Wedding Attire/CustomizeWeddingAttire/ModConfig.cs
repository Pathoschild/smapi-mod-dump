/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elizabethcd/CustomizeWeddingAttire
**
*************************************************/

using System;
namespace CustomizeWeddingAttire
{
    public class ModConfig
    {
        // Add an config for what kind of attire to use during the wedding scene
        // Options are: None, Tux, Dress, Default
        public string WeddingAttire { get; set; }

        public ModConfig()
        {
            // By default, make no changes
            this.WeddingAttire = ModEntry.noneOption;
        }
    }
}