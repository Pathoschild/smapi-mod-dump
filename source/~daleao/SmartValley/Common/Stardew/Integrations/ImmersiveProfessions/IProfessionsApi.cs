/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Common.Stardew.Integrations;

#region using directives

using StardewValley;

#endregion using directives

public interface IProfessionsApi
{
    public int GetForageQuality(Farmer farmer);
    public int GetEcologistItemsForaged(Farmer farmer);
    public int GetMineralQuality(Farmer farmer);
    public int GetGemologistMineralsCollected(Farmer farmer);
    public int GetConservationistTrashCollected(Farmer farmer);
    public float GetConservationistTaxBonus(Farmer farmer);
    public int GetRegisteredUltimate(Farmer farmer);
}