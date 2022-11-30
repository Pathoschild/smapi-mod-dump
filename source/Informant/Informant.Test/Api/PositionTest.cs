/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using Microsoft.Xna.Framework;
using NUnit.Framework;
using Slothsoft.Informant.Api;

namespace InformantTest.Api; 

public class PositionTest {
    
    [Test]
    [TestCaseSource(nameof(_bigTooltipSmallIconCases))]
    public void CalculatePosition(string displayName, IPosition position, Rectangle tooltipBounds, Vector2 iconSize, Rectangle expectedPosition) {
        Assert.AreEqual(expectedPosition, position.CalculateIconPosition(tooltipBounds, iconSize), $"Position was wrong for {displayName}");
    }

    private static object[] _bigTooltipSmallIconCases =
    {
        new object[] {
            "TopLeft",
            IPosition.TopLeft,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(100, 200, 5, 10)
        },
        new object[] {
            "TopCenter",
            IPosition.TopCenter,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(117, 200, 5, 10)
        },
        new object[] {
            "TopRight",
            IPosition.TopRight,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(135, 200, 5, 10)
        },
        new object[] {
            "CenterLeft",
            IPosition.CenterLeft,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(100, 210, 5, 10)
        },
        new object[] {
            "Center",
            IPosition.Center,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(117, 210, 5, 10)
        },
        new object[] {
            "CenterRight",
            IPosition.CenterRight,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(135, 210, 5, 10)
        },
        new object[] {
            "BottomLeft",
            IPosition.BottomLeft,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(100, 220, 5, 10)
        },
        new object[] {
            "BottomCenter",
            IPosition.BottomCenter,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(117, 220, 5, 10)
        },
        new object[] {
            "BottomRight",
            IPosition.BottomRight,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(135, 220, 5, 10)
        },
        new object[] {
            "Fill",
            IPosition.Fill,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(100, 200, 40, 30)
        },
    };
    
    [Test]
    [TestCaseSource(nameof(_tooltipPosition))]
    public void CalculateTooltipPosition(string displayName, IPosition position, Rectangle tooltipBounds, Vector2 iconSize, Rectangle expectedPosition) {
        Assert.AreEqual(expectedPosition, position.CalculateTooltipPosition(tooltipBounds, iconSize), $"Position was wrong for {displayName}");
    }

    private static object[] _tooltipPosition =
    {
        new object[] {
            "TopLeft",
            IPosition.TopLeft,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(95, 200, 45, 30)
        },
        new object[] {
            "TopCenter",
            IPosition.TopCenter,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(100, 190, 40, 40),
        },
        new object[] {
            "TopRight",
            IPosition.TopRight,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(100, 200, 45, 30)
        },
        new object[] {
            "CenterLeft",
            IPosition.CenterLeft,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(95, 200, 45, 30)
        },
        new object[] {
            "Center",
            IPosition.Center,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(100, 200, 40, 30),
        },
        new object[] {
            "CenterRight",
            IPosition.CenterRight,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(100, 200, 45, 30)
        },
        new object[] {
            "BottomLeft",
            IPosition.BottomLeft,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(95, 200, 45, 30)
        },
        new object[] {
            "BottomCenter",
            IPosition.BottomCenter,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(100, 200, 40, 40),
        },
        new object[] {
            "BottomRight",
            IPosition.BottomRight,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(100, 200, 45, 30)
        },
        new object[] {
            "Fill",
            IPosition.Fill,
            new Rectangle(100, 200, 40, 30),
            new Vector2(5, 10), 
            new Rectangle(100, 200, 40, 30)
        },
    };
}