/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Text;
using Microsoft.Xna.Framework;

namespace MappingExtensionsAndExtraProperties.Models.FarmAnimals;

public class Animal
{
    public string? Id { get; set; }
    public string? AnimalId { get; set; }
    public string? SkinId { get; set; }
    public int Age { get; set; }
    public string? LocationId { get; set; }
    public string? DisplayName { get; set; }
    public string[]? PetMessage { get; set; }
    public int HomeTileX { get; set; }
    public int HomeTileY { get; set; }
    public string? Condition { get; set; }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine();
        sb.Append("---------------");
        sb.AppendLine();

        sb.Append($"Id: {this.Id}");
        sb.AppendLine();
        sb.Append($"AnimalId: {this.AnimalId}");
        sb.AppendLine();
        sb.Append($"SkinId: {this.SkinId}");
        sb.AppendLine();
        sb.Append($"Age: {this.Age}");
        sb.AppendLine();
        sb.Append($"LocationId: {this.LocationId}");
        sb.AppendLine();
        sb.Append($"DisplayName: {this.DisplayName}");
        sb.AppendLine();

        foreach (string s in this.PetMessage)
        {
            sb.Append($"PetMessage: {s}");
            sb.AppendLine();
        }

        sb.Append($"HomeTileX {this.HomeTileX}");
        sb.AppendLine();
        sb.Append($"HomeTileY: {this.HomeTileY}");
        sb.AppendLine();
        sb.Append($"Condition: {this.Condition}");
        sb.AppendLine();


        sb.Append("---------------");
        sb.AppendLine();

        return sb.ToString();
    }
}
