/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

global using XVector2 = Microsoft.Xna.Framework.Vector2;
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