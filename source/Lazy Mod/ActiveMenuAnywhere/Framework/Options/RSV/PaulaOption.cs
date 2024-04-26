/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using System.Reflection;
using Common.Integration;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace ActiveMenuAnywhere.Framework.Options;

public class PaulaOption : BaseOption
{
    private readonly IModHelper helper;
    
    public PaulaOption(Rectangle sourceRect, IModHelper helper) : base(I18n.Option_Paula(), sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        var lanHouse = RSVIntegration.GetType("RidgesideVillage.PaulaClinic");
        lanHouse?.GetMethod("ClinicChoices", BindingFlags.NonPublic|BindingFlags.Static)?.Invoke(null, null);
    }
}