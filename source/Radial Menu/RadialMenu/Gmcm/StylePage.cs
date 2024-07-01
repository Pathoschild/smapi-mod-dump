/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RadialMenu.Config;
using StardewModdingAPI;
using StardewValley;

namespace RadialMenu.Gmcm;

internal class StylePage(
    IGenericModMenuConfigApi gmcm,
    IGMCMOptionsAPI? gmcmOptions,
    IManifest mod,
    IModContentHelper modContent,
    ITranslationHelper translations,
    Func<Styles> getStyles)
{
    public const string ID = "MenuStyle";

    // Fields related to menu colors; these update the preview.
    private const string FIELD_ID_PREFIX = "focustense.RadialMenu.Style";
    private const string FIELD_ID_INNER_COLOR = $"{FIELD_ID_PREFIX}.InnerBackgroundColor";
    private const string FIELD_ID_OUTER_COLOR = $"{FIELD_ID_PREFIX}.OuterBackgroundColor";
    private const string FIELD_ID_SELECTION_COLOR = $"{FIELD_ID_PREFIX}.SelectionColor";
    private const string FIELD_ID_HIGHLIGHT_COLOR = $"{FIELD_ID_PREFIX}.HighlightColor";
    private const string FIELD_ID_CURSOR_COLOR = $"{FIELD_ID_PREFIX}.CursorColor";
    private const string FIELD_ID_TITLE_COLOR = $"{FIELD_ID_PREFIX}.TitleColor";
    private const string FIELD_ID_DESCRIPTION_COLOR = $"{FIELD_ID_PREFIX}.DescriptionColor";

    // A smaller image, scaled up, would be much, much more efficient to use here.
    // But we can't, because https://github.com/spacechase0/StardewValleyMods/issues/387.
    private const int PREVIEW_SIZE = 400;
    private const int PREVIEW_LENGTH = PREVIEW_SIZE * PREVIEW_SIZE;

    // Read-only aliases for dependencies
    private readonly IGenericModMenuConfigApi gmcm = gmcm;
    private readonly IGMCMOptionsAPI? gmcmOptions = gmcmOptions;
    private readonly IManifest mod = mod;
    private readonly IModContentHelper modContent = modContent;
    private readonly ITranslationHelper translations = translations;
    private readonly Func<Styles> getStyles = getStyles;

    // These assets are initialized in Setup(), so we assume they're not null once we need them.
    private readonly Color[] innerPreviewData = new Color[PREVIEW_LENGTH];
    private readonly Color[] outerPreviewData = new Color[PREVIEW_LENGTH];
    private readonly Color[] selectionPreviewData = new Color[PREVIEW_LENGTH];
    private readonly Color[] highlightPreviewData = new Color[PREVIEW_LENGTH];
    private readonly Color[] cursorPreviewData = new Color[PREVIEW_LENGTH];
    private readonly Color[] itemsPreviewData = new Color[PREVIEW_LENGTH];
    private readonly Color[] titlePreviewData = new Color[PREVIEW_LENGTH];
    private readonly Color[] descriptionPreviewData = new Color[PREVIEW_LENGTH];

    // GMCM won't update the actual configuration until it's saved, so we have to store transient
    // values (while editing) from OnFieldChanged in a local lookup.
    private readonly Dictionary<string, Color> liveColorValues = [];
    private Texture2D menuPreview = null!;

    protected Styles Styles => getStyles();

    public void Setup()
    {
        LoadAssets();
        RegisterPage();
        menuPreview = new(Game1.graphics.GraphicsDevice, PREVIEW_SIZE, PREVIEW_SIZE);

        // Do this early just to set up the dictionary. We'll do it again when the menu is opened.
        // It's not needed when editing colors as hex values, but with GMCMOptions (color picker)
        // enabled, the widgets are a bit eager and will try to fire the field-change handlers
        // before our hacked-up ComplexOption hook gets to run.
        UpdateColorValuesFromConfig();
    }

    /* ----- Setup Methods ----- */

    private void LoadAssets()
    {
        modContent.Load<Texture2D>("assets/preview-inner.png").GetData(innerPreviewData);
        modContent.Load<Texture2D>("assets/preview-outer.png").GetData(outerPreviewData);
        modContent.Load<Texture2D>("assets/preview-previous.png").GetData(selectionPreviewData);
        modContent.Load<Texture2D>("assets/preview-selection.png").GetData(highlightPreviewData);
        modContent.Load<Texture2D>("assets/preview-cursor.png").GetData(cursorPreviewData);
        modContent.Load<Texture2D>("assets/preview-items.png").GetData(itemsPreviewData);
        modContent.Load<Texture2D>("assets/preview-title.png").GetData(titlePreviewData);
        modContent.Load<Texture2D>("assets/preview-description.png").GetData(descriptionPreviewData);
    }

    private void RegisterPage()
    {
        gmcm.AddPage(mod, ID, () => translations.Get("gmcm.style"));

        gmcm.AddImage(mod, () => menuPreview, scale: 1);

        gmcm.AddSectionTitle(
            mod,
            text: () => translations.Get("gmcm.style.colors"));
        if (gmcmOptions is null)
        {
            gmcm.AddParagraph(
                mod,
                text: () => translations.Get("gmcm.style.colors.slidernote"));
        }
        AddColorOption(
            FIELD_ID_INNER_COLOR,
            name: () => translations.Get("gmcm.style.colors.inner"),
            tooltip: () => translations.Get("gmcm.style.colors.inner.tooltip"),
            getColor: () => Styles.InnerBackgroundColor,
            setColor: color => Styles.InnerBackgroundColor = color);
        AddColorOption(
            FIELD_ID_OUTER_COLOR,
            name: () => translations.Get("gmcm.style.colors.outer"),
            tooltip: () => translations.Get("gmcm.style.colors.outer.tooltip"),
            getColor: () => Styles.OuterBackgroundColor,
            setColor: color => Styles.OuterBackgroundColor = color);
        AddColorOption(
            FIELD_ID_SELECTION_COLOR,
            name: () => translations.Get("gmcm.style.colors.selection"),
            tooltip: () => translations.Get("gmcm.style.colors.selection.tooltip"),
            getColor: () => Styles.SelectionColor,
            setColor: color => Styles.SelectionColor = color);
        AddColorOption(
            FIELD_ID_HIGHLIGHT_COLOR,
            name: () => translations.Get("gmcm.style.colors.highlight"),
            tooltip: () => translations.Get("gmcm.style.colors.highlight.tooltip"),
            getColor: () => Styles.HighlightColor,
            setColor: color => Styles.HighlightColor = color);
        AddColorOption(
            FIELD_ID_CURSOR_COLOR,
            name: () => translations.Get("gmcm.style.colors.cursor"),
            tooltip: () => translations.Get("gmcm.style.colors.cursor.tooltip"),
            getColor: () => Styles.CursorColor,
            setColor: color => Styles.CursorColor = color);
        AddColorOption(
            FIELD_ID_TITLE_COLOR,
            name: () => translations.Get("gmcm.style.colors.title"),
            tooltip: () => translations.Get("gmcm.style.colors.title.tooltip"),
            getColor: () => Styles.SelectionTitleColor,
            setColor: color => Styles.SelectionTitleColor = color);
        AddColorOption(
            FIELD_ID_DESCRIPTION_COLOR,
            name: () => translations.Get("gmcm.style.colors.description"),
            tooltip: () => translations.Get("gmcm.style.colors.description.tooltip"),
            getColor: () => Styles.SelectionDescriptionColor,
            setColor: color => Styles.SelectionDescriptionColor = color);

        gmcm.AddSectionTitle(
            mod,
            text: () => translations.Get("gmcm.style.dimensions"));
        gmcm.AddParagraph(
            mod,
            text: () => translations.Get("gmcm.style.dimensions.note"));
        gmcm.AddNumberOption(
            mod,
            name: () => translations.Get("gmcm.style.dimensions.inner"),
            tooltip: () => translations.Get("gmcm.style.dimensions.inner.tooltip"),
            getValue: () => Styles.InnerRadius,
            setValue: value => Styles.InnerRadius = value,
            min: 200,
            max: 400,
            interval: 25);
        gmcm.AddNumberOption(
            mod,
            name: () => translations.Get("gmcm.style.dimensions.outer"),
            tooltip: () => translations.Get("gmcm.style.dimensions.outer.tooltip"),
            getValue: () => Styles.OuterRadius,
            setValue: value => Styles.OuterRadius = value,
            min: 100,
            max: 200,
            interval: 10);
        gmcm.AddNumberOption(
            mod,
            name: () => translations.Get("gmcm.style.dimensions.gap"),
            tooltip: () => translations.Get("gmcm.style.dimensions.gap.tooltip"),
            getValue: () => Styles.GapWidth,
            setValue: value => Styles.GapWidth = value,
            min: 0,
            max: 20);
        gmcm.AddNumberOption(
            mod,
            name: () => translations.Get("gmcm.style.dimensions.cursor.size"),
            tooltip: () => translations.Get("gmcm.style.dimensions.cursor.size.tooltip"),
            getValue: () => Styles.CursorSize,
            setValue: value => Styles.CursorSize = value,
            min: 16,
            max: 64,
            interval: 4);
        gmcm.AddNumberOption(
            mod,
            name: () => translations.Get("gmcm.style.dimensions.cursor.distance"),
            tooltip: () => translations.Get("gmcm.style.dimensions.cursor.distance.tooltip"),
            getValue: () => Styles.CursorDistance,
            setValue: value => Styles.CursorDistance = value,
            min: 0,
            max: 16);
        gmcm.AddNumberOption(
            mod,
            name: () => translations.Get("gmcm.style.dimensions.itemheight"),
            tooltip: () => translations.Get("gmcm.style.dimensions.itemheight.tooltip"),
            getValue: () => Styles.MenuSpriteHeight,
            setValue: value => Styles.MenuSpriteHeight = value,
            min: 16,
            max: 64,
            interval: 8);
        gmcm.AddNumberOption(
            mod,
            name: () => translations.Get("gmcm.style.dimensions.selectionheight"),
            tooltip: () => translations.Get("gmcm.style.dimensions.selectionheight.tooltip"),
            getValue: () => Styles.SelectionSpriteHeight,
            setValue: value => Styles.SelectionSpriteHeight = value,
            min: 32,
            max: 256,
        interval: 16);

        // This option isn't meant to do anything in the UI; it's a hack for us to get access to the
        // menu events, so that we can automatically reload the configuration into transient values
        // and rebuild the preview.
        //
        // Without this, we could add similar logic in Setup(), but it would only run when the menu
        // is created, leaving easy-to-repro bugs with stale state by simply cancelling out of the
        // sub-menu and then returning to it.
        //
        // The dummy option will unfortunately leave a bit of extra space at the bottom of the menu,
        // despite the row/elements having a height of 0, but this is far less noticeable than the
        // preview having the wrong colors.
        gmcm.AddComplexOption(
            mod,
            name: () => "",
            draw: (_, _) => { },
            height: () => 0,
            beforeMenuOpened: () =>
            {
                UpdateColorValuesFromConfig();
                UpdateMenuPreview();
            });

        gmcm.OnFieldChanged(mod, (fieldId, value) =>
        {
            switch (fieldId)
            {
                case FIELD_ID_OUTER_COLOR:
                case FIELD_ID_INNER_COLOR:
                case FIELD_ID_SELECTION_COLOR:
                case FIELD_ID_HIGHLIGHT_COLOR:
                case FIELD_ID_CURSOR_COLOR:
                case FIELD_ID_TITLE_COLOR:
                case FIELD_ID_DESCRIPTION_COLOR:
                    if (GetColorFromAmbiguousType(value) is Color color)
                    {
                        liveColorValues[fieldId] = color;
                        UpdateMenuPreview();
                    }
                    break;
            }
        });
    }

    private void AddColorOption(
        string fieldId,
        Func<string> name,
        Func<HexColor> getColor,
        Action<HexColor> setColor,
        Func<string>? tooltip = null)
    {
        if (gmcmOptions is not null)
        {
            gmcmOptions.AddColorOption(
                mod,
                name: name,
                tooltip: tooltip,
                fieldId: fieldId,
                getValue: () => getColor(),
                setValue: value => setColor(new(value)),
                colorPickerStyle: (uint)(
                    IGMCMOptionsAPI.ColorPickerStyle.RGBSliders
                    | IGMCMOptionsAPI.ColorPickerStyle.HSLColorWheel
                    | IGMCMOptionsAPI.ColorPickerStyle.HSVColorWheel
                    | IGMCMOptionsAPI.ColorPickerStyle.RadioChooser));
        }
        else
        {
            gmcm.AddTextOption(
                mod,
                name: name,
                tooltip: tooltip,
                fieldId: fieldId,
                getValue: () => getColor().ToString(),
                setValue: hexString =>
                {
                    if (HexColor.TryParse(hexString, out var hexColor))
                    {
                        setColor(hexColor);
                    }
                });
        }
    }

    private static Color? GetColorFromAmbiguousType(object value)
    {
        if (value is Color color)
        {
            return color;
        }
        if (value is string formattedColor
            && HexColor.TryParse(formattedColor, out var hexColor))
        {
            return hexColor;
        }
        return null;
    }

    /* ----- Live Updates ----- */

    private void UpdateColorValuesFromConfig()
    {
        liveColorValues[FIELD_ID_INNER_COLOR] = Styles.InnerBackgroundColor;
        liveColorValues[FIELD_ID_OUTER_COLOR] = Styles.OuterBackgroundColor;
        liveColorValues[FIELD_ID_SELECTION_COLOR] = Styles.SelectionColor;
        liveColorValues[FIELD_ID_HIGHLIGHT_COLOR] = Styles.HighlightColor;
        liveColorValues[FIELD_ID_CURSOR_COLOR] = Styles.CursorColor;
        liveColorValues[FIELD_ID_TITLE_COLOR] = Styles.SelectionTitleColor;
        liveColorValues[FIELD_ID_DESCRIPTION_COLOR] = Styles.SelectionDescriptionColor;
    }

    private void UpdateMenuPreview()
    {
        Color[] previewData = new Color[PREVIEW_LENGTH];
        for (var i = 0; i < PREVIEW_LENGTH; i++)
        {
            var innerColor =
                Premultiply(liveColorValues[FIELD_ID_INNER_COLOR], innerPreviewData[i].A);
            var outerColor =
                Premultiply(liveColorValues[FIELD_ID_OUTER_COLOR], outerPreviewData[i].A);
            var selectionColor =
                Premultiply(liveColorValues[FIELD_ID_SELECTION_COLOR], selectionPreviewData[i].A);
            var highlightColor =
                Premultiply(liveColorValues[FIELD_ID_HIGHLIGHT_COLOR], highlightPreviewData[i].A);
            var cursorColor =
                Premultiply(liveColorValues[FIELD_ID_CURSOR_COLOR], cursorPreviewData[i].A);
            var itemsColor = Premultiply(Color.DarkGreen, itemsPreviewData[i].A);
            var titleColor =
                Premultiply(liveColorValues[FIELD_ID_TITLE_COLOR], titlePreviewData[i].A);
            var descriptionColor = Premultiply(
                liveColorValues[FIELD_ID_DESCRIPTION_COLOR],
                descriptionPreviewData[i].A);
            previewData[i] = innerColor
                .BlendPremultiplied(outerColor)
                .BlendPremultiplied(selectionColor)
                .BlendPremultiplied(highlightColor)
                .BlendPremultiplied(cursorColor)
                .BlendPremultiplied(itemsColor)
                .BlendPremultiplied(titleColor)
                .BlendPremultiplied(descriptionColor);
        }
        menuPreview.SetData(previewData);
    }

    private static Color Premultiply(Color color, byte alpha)
    {
        var a = (color.A / 255f) * (alpha / 255f);
        return new Color(
            (byte)(color.R * a),
            (byte)(color.G * a),
            (byte)(color.B * a),
            (byte)(a * 255));
    }
}

static class ColorExt
{
    public static Color BlendPremultiplied(this Color under, Color over)
    {
        var a1 = over.A / 255f;
        var a2 = under.A / 255f;

        byte r = (byte)MathF.Min(255, over.R + under.R * (1 - a1));
        byte g = (byte)MathF.Min(255, over.G + under.G * (1 - a1));
        byte b = (byte)MathF.Min(255, over.B + under.B * (1 - a1));
        byte a = (byte)(255 * (a1 + a2 * (1 - a1)));
        return new Color(r, g, b, a);
    }
}