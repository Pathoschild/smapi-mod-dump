/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Objects;

namespace PuzzleFloor;

[HarmonyPatch(typeof(Furniture), nameof(Furniture.updateRotation))]
internal class Furniture_UpdateRotation
{
    public static bool Prefix(Furniture __instance)
    {
        if (!__instance.QualifiedItemId.StartsWith("(F)drbirbdev.PuzzleFloor"))
        {
            return true;
        }

        // TODO: this is just copied from furniture class.  Actually patch to make rugs fully rotateable
        __instance.Flipped = false;
        if (__instance.currentRotation.Value > 0)
        {
            Point specialRotationOffsets = __instance.furniture_type.Value switch
            {
                2 => new Point(-1, 1),
                5 => new Point(-1, 0),
                3 => new Point(-1, 1),
                _ => Point.Zero
            };
            bool differentSizesFor2Rotations =
                (__instance.rotations.Value == 2 && __instance.furniture_type.Value == 5) ||
                __instance.furniture_type.Value == 12 || __instance.QualifiedItemId == "(F)724" ||
                __instance.QualifiedItemId == "(F)727";
            bool sourceRectRotate = __instance.defaultBoundingBox.Width != __instance.defaultBoundingBox.Height;
            if (differentSizesFor2Rotations && __instance.currentRotation.Value == 2)
            {
                __instance.currentRotation.Value = 1;
            }

            if (sourceRectRotate)
            {
                int oldBoundingBoxHeight = __instance.boundingBox.Height;
                switch (__instance.currentRotation.Value)
                {
                    case 0:
                    case 2:
                        __instance.boundingBox.Height = __instance.defaultBoundingBox.Height;
                        __instance.boundingBox.Width = __instance.defaultBoundingBox.Width;
                        break;
                    case 1:
                    case 3:
                        __instance.boundingBox.Height = __instance.boundingBox.Width + (specialRotationOffsets.X * 64);
                        __instance.boundingBox.Width = oldBoundingBoxHeight + (specialRotationOffsets.Y * 64);
                        break;
                }
            }

            Point specialSpecialSourceRectOffset =
                (__instance.furniture_type.Value == 12) ? new Point(1, -1) : Point.Zero;
            if (sourceRectRotate)
            {
                switch (__instance.currentRotation.Value)
                {
                    case 0:
                        __instance.sourceRect.Value = __instance.defaultSourceRect.Value;
                        break;
                    case 1:
                        __instance.sourceRect.Value = new Rectangle(
                            __instance.defaultSourceRect.X + __instance.defaultSourceRect.Width,
                            __instance.defaultSourceRect.Y,
                            __instance.defaultSourceRect.Height - 16 + (specialRotationOffsets.Y * 16) +
                            (specialSpecialSourceRectOffset.X * 16),
                            __instance.defaultSourceRect.Width + 16 + (specialRotationOffsets.X * 16) +
                            (specialSpecialSourceRectOffset.Y * 16));
                        break;
                    case 2:
                        __instance.sourceRect.Value = new Rectangle(
                            __instance.defaultSourceRect.X + __instance.defaultSourceRect.Width +
                            __instance.defaultSourceRect.Height - 16 + (specialRotationOffsets.Y * 16) +
                            (specialSpecialSourceRectOffset.X * 16), __instance.defaultSourceRect.Y,
                            __instance.defaultSourceRect.Width, __instance.defaultSourceRect.Height);
                        break;
                    case 3:
                        __instance.sourceRect.Value = new Rectangle(
                            __instance.defaultSourceRect.X + __instance.defaultSourceRect.Width,
                            __instance.defaultSourceRect.Y,
                            __instance.defaultSourceRect.Height - 16 + (specialRotationOffsets.Y * 16) +
                            (specialSpecialSourceRectOffset.X * 16),
                            __instance.defaultSourceRect.Width + 16 + (specialRotationOffsets.X * 16) +
                            (specialSpecialSourceRectOffset.Y * 16));
                        __instance.Flipped = true;
                        break;
                }
            }
            else
            {
                __instance.Flipped = __instance.currentRotation.Value == 3;
                __instance.sourceRect.Value = __instance.rotations.Value == 2
                    ? new Rectangle(
                        __instance.defaultSourceRect.X + (((__instance.currentRotation.Value == 2) ? 1 : 0) *
                                                          __instance.defaultSourceRect.Width),
                        __instance.defaultSourceRect.Y, __instance.defaultSourceRect.Width,
                        __instance.defaultSourceRect.Height)
                    : new Rectangle(
                        __instance.defaultSourceRect.X +
                        (((__instance.currentRotation.Value == 3) ? 1 : __instance.currentRotation.Value) *
                         __instance.defaultSourceRect.Width), __instance.defaultSourceRect.Y,
                        __instance.defaultSourceRect.Width, __instance.defaultSourceRect.Height);
            }

            if (differentSizesFor2Rotations && __instance.currentRotation.Value == 1)
            {
                __instance.currentRotation.Value = 2;
            }
        }
        else
        {
            __instance.sourceRect.Value = __instance.defaultSourceRect.Value;
            __instance.boundingBox.Value = __instance.defaultBoundingBox.Value;
        }

        __instance.updateDrawPosition();

        return false;
    }
}
