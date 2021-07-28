/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using System;

namespace PlatoTK.Presets
{
    internal class PresetHelper : IPresetHelper
    {
        internal IPlatoHelper Helper;

        public PresetHelper(IPlatoHelper helper)
        {
            Helper = helper;
        }

        public void RegisterArcade(string id, string name, string objectName, Action start, string sprite, string iconForMobilePhone)
        {
            ArcadeMachinePreset.Add(new ArcadeMachineSpecs(name, Helper.ModHelper.ModRegistry.ModID + id, objectName, start, sprite, iconForMobilePhone), Helper);
        }
    }
}
