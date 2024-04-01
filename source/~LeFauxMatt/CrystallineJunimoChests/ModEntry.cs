/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.CrystallineJunimoChests;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewMods.CrystallineJunimoChests.Models;
using StardewValley.Menus;
using StardewValley.Objects;

/// <inheritdoc />
public sealed class ModEntry : Mod
{
    private Texture2D texture = null!;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        // Init
        I18n.Init(this.Helper.Translation);
        this.texture = this.Helper.ModContent.Load<Texture2D>("assets/texture.png");
        _ = new ModPatches(this.Helper.ModContent, this.ModManifest, this.texture);

        // Events
        this.Helper.Events.Input.ButtonPressed += this.OnButtonPressed;
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!e.Button.IsUseToolButton()
            || Game1.activeClickableMenu is not ItemGrabMenu
            {
                context: Chest chest,
                chestColorPicker:
                { } chestColorPicker,
            })
        {
            return;
        }

        var (x, y) = e.Cursor.GetScaledScreenPixels();
        var area = new Rectangle(
            chestColorPicker.xPositionOnScreen + (IClickableMenu.borderWidth / 2),
            chestColorPicker.yPositionOnScreen + (IClickableMenu.borderWidth / 2),
            36 * DiscreteColorPicker.totalColors,
            28);

        if (!area.Contains(x, y))
        {
            return;
        }

        var selection = ((int)x - area.X) / 36;
        if (selection < 0 || selection >= DiscreteColorPicker.totalColors)
        {
            return;
        }

        var currentSelection = DiscreteColorPicker.getSelectionFromColor(chest.playerChoiceColor.Value);

        //--selection;

        if (selection == currentSelection)
        {
            return;
        }

        if (selection == 0)
        {
            chest.GlobalInventoryId = "JunimoChests";
            return;
        }

        this.Helper.Input.Suppress(e.Button);

        // Player has item
        var data = this.Helper.ModContent.Load<DataModel>("assets/data.json");
        var item = ItemRegistry.GetDataOrErrorItem(data.Colors[selection - 1].Item);
        if (Game1.player.Items.ContainsId(item.QualifiedItemId, data.Cost))
        {
            var responses = Game1.currentLocation.createYesNoResponses();
            Game1.currentLocation.createQuestionDialogue(
                I18n.Message_Confirm(
                    data.Cost,
                    item.DisplayName,
                    chest.DisplayName,
                    this.Helper.Translation.Get($"color.{data.Colors[selection].Name}")),
                responses,
                (who, whichAnswer) =>
                {
                    if (whichAnswer != "Yes")
                    {
                        return;
                    }

                    Game1.playSound(data.Sound);
                    who.Items.ReduceId(item.QualifiedItemId, data.Cost);
                    chest.GlobalInventoryId = $"{this.ModManifest.UniqueID}-{data.Colors[selection].Name}";
                    chest.playerChoiceColor.Value = DiscreteColorPicker.getColorFromSelection(selection);
                    chest.Location.temporarySprites.Add(
                        new TemporaryAnimatedSprite(
                            5,
                            (chest.TileLocation * Game1.tileSize) - new Vector2(0, 32),
                            DiscreteColorPicker.getColorFromSelection(selection))
                        {
                            layerDepth = 1f,
                        });
                });

            return;
        }

        Game1.drawObjectDialogue(
            I18n.Message_Alert(
                data.Cost,
                item.DisplayName,
                chest.DisplayName,
                this.Helper.Translation.Get($"color.{data.Colors[selection].Name}")));
    }
}