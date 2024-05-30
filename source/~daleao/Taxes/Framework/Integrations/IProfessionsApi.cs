/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Taxes.Framework.Integrations;

public interface IProfessionsApi
{
    public interface IProfessionsConfig
    {
        uint ConservationistTrashNeededPerTaxDeduction { get; }

        float ConservationistTaxDeductionCeiling { get; }
    }

    float GetConservationistTaxDeduction(Farmer? farmer = null);

    IProfessionsConfig GetConfig();
}
