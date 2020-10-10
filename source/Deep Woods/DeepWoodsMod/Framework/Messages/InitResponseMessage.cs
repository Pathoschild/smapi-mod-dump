/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/

namespace DeepWoodsMod.Framework.Messages
{
    internal class InitResponseMessage
    {
        public DeepWoodsSettings Settings { get; set; }
        public DeepWoodsStateData State { get; set; }
        public string[] LevelNames { get; set; }
    }
}
