/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Slothsoft.Informant.Api;
using Slothsoft.Informant.Implementation.Common;

namespace Slothsoft.Informant.Implementation.Decorator;

internal class RarecrowDecorator : IDecorator<Item> {
    private static Texture2D? _rarecrow;

    private readonly IModHelper _modHelper;

    public RarecrowDecorator(IModHelper modHelper) {
        _modHelper = modHelper;
        _rarecrow ??= modHelper.ModContent.Load<Texture2D>("assets/rarecrow.png");
    }

    public string Id => "rarecrow";
    public string DisplayName => _modHelper.Translation.Get("RarecrowDecorator");
    public string Description => _modHelper.Translation.Get("RarecrowDecorator.Description");

    public bool HasDecoration(Item input) {
        if (_rarecrow != null && input is SObject obj && obj.bigCraftable.Value) {
            return BigCraftableIds.AllRarecrows.Contains(obj.ParentSheetIndex) &&
                   !Utility.doesItemWithThisIndexExistAnywhere(obj.ParentSheetIndex, bigCraftable: true);
        }
        return false;
    }

    public Decoration Decorate(Item input) {
        return new Decoration(_rarecrow!);
    }
}