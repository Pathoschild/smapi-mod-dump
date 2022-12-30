/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Slothsoft.Informant.Api;
using StardewValley.Locations;
using StardewValley.Menus;

namespace Slothsoft.Informant.Implementation.Decorator;

internal class FieldOfficeDecorator : IDecorator<Item> {

    private static Texture2D? _fieldOffice;
    
    private readonly IModHelper _modHelper;
    
    public FieldOfficeDecorator(IModHelper modHelper) {
        _modHelper = modHelper;
        _fieldOffice ??= modHelper.ModContent.Load<Texture2D>("assets/field_office.png");
    }

    public string Id => "fieldoffice";
    public string DisplayName => _modHelper.Translation.Get("FieldOfficeDecorator");
    public string Description => _modHelper.Translation.Get("FieldOfficeDecorator.Description");

    public bool HasDecoration(Item input) {
        var islandFieldOffice = (IslandNorth) Game1.getLocationFromName("IslandNorth");
        if (!islandFieldOffice.caveOpened.Value) {
            // the field office is not open yet
            return false;
        }    
        if (_fieldOffice != null && input is SObject obj && !obj.bigCraftable.Value) {
            // this method highlights the bones that are still needed, so perfect for this decorator
            return FieldOfficeMenu.highlightBones(obj);
        }
        return false;
    }
    
    public Decoration Decorate(Item input) {
        return new Decoration(_fieldOffice!);
    }
}