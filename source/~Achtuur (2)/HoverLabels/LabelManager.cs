/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using HoverLabels.Framework;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoverLabels;
internal class LabelManager
{
    public static readonly string NewLineDelimiter = "\n\n";

    /// <summary>
    /// Registered labels, one entry for each mod
    /// </summary>
    internal List<RegisteredLabel> RegisteredLabels { get; set; }

    internal IHoverLabel CurrentLabel;

    public LabelManager() 
    {
        RegisteredLabels = new();
    }

    public List<IManifest> GetUniqueRegisteredManifests()
    {
        return RegisteredLabels.Select(rl => rl.Manifest).Distinct().ToList();
    }

    public void TrySetLabel(Vector2 cursorTile)
    {
        IEnumerable<RegisteredLabel> labels = RegisteredLabels.Where(registeredLabel => registeredLabel.Enabled)
            .OrderByDescending(registeredLabel => registeredLabel.Label.Priority);

        this.CurrentLabel = null;
        foreach (RegisteredLabel registeredLabel in labels)
        {
            if (registeredLabel.Label.ShouldGenerateLabel(cursorTile))
            {
                this.CurrentLabel = registeredLabel.Label;
                break;
            }
        }
    }

    public void SetLabelEnabled(ModConfig config)
    {
        foreach(RegisteredLabel registeredLabel in RegisteredLabels)
        {
            registeredLabel.Enabled = config.IsLabelEnabled(registeredLabel.Manifest, registeredLabel.Name);
        }
    }

    public void AddLabel(IManifest manifest, string name, IHoverLabel label)
    {
        // label already exists
        if (RegisteredLabels.Any(label => label.Manifest == manifest && label.Name == name))
            return;

        RegisteredLabel newLabel = new RegisteredLabel(manifest, name, label);

        RegisteredLabels.Add(new RegisteredLabel(manifest, name, label));
    }
    public string GetDescriptionAsString()
    {
        return String.Join(NewLineDelimiter, CurrentLabel.GetDescription().ToArray());
    }

    public string GetLabelString()
    {
        return CurrentLabel.GetName() + (this.LabelHasDescription() ? NewLineDelimiter + this.GetDescriptionAsString() : "");

    }
    public Vector2 GetNameSize(SpriteFont font)
    {
        return font.MeasureString(CurrentLabel.GetName());
    }

    public Vector2 GetDescriptionSize(SpriteFont font)
    {
        if (this.LabelHasDescription())
            return font.MeasureString(this.GetDescriptionAsString());
        return Vector2.Zero;
    }
    public Vector2 GetLabelSize(SpriteFont nameFont, SpriteFont descFont)
    {
        Vector2 nameSize = this.GetNameSize(nameFont);
        Vector2 descSize = this.GetDescriptionSize(descFont);
        return new Vector2(Math.Max(nameSize.X, descSize.X), nameSize.Y + descSize.Y);
    }
    
    public bool HasLabel()
    {
        return this.CurrentLabel is not null;
    }

    public bool LabelHasDescription()
    {
        return CurrentLabel.GetDescription().Count() > 0;
    }

    internal void ClearLabel()
    {
        this.CurrentLabel = null;
    }
}
