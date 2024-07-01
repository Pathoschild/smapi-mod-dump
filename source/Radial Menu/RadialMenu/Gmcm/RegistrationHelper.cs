/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using StardewValley;
using Microsoft.Xna.Framework;

namespace RadialMenu.Gmcm;

// Wrapper around the IGenericModMenuConfigApi interface, except that:
//
// - The `mod` is provided in ctor so we don't have to keep specifying it;
// - It automatically tracks the table position of each field so we can look it up later.
//
// The second point, in particular, is what's relevant for some of our hacks, as we need to "force
// update" the table at specific positions, and that means knowing where the fields are.
internal class RegistrationHelper(IGenericModMenuConfigApi api, IManifest mod)
{
    private readonly IGenericModMenuConfigApi api = api;
    private readonly IManifest mod = mod;
    private readonly Dictionary<string, int> tablePositionsByFieldId = [];

    // Start at 1 because position 0 is used for scrollbar.
    private int currentTablePosition = 1;

    public int GetTablePosition(string fieldId)
    {
        return tablePositionsByFieldId[fieldId];
    }

    public void AddSectionTitle(Func<string> text, Func<string>? tooltip = null)
    {
        api.AddSectionTitle(mod, text, tooltip);
        currentTablePosition++;
    }

    public void AddParagraph(Func<string> text)
    {
        api.AddParagraph(mod, text);
        currentTablePosition++;
    }

    public void AddImage(
        Func<Texture2D> texture,
        Rectangle? texturePixelArea = null,
        int scale = Game1.pixelZoom)
    {
        api.AddImage(mod, texture, texturePixelArea, scale);
        currentTablePosition++;
    }

    public void AddBoolOption(
        Func<bool> getValue,
        Action<bool> setValue,
        Func<string> name,
        Func<string>? tooltip = null,
        string? fieldId = null)
    {
        api.AddBoolOption(mod, getValue, setValue, name, tooltip, fieldId);
        if (fieldId is not null)
        {
            tablePositionsByFieldId.Add(fieldId, currentTablePosition);
        }
        currentTablePosition += 2;
    }

    public void AddNumberOption(
        Func<int> getValue,
        Action<int> setValue,
        Func<string> name,
        Func<string>? tooltip = null,
        int? min = null,
        int? max = null,
        int? interval = null,
        Func<int, string>? formatValue = null,
        string? fieldId = null)
    {
        api.AddNumberOption(
            mod, getValue, setValue, name, tooltip, min, max, interval, formatValue, fieldId);
        if (fieldId is not null)
        {
            tablePositionsByFieldId.Add(fieldId, currentTablePosition);
        }
        currentTablePosition += 3;
    }

    public void AddNumberOption(
        Func<float> getValue,
        Action<float> setValue,
        Func<string> name,
        Func<string>? tooltip = null,
        float? min = null,
        float? max = null,
        float? interval = null,
        Func<float, string>? formatValue = null,
        string? fieldId = null)
    {
        api.AddNumberOption(
            mod, getValue, setValue, name, tooltip, min, max, interval, formatValue, fieldId);
        if (fieldId is not null)
        {
            tablePositionsByFieldId.Add(fieldId, currentTablePosition);
        }
        currentTablePosition += 3;
    }

    public void AddTextOption(
        Func<string> getValue,
        Action<string> setValue,
        Func<string> name,
        Func<string>? tooltip = null,
        string[]? allowedValues = null,
        Func<string, string>? formatAllowedValue = null,
        string? fieldId = null)
    {
        api.AddTextOption(
            mod, getValue, setValue, name, tooltip, allowedValues, formatAllowedValue, fieldId);
        if (fieldId is not null)
        {
            tablePositionsByFieldId.Add(fieldId, currentTablePosition);
        }
        currentTablePosition += 2;
    }

    public void AddKeybind(
        Func<SButton> getValue,
        Action<SButton> setValue,
        Func<string> name,
        Func<string>? tooltip = null,
        string? fieldId = null)
    {
        api.AddKeybind(mod, getValue, setValue, name, tooltip, fieldId);
        if (fieldId is not null)
        {
            tablePositionsByFieldId.Add(fieldId, currentTablePosition);
        }
        currentTablePosition += 2;
    }

    public void AddKeybindList(
        Func<KeybindList> getValue,
        Action<KeybindList> setValue,
        Func<string> name,
        Func<string>? tooltip = null,
        string? fieldId = null)
    {
        api.AddKeybindList(mod, getValue, setValue, name, tooltip, fieldId);
        if (fieldId is not null)
        {
            tablePositionsByFieldId.Add(fieldId, currentTablePosition);
        }
        currentTablePosition += 2;
    }

    public void AddPageLink(string pageId, Func<string> text, Func<string>? tooltip = null)
    {
        api.AddPageLink(mod, pageId, text, tooltip);
        currentTablePosition++;
    }

    public void AddComplexOption(
        Func<string> name,
        Action<SpriteBatch, Vector2> draw,
        Func<string>? tooltip = null,
        Action? beforeMenuOpened = null,
        Action? beforeSave = null,
        Action? afterSave = null,
        Action? beforeReset = null,
        Action? afterReset = null,
        Action? beforeMenuClosed = null,
        Func<int>? height = null,
        string? fieldId = null)
    {
        api.AddComplexOption(
            mod, name, draw, tooltip, beforeMenuOpened, beforeSave, afterSave, beforeReset,
            afterReset, beforeMenuClosed, height, fieldId);
        if (fieldId is not null)
        {
            tablePositionsByFieldId.Add(fieldId, currentTablePosition);
        }
        currentTablePosition += 2;
    }
}
