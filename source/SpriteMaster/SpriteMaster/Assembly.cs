/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

global using XNA = Microsoft.Xna.Framework;
global using DrawingColor = System.Drawing.Color;
global using DrawingPoint = System.Drawing.Point;
global using DrawingRectangle = System.Drawing.Rectangle;
global using DrawingSize = System.Drawing.Size;
global using XTilePoint = xTile.Dimensions.Location;
global using XTileRectangle = xTile.Dimensions.Rectangle;
global using XTileSize = xTile.Dimensions.Size;
global using half = System.Half;
global using XTexture2D = Microsoft.Xna.Framework.Graphics.Texture2D;

global using DefaultScaler = SpriteMaster.Resample.Scalers.xBRZ;

using System.Runtime.CompilerServices;
using System.Security;
using System;

[assembly: CompilationRelaxations(CompilationRelaxations.NoStringInterning)]

// https://stackoverflow.com/questions/24802222/performance-of-expression-trees#comment44537873_24802222
[assembly: CLSCompliant(false)]
[assembly: AllowPartiallyTrustedCallers]
[assembly: SecurityTransparent]
[assembly: InternalsVisibleToAttribute("xBRZ")]
[assembly: InternalsVisibleToAttribute("Hashing")]
[assembly: SecurityRules(SecurityRuleSet.Level2, SkipVerificationInFullTrust = true)]
[assembly: ChangeList("7ca2ba6:0.13.0-beta.2-1-g7ca2ba6")]
[assembly: BuildComputerName("Palatinate")]
[assembly: FullVersion("0.13.0.103-beta.3")]
// [assembly: SuppressUnmanagedCodeSecurity]

[module: CompilationRelaxations(CompilationRelaxations.NoStringInterning)]
[module: CLSCompliant(false)]
[module: SkipLocalsInit]

[AttributeUsage(validOn: AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
sealed class ChangeListAttribute : Attribute {
	internal readonly string Value;
	internal ChangeListAttribute(string value) => Value = value;
}

[AttributeUsage(validOn: AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
sealed class BuildComputerNameAttribute : Attribute {
	internal readonly string Value;
	internal BuildComputerNameAttribute(string value) => Value = value;
}

[AttributeUsage(validOn: AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
sealed class FullVersionAttribute : Attribute {
	internal readonly string Value;
	internal FullVersionAttribute(string value) => Value = value;
}