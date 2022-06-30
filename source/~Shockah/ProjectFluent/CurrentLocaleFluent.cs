/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;

namespace Shockah.ProjectFluent
{
	internal class CurrentLocaleFluent: IFluent<string>
	{
		private IManifest Mod { get; set; }
		private string? File { get; set; }

		private IGameLocale? Locale { get; set; }
		private IFluent<string> Wrapped { get; set; } = null!;

		public CurrentLocaleFluent(IManifest mod, string? file = null)
		{
			this.Mod = mod;
			this.File = file;
		}

		private IFluent<string> CurrentFluent
		{
			get
			{
				if (Locale is null || Locale.LocaleCode != ProjectFluent.Instance.CurrentLocale.LocaleCode)
				{
					Locale = ProjectFluent.Instance.CurrentLocale;
					Wrapped = ProjectFluent.Instance.Api.GetLocalizations(Locale, Mod, File);
				}
				return Wrapped;
			}
		}

		public bool ContainsKey(string key)
			=> CurrentFluent.ContainsKey(key);

		public string Get(string key, object? tokens)
			=> CurrentFluent.Get(key, tokens);
	}
}