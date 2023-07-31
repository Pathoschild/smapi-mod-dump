/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
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
    internal static List<RegisteredLabel> RegisteredLabels { get; set; }

    internal IHoverLabel CurrentLabel;

    public LabelManager() 
    {
        RegisteredLabels = new();
    }

    public static List<IManifest> GetUniqueRegisteredManifests()
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

        //IEnumerable<KeyValuePair<string, IHoverLabel>> labels = RegisteredLabels.Values
        //    .SelectMany(labelDict => labelDict) //get all IHoverLabels inside second dict
        //    .OrderByDescending(kvp => kvp.Value.Priority); //order by IHoverLabel.Priority

        //List<(IManifest, string, IHoverLabel)> labels2 = new();
        //foreach(IManifest manifest in RegisteredLabels.Keys)
        //{
        //    foreach ((string name, IHoverLabel label) in RegisteredLabels[manifest])
        //        labels2.Add((manifest, name, label));
        //}

        //this.CurrentLabel = null;
        //foreach ((string name, IHoverLabel label) in labels)
        //{
        //    if (label.ShouldGenerateLabel(cursorTile))
        //    {
        //        this.CurrentLabel = label;
        //        break;
        //    }
        //}
    }

    public void AddLabel(IManifest manifest, string name, IHoverLabel label)
    {
        // label already exists
        if (RegisteredLabels.Any(label => label.Manifest == manifest && label.Name == name))
            return;

        RegisteredLabels.Add(new RegisteredLabel(manifest, name, label));

        //if (!this.RegisteredLabels.ContainsKey(manifest))
        //    this.RegisteredLabels.Add(manifest, new Dictionary<string, IHoverLabel>());

        //if (this.RegisteredLabels[manifest].ContainsKey(name))
        //    throw new Exception($"{name} label is already registered");

        //this.RegisteredLabels[manifest].Add(name, label);
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
