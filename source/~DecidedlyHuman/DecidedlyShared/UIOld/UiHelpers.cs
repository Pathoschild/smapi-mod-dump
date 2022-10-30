/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DecidedlyShared.UIOld;

public static class UiHelpers
{
    public static int GetYPositionInScrollArea(int numberOfElements, int maxElementsVisible,
                                               int scrollAreaHeight, int scrollAreaTop,
                                               int currentTopIndex)
    {
        int barSteps = Math.Max(numberOfElements - maxElementsVisible, 1);
        int barStepSize = scrollAreaHeight / barSteps;

        return scrollAreaTop + currentTopIndex * barStepSize;
    }

    public static int GetTopIndexFromYPosition(int numberOfElements, int maxElementsVisible,
                                               int scrollAreaHeight, int scrollAreaTop, int mouseY)
    {
        mouseY -= scrollAreaTop - 32;
        int barSteps = Math.Max(numberOfElements - maxElementsVisible, 1);
        int barStepSize = scrollAreaHeight / barSteps;

        return mouseY / barStepSize;
    }
}
