/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

namespace Igorious.StardewValley.DynamicAPI.Interfaces
{
    public interface IInformation
    {
        int ID { get; }

        /// <summary>
        /// Get serialized string.
        /// </summary>
        string ToString();
    }

    public interface ICropInformation : IInformation {}

    public interface IItemInformation : IInformation {}

    public interface ITreeInformation : IInformation {}
}