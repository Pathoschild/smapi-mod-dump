/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

global using XNA = Microsoft.Xna.Framework;
global using XColor = Microsoft.Xna.Framework.Color;
global using XGraphics = Microsoft.Xna.Framework.Graphics;
global using XSpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;
global using XTexture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
global using XRectangle = Microsoft.Xna.Framework.Rectangle;
global using XVector2 = Microsoft.Xna.Framework.Vector2;
global using DefaultScaler = SpriteMaster.Resample.Scalers.xBRZ;
global using DrawingColor = System.Drawing.Color;
global using DrawingPoint = System.Drawing.Point;
global using DrawingRectangle = System.Drawing.Rectangle;
global using DrawingSize = System.Drawing.Size;
global using half = System.Half;
global using XTilePoint = xTile.Dimensions.Location;
global using XTileRectangle = xTile.Dimensions.Rectangle;
global using XTileSize = xTile.Dimensions.Size;
global using PureAttribute = System.Diagnostics.Contracts.PureAttribute;
using System;
using System.Runtime.CompilerServices;
using System.Security;
// ReSharper disable StringLiteralTypo

// https://stackoverflow.com/questions/24802222/performance-of-expression-trees#comment44537873_24802222
[assembly: CLSCompliant(false)]
[assembly: AllowPartiallyTrustedCallers]
[assembly: SecurityTransparent]
[assembly: InternalsVisibleTo("Preview")]
[assembly: InternalsVisibleTo("Benchmarks.BenchmarkBase")]
[assembly: InternalsVisibleTo("Hashing")]
[assembly: InternalsVisibleTo("Arrays")]
[assembly: InternalsVisibleTo("Sprites")]
[assembly: InternalsVisibleTo("Strings")]
[assembly: InternalsVisibleTo("Math")]
[assembly: SecurityRules(SecurityRuleSet.Level2, SkipVerificationInFullTrust = true)]
[assembly: ChangeList("cb3cf6c:0.15.0-beta.16-1-gcb3cf6c")]
[assembly: BuildComputerName("Palatinate")]
[assembly: FullVersion("0.15.0.116.0-beta.16.0")]

[module: CLSCompliant(false)]
[module: SkipLocalsInit]

[AttributeUsage(validOn: AttributeTargets.Assembly)]
internal sealed class ChangeListAttribute : Attribute {
	internal readonly string Value;
	internal ChangeListAttribute(string value) => Value = value;
}

[AttributeUsage(validOn: AttributeTargets.Assembly)]
internal sealed class BuildComputerNameAttribute : Attribute {
	internal readonly string Value;
	internal BuildComputerNameAttribute(string value) => Value = value;
}

[AttributeUsage(validOn: AttributeTargets.Assembly)]
internal sealed class FullVersionAttribute : Attribute {
	internal readonly string Value;
	internal FullVersionAttribute(string value) => Value = value;
}