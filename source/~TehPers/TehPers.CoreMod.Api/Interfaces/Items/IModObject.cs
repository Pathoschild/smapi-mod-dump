using System.Diagnostics;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;

namespace TehPers.CoreMod.Api.Items {
    public interface IModObject : IModItem {
        /// <summary>Gets the raw information that should be added to "Data/ObjectInformation".</summary>
        /// <returns>The raw information string.</returns>
        string GetRawObjectInformation();
    }
}