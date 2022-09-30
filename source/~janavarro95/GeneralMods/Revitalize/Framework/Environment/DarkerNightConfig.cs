/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

namespace Omegasis.Revitalize.Framework.Environment
{
    /// <summary>
    /// Deals with configurations for darker night.
    /// </summary>
    public class DarkerNightConfig
    {
        /// <summary>
        /// Is darker night enabled?
        /// </summary>
        public bool Enabled;
        /// <summary>
        /// The intensity for how dark it gets at night.
        /// </summary>
        public float DarknessIntensity;
        public DarkerNightConfig()
        {
            this.Enabled = true;
            this.DarknessIntensity = .9f;
        }
    }
}
