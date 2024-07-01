/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class PaulaOption : BaseOption
{
    public PaulaOption(Rectangle sourceRect) : base(I18n.Option_Paula(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        RSVReflection.GetRSVPrivateStaticMethod("RidgesideVillage.PaulaClinic", "ClinicChoices").Invoke(null, null);
    }
}