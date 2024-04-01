/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.SpritePatcher.Framework.Services.Operations;

using Microsoft.Xna.Framework;
using StardewMods.SpritePatcher.Framework.Interfaces;

internal sealed class BasePatchHandler : IPatchHandler
{
    public void ApplyTexture(IRawTextureData inputTextureData, IRawTextureData outputTextureData)
    {
        for (var x = 0; x < inputTextureData.Width; x++)
        {
            for (var y = 0; y < inputTextureData.Height; y++)
            {
                var targetIndex = y * inputTextureData.Width + x;
                var targetColor = inputTextureData.Data[targetIndex];
                var newColor = targetColor;
                foreach (var operation in _operations)
                {
                    newColor = operation(targetIndex, newColor);
                }
                outputTextureData.Data[targetIndex] = newColor;
            }
        }
    }

    public IRawTextureData RegisterOperation(IRawTextureData inputTextureData)
    {
        for (var x = 0; x < inputTextureData.Width; x++)
        {
            for (var y = 0; y < inputTextureData.Height; y++)
            {
                var targetIndex = 
            }
        }
    }

    public Color RegisterOperation(Func<int, Color, Color> operation)
    {
        
    }
}