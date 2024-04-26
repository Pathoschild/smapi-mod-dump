/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using StardewModdingAPI;

namespace PersonalTravellingCart
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool DrawCartExterior { get; set; } = true;
        public bool DrawCartExteriorWeather { get; set; } = true;
        public bool Debug { get; set; } = false;
        public SButton HitchButton { get; set; } = SButton.Back;
        public string ThisPlayerCartLocationName { get; set; } = null;
    }
}
