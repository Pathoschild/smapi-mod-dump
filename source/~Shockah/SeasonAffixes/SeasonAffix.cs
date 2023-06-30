/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

namespace Shockah.SeasonAffixes;

internal abstract class BaseSeasonAffix
{
	protected static SeasonAffixes Mod
		=> SeasonAffixes.Instance;

	public string UniqueID { get; init; }
	protected string I18nPrefix { get; init; }

	public virtual string LocalizedName => Mod.Helper.Translation.Get($"{I18nPrefix}.name");

	protected BaseSeasonAffix(string shortUniqueID, string affixType)
	{
		this.UniqueID = $"{Mod.ModManifest.UniqueID}.{shortUniqueID}";
		this.I18nPrefix = $"affix.{affixType}.{shortUniqueID}";
	}

	public override bool Equals(object? obj)
		=> obj is ISeasonAffix affix && UniqueID == affix.UniqueID;

	public override int GetHashCode()
		=> UniqueID.GetHashCode();
}

internal abstract class BaseVariantedSeasonAffix : BaseSeasonAffix
{
	public AffixVariant Variant { get; init; }

	protected BaseVariantedSeasonAffix(string shortUniqueID, AffixVariant variant) : base(shortUniqueID, variant == AffixVariant.Positive ? "positive" : "negative")
	{
		this.Variant = variant;
	}
}