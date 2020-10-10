/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using Newtonsoft.Json;

namespace Igorious.StardewValley.NewMachinesMod.Data
{
    public class MachineDraw
    {
        /// <summary>
        /// Sprite delta to drawing empty machine.
        /// </summary>
        [JsonProperty]
        public int Empty { get; set; }

        /// <summary>
        /// Sprite delta to drawing working machine.
        /// </summary>
        [JsonProperty]
        public int Working { get; set; }

        [JsonProperty]
        public string WorkingColor { get; set; }

        /// <summary>
        /// Sprite delta to drawing ready machine.
        /// </summary>
        [JsonProperty]
        public int Ready { get; set; }

        [JsonProperty]
        public string ReadyColor { get; set; }
    }
}