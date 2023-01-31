/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using CommunityToolkit.Diagnostics;
using Microsoft.Xna.Framework;

namespace AtraShared.Content.RawTexData;

public static class IRawTextureDataExtensions
{
    public static void PatchImage(
        this IRawTextureData raw,
        IRawTextureData source,
        Rectangle? sourceArea = null,
        Rectangle? targetArea = null,
        PatchMode patchMode = PatchMode.Replace)
    {
        Guard.IsNotNull(raw);
        Guard.IsNotNull(source);

        // Calculate bounds.
        sourceArea ??= new(0, 0, source.Width, source.Height);
        targetArea ??= new(0, 0, Math.Min(source.Width, raw.Width), Math.Min(source.Height, raw.Height));

        raw.ApplyPatch(source, sourceArea.Value, targetArea.Value, patchMode);
    }

    private static void ApplyPatch(
        this IRawTextureData raw,
        IRawTextureData source,
        Rectangle sourceArea,
        Rectangle targetArea,
        PatchMode patchMode
        )
    {
        // validate
        if (sourceArea.X < 0 || sourceArea.Y < 0 || sourceArea.Right > source.Width || sourceArea.Left > source.Height)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException($"Source rectangle appears to be out of range.");
        }

        if (targetArea.X < 0 || targetArea.Y < 0 || targetArea.Right > raw.Width || targetArea.Right > raw.Height)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException($"Target rectangle appears to be out of range.");
        }

        if (targetArea.Width != sourceArea.Width || targetArea.Height != sourceArea.Height)
        {
            ThrowHelper.ThrowArgumentException($"Size of target and source rectangles appear to be different.");
        }

        Color[] rawData = raw.Data;
        Color[] sourceData = source.Data;

        if (patchMode == PatchMode.Overlay)
        {
            for (int y = 0; y < targetArea.Height; y++)
            {
                for (int x = 0; x < targetArea.Width; x++)
                {
                    int sourcePixel = ((sourceArea.Y + y) * source.Width) + sourceArea.X + x;
                    int targetPixel = ((targetArea.Y + y) * raw.Width) + targetArea.X + x;

                    ref Color sourceColor = ref sourceData[sourcePixel];
                    if (sourceColor.A < 5)
                    {
                        continue;
                    }

                    ref Color targetColor = ref rawData[targetPixel];

                    if (targetColor.A < 5 || sourceColor.A > 250)
                    {
                        rawData[targetPixel] = sourceColor;
                        continue;
                    }

                    // perform merge (for premultiplied alpha).
                    // this formula matches the one used by SMAPI.
                    int remainingAlpha = byte.MaxValue - sourceColor.A;
                    rawData[targetPixel] = new Color(
                        r: sourceColor.R + (targetColor.R * remainingAlpha / byte.MaxValue),
                        b: sourceColor.B + (targetColor.B * remainingAlpha / byte.MaxValue),
                        g: sourceColor.G + (targetColor.G * remainingAlpha / byte.MaxValue),
                        alpha: Math.Max(sourceColor.A, targetColor.A));
                }
            }
        }
        else
        {
            if (sourceArea.X == 0 && source.Width == sourceArea.Width && targetArea.X == 0 && raw.Width == targetArea.Width)
            {
                int sourceIndex = sourceArea.Y * source.Width;
                int targetIndex = targetArea.Y * raw.Width;

                Array.Copy(sourceData, sourceIndex, rawData, targetIndex, sourceArea.Width * sourceArea.Height);
            }
            else
            {
                for (int y = 0; y < targetArea.Height; y++)
                {
                    int sourceIndex = ((y + sourceArea.Y) * source.Width) + sourceArea.X;
                    int targetIndex = ((y + targetArea.Y) * raw.Width) + targetArea.X;

                    Array.Copy(sourceData, sourceIndex, rawData, targetIndex, sourceArea.Width);
                }
            }
        }
    }
}
