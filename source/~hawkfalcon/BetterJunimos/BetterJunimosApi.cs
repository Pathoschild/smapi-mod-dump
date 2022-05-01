/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using BetterJunimos.Abilities;
using BetterJunimos.Utils;

namespace BetterJunimos {
    public interface IBetterJunimosApi {
        public int GetJunimoHutMaxRadius();

        public int GetJunimoHutMaxJunimos();

        public Dictionary<string, bool> GetJunimoAbilities();

        public void RegisterJunimoAbility(IJunimoAbility junimoAbility);

        public bool GetWereJunimosPaidToday();

        public void ShowPerfectionTracker();
        public void ShowConfigurationMenu();

        public void ListAvailableActions(Guid hutGuid);
        public CropMap GetCropMapForHut(Guid hutGuid);
        public void SetCropMapForHut(Guid hutGuid, CropMap map);
        public void ClearCropMapForHut(Guid hutGuid);
    }

    public class BetterJunimosApi : IBetterJunimosApi {
        public int GetJunimoHutMaxRadius() {
            return Util.CurrentWorkingRadius;
        }

        public int GetJunimoHutMaxJunimos() {
            return Util.Progression.MaxJunimosUnlocked;
        }

        public Dictionary<string, bool> GetJunimoAbilities() {
            return BetterJunimos.Config.JunimoAbilities;
        }

        public void RegisterJunimoAbility(IJunimoAbility junimoAbility) {
            Util.Abilities.RegisterJunimoAbility(junimoAbility);
        }

        public bool GetWereJunimosPaidToday() {
            return Util.Payments.WereJunimosPaidToday;
        }

        public void ShowPerfectionTracker() {
            Util.Progression.ShowPerfectionTracker();
        }
        
        public void ShowConfigurationMenu() {
            Util.Progression.ShowConfigurationMenu();
        }

        public void ListAvailableActions(Guid hutGuid) {
            Util.Progression.ListAvailableActions(hutGuid);
        }

        public CropMap GetCropMapForHut(Guid hutGuid) {
            return BetterJunimos.CropMaps.GetCropMapForHut(Util.GetHutFromId(hutGuid));
        }

        public void SetCropMapForHut(Guid hutGuid, CropMap map) {
            BetterJunimos.CropMaps.SetCropMapForHut(Util.GetHutFromId(hutGuid), map);
        }

        public void ClearCropMapForHut(Guid hutGuid) {
            BetterJunimos.CropMaps.ClearCropMapForHut(Util.GetHutFromId(hutGuid));
        }
    }
}